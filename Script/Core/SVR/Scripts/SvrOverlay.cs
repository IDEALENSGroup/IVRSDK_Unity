using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SvrOverlay : MonoBehaviour, IComparable<SvrOverlay>
{
    public static List<SvrOverlay> Instances = new List<SvrOverlay>();

    public enum eSide
    {
        Left = 1,
        Right = 2,
        Both = 3,
        [HideInInspector]
        Count = Both
    };

    public enum eType
    {
        RenderTexture = 0,
        StandardTexture = 1,
        EglTexture = 2,
    };

    public delegate void OnPostRenderCallback(int sideMask, int layerMask);
    public OnPostRenderCallback OnPostRenderListener;

    public delegate void OnPreRenderCallback(int sideMask, int textureId, int previousId);
    public OnPreRenderCallback OnPreRenderListener;

    [Tooltip("Layer depth stack")]
    public int layerDepth = 0;
    [Tooltip("Image display transform")]
    public Camera imageCamera;
    [Tooltip("Image type: 0/Camera render target, 1/Texture 2d, 2/External egl")]
    public eType imageType = eType.RenderTexture;
    [Tooltip("Image texture used when ImageType is StandardTexture")]
    public Texture imageTexture;
    [Tooltip("Image transform for scale, rotation and position (optional)")]
    public Transform imageTransform;
    [Tooltip("Image display region (camera clip space)")]
    public Vector4 clipLowerLeft = new Vector4(-1, -1, 0, 1);
    public Vector4 clipUpperLeft = new Vector4(-1, 1, 0, 1);
    public Vector4 clipUpperRight = new Vector4(1, 1, 0, 1);
    public Vector4 clipLowerRight = new Vector4(1, -1, 0, 1);
    [Tooltip("Image source region (texture uv space)")]
    public Vector2 uvLowerLeft = new Vector2(0, 0);
    public Vector2 uvUpperLeft = new Vector2(0, 1);
    public Vector2 uvUpperRight = new Vector2(1, 1);
    public Vector2 uvLowerRight = new Vector2(1, 0);
    [Tooltip("Side mask")]
    public eSide side = eSide.Both;

    private RenderTextureFormat format = RenderTextureFormat.Default;
    private Vector2 resolution = new Vector2(1024.0f, 1024.0f);
    private float resolutionScaleFactor = 1.0f;
    private int antiAliasing = 1;
    private int depth = 24;
    private int frustumType = 0;
    private const int bufferCount = 3;
    private RenderTexture[] overlayTextures = new RenderTexture[bufferCount];
    private int[] overlayTextureIds = new int[bufferCount];
    private int currentTextureIndex = 0;
    private Camera[] mainCameras = null;
    private bool dirty = false;
    private Coroutine recreateBuffersCoroutine = null;

    public int CompareTo(SvrOverlay that)
    {
        return this.layerDepth.CompareTo(that.layerDepth);
    }

    public int FrustumType
    {
        get { return frustumType; }
        set { frustumType = value; }
    }

    public void SetImage(Texture2D texture)
    {
        imageTexture = texture;
        InitializeBuffers();
    }

    public eType ImageType
    {
        get { return imageType; }
        set { imageType = value; }
    }

    public eSide Side
    {
        get { return side; }
        set { side = value; }
    }
	
    public RenderTextureFormat Format
    {
        get { return format; }
        set { SetDirty(format != value); format = value; }
    }
	
    public int AntiAliasing
    {
        get { return antiAliasing; }
        set { SetDirty(antiAliasing != value); antiAliasing = value; }
    }
	
    public int Depth
    {
        get { return depth; }
        set { SetDirty(depth != value); depth = value; }
    }
	
    public Vector2 Resolution
    {
        get { return resolution; }
        set { SetDirty(!Mathf.Approximately(resolution.x, value.x) || !Mathf.Approximately(resolution.y, value.y)); resolution = value; }
    }

    public float ResolutionScaleFactor
    {
        get { return resolutionScaleFactor; }
        set { SetDirty(!Mathf.Approximately(resolutionScaleFactor, value)); resolutionScaleFactor = value; }
    }
	
    void SetDirty(bool value)
    {
        dirty = dirty == true ? true : value;
    }

    public int TextureId
    {
        get { return overlayTextureIds[currentTextureIndex]; }
        set { overlayTextureIds[currentTextureIndex] = value; }
    }
    public int PreviousId
    {
        get { return overlayTextureIds[(currentTextureIndex + bufferCount - 1) % bufferCount]; }
    }
    public Texture TexturePtr
    {
        get { return (imageTexture != null ? imageTexture : (Texture)overlayTextures[currentTextureIndex]); }
    }

    void Awake()
    {
        Instances.Add(this);
        AcquireComponents();
        InitializeCoords();
    }

    void OnDestroy()
    {
        Instances.Remove(this);
    }

    void AcquireComponents()
    {
        if (imageCamera == null) imageCamera = gameObject.GetComponent<Camera>();
        Debug.Assert(imageCamera != null, "ImageCamera object required");
        mainCameras = imageCamera.GetComponentsInChildren<Camera>();
    }

    void Start()
    {
        //Initialize(); Called by SvrManager.InitializeOverlays()
    }

    void LateUpdate()
    {
        UpdateCoords();
    }

    public void Initialize()
    {
        InitializeBuffers();
        InitializeCameras();
    }

    void InitializeBuffers()
    {
        for (int i = 0; i < bufferCount; ++i)
        {
            if (overlayTextures[i] != null)
                overlayTextures[i].Release();
            switch (imageType)
            {
                case eType.RenderTexture:
                    overlayTextures[i] = new RenderTexture((int)(resolution.x * resolutionScaleFactor), (int)(resolution.y * resolutionScaleFactor), depth, format);
                    overlayTextures[i].antiAliasing = antiAliasing;
                    overlayTextures[i].Create();
                    overlayTextureIds[i] = overlayTextures[i].GetNativeTexturePtr().ToInt32();
                    Debug.Log("Create Render Texture with ID: " + overlayTextureIds[i] + " Width: " + overlayTextures[i].width + " Height: " + overlayTextures[i].height + " AA: " + overlayTextures[i].antiAliasing);
                    break;

                case eType.StandardTexture:
                    if (imageTexture) overlayTextureIds[i] = imageTexture.GetNativeTexturePtr().ToInt32();
                    break;

                case eType.EglTexture:
                    overlayTextureIds[i] = 0;
                    break;
            }
        }
        dirty = false;
    }

    void InitializeCameras()
    {
        var deviceInfo = SvrPlugin.Instance.deviceInfo;
        var deviceFov = new Vector2(deviceInfo.targetFovXRad, deviceInfo.targetFovYRad) * Mathf.Rad2Deg;
        var frustum = side == eSide.Right ? deviceInfo.targetFrustumRight : deviceInfo.targetFrustumLeft;

        foreach (var mainCamera in mainCameras)
        {
            mainCamera.fieldOfView = deviceFov.y;
            mainCamera.aspect = deviceFov.x / deviceFov.y;
            if (frustumType == (int)SvrManager.SvrSettings.eFrustumType.Device)
            {
                mainCamera.projectionMatrix = SvrManager.Perspective(frustum.left, frustum.right, frustum.bottom, frustum.top, frustum.near, mainCamera.farClipPlane);
            }
        }
    }

    void InitializeCoords()
    {
        //clipLowerLeft.Set(-1, -1, 0, 1);
        //clipUpperLeft.Set(-1, 1, 0, 1);
        //clipUpperRight.Set(1, 1, 0, 1);
        //clipLowerRight.Set(1, -1, 0, 1);
    }

    void UpdateCoords()
    {
        if (imageTransform == null)
            return;

        var viewCamera = mainCameras[0];
        if (viewCamera == null)
            return;

        var extents = 0.5f * Vector3.one;
        var center = Vector3.zero;

        var worldLowerLeft = new Vector4(center.x - extents.x, center.y - extents.y, 0, 1);
        var worldUpperLeft = new Vector4(center.x - extents.x, center.y + extents.y, 0, 1);
        var worldUpperRight = new Vector4(center.x + extents.x, center.y + extents.y, 0, 1);
        var worldLowerRight = new Vector4(center.x + extents.x, center.y - extents.y, 0, 1);

        Matrix4x4 MVP = viewCamera.projectionMatrix * viewCamera.worldToCameraMatrix * imageTransform.localToWorldMatrix;

        clipLowerLeft = MVP * worldLowerLeft;
        clipUpperLeft = MVP * worldUpperLeft;
        clipUpperRight = MVP * worldUpperRight;
        clipLowerRight = MVP * worldLowerRight;
    }

    void OnPreRender()
    {
        if (imageType != eType.RenderTexture) return;

        SwapBuffers();

        if (OnPreRenderListener != null)
        {
            OnPreRenderListener((int)side, TextureId, PreviousId);
        }
    }

    void SwapBuffers()
    {
        if (imageType != eType.RenderTexture) return;

        currentTextureIndex = ++currentTextureIndex % bufferCount;
        var targetTexture = overlayTextures[currentTextureIndex];
        if (targetTexture == null) return;

        for (int i = 0; i < mainCameras.Length; i++)
        {
            mainCameras[i].targetTexture = targetTexture;
        }
        targetTexture.DiscardContents();
    }

    void OnPostRender()
    {
        RecreateBuffersIfDirty();
        if (OnPostRenderListener != null)
        {
            OnPostRenderListener((int)side, 0x1/*kLayerFlagHeadLocked*/);
        }
    }
	
    void RecreateBuffersIfDirty()
    {
        if (dirty)
        {
            if (recreateBuffersCoroutine != null)
            {
                StopCoroutine(recreateBuffersCoroutine);
                recreateBuffersCoroutine = null;
            }

            recreateBuffersCoroutine = StartCoroutine(RecreateBuffersDeferred());
            dirty = false;
        }
    }
	
    IEnumerator RecreateBuffersDeferred()
    {
        int i = 0;
        while (i < bufferCount)
        {
            int index = currentTextureIndex - 1;
            index = index >= 0 ? index : bufferCount - 1;

            if (overlayTextures[index] != null)
                overlayTextures[index].Release();

            switch (imageType)
            {
                case eType.RenderTexture:
                    overlayTextures[index] = new RenderTexture((int)(resolution.x * resolutionScaleFactor), (int)(resolution.y * resolutionScaleFactor), depth, format);
                    overlayTextures[index].antiAliasing = antiAliasing;
                    overlayTextures[index].Create();
                    overlayTextureIds[index] = overlayTextures[index].GetNativeTexturePtr().ToInt32();
                    Debug.Log("Re-create Render Texture with ID: " + overlayTextureIds[index] + " Width: " + overlayTextures[index].width + " Height: " + overlayTextures[index].height + " AA: " + overlayTextures[index].antiAliasing);
                    break;

                case eType.StandardTexture:
                    if (imageTexture) overlayTextureIds[index] = imageTexture.GetNativeTexturePtr().ToInt32();
                    break;

                case eType.EglTexture:
                    overlayTextureIds[index] = 0;
                    break;
            }

            int prevTextureIndex = currentTextureIndex;
            yield return new WaitUntil(() => currentTextureIndex != prevTextureIndex);

            i++;
        }

        yield break;
    }
	
}