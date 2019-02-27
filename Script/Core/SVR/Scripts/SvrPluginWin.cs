using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

class SvrPluginWin : SvrPlugin
{
	public static SvrPluginWin Create()
	{
		return new SvrPluginWin ();
	}

    private SvrPluginWin() { }

    public override bool IsInitialized() { return svrCamera != null; }

    public override bool IsRunning() { return eyes != null; }

    public override IEnumerator Initialize()
	{
        yield return base.Initialize();

        deviceInfo = GetDeviceInfo();

        yield break;
	}

	public override IEnumerator BeginVr(int cpuPerfLevel, int gpuPerfLevel)
	{
        yield return base.BeginVr(cpuPerfLevel, gpuPerfLevel);

		yield break;
    }
	
    public override void SetVSyncCount(int vSyncCount)
    {
        QualitySettings.vSyncCount = vSyncCount;
    }

    public override int GetHeadPose(ref HeadPose headPose, int frameIndex)
	{
        int poseStatus = 0;

		headPose.orientation = Quaternion.identity;
        headPose.position = Vector3.zero;

        if (Input.GetMouseButton(0))    // 0/Left mouse button
        {
            poseStatus |= (int)TrackingMode.kTrackingOrientation;
            poseStatus |= (int)TrackingMode.kTrackingPosition;
        }

        if (Input.GetMouseButton(1))    // 1/Right mouse button
		{
			Vector2 mouseNDC = Vector2.zero;
			mouseNDC.x = 2 * (Input.mousePosition.x / Screen.width) - 1f;
			mouseNDC.y = 2 * (Input.mousePosition.y / Screen.height) - 1f;

			Vector3 eulers = Vector3.zero;
			eulers.y = mouseNDC.x * 90f;  // +/- degrees
			eulers.x = -mouseNDC.y * 45f;  // +/- degrees

            headPose.orientation.eulerAngles = eulers;

            poseStatus |= (int)TrackingMode.kTrackingOrientation;
        }

        if (Input.GetMouseButton(2))    // 2/Middle mouse button
        {
            Vector2 mouseNDC = Vector2.zero;
            mouseNDC.x = 2 * (Input.mousePosition.x / Screen.width) - 1f;
            mouseNDC.y = 2 * (Input.mousePosition.y / Screen.height) - 1f;

            headPose.position.x = mouseNDC.x;
            headPose.position.z = mouseNDC.y;

            poseStatus |= (int)TrackingMode.kTrackingPosition;
        }

        return poseStatus;
    }

	public override DeviceInfo GetDeviceInfo()
	{
		DeviceInfo info 			= new DeviceInfo();

		info.displayWidthPixels 	= Screen.width;
		info.displayHeightPixels 	= Screen.height;
		info.displayRefreshRateHz 	= 60.0f;
		info.targetEyeWidthPixels 	= Screen.width / 2;
		info.targetEyeHeightPixels 	= Screen.height;
		info.targetFovXRad			= Mathf.Deg2Rad * 90;
		info.targetFovYRad			= Mathf.Deg2Rad * 90;
        info.targetFrustumLeft.left     = -0.0428f;
        info.targetFrustumLeft.right    = 0.0428f;
        info.targetFrustumLeft.top      = 0.0428f;
        info.targetFrustumLeft.bottom   = -0.0428f;
        info.targetFrustumLeft.near     = 0.0508f;
        info.targetFrustumLeft.far      = 100f;
        info.targetFrustumRight.left    = -0.0428f;
        info.targetFrustumRight.right   = 0.0428f;
        info.targetFrustumRight.top     = 0.0428f;
        info.targetFrustumRight.bottom  = -0.0428f;
        info.targetFrustumRight.near    = 0.0508f;
        info.targetFrustumRight.far     = 100f;
        return info;
	}

	public override void SubmitFrame(int frameIndex, float fieldOfView, int frameType)
	{
		RenderTexture.active = null;
		GL.Clear (false, true, Color.black);

		//float cameraFov = fieldOfView;
		//float fovMarginX = (cameraFov / deviceInfo.targetFovXRad) - 1;
		//float fovMarginY = (cameraFov / deviceInfo.targetFovYRad) - 1;
        //Rect textureRect = new Rect(fovMarginX * 0.5f, fovMarginY * 0.5f, 1 - fovMarginX, 1 - fovMarginY);
        Rect textureRect = new Rect(0, 0, 1, 1);

        Vector2 leftCenter = new Vector2(Screen.width * 0.25f, Screen.height * 0.5f);
		Vector2 rightCenter = new Vector2(Screen.width * 0.75f, Screen.height * 0.5f);
		Vector2 eyeExtent = new Vector3(Screen.width * 0.25f, Screen.height * 0.5f);
		eyeExtent.x -= 10.0f;
		eyeExtent.y -= 10.0f;

		Rect leftScreen = Rect.MinMaxRect(
            leftCenter.x - eyeExtent.x, 
            leftCenter.y - eyeExtent.y, 
            leftCenter.x + eyeExtent.x, 
            leftCenter.y + eyeExtent.y);
		Rect rightScreen = Rect.MinMaxRect(
            rightCenter.x - eyeExtent.x, 
            rightCenter.y - eyeExtent.y, 
            rightCenter.x + eyeExtent.x, 
            rightCenter.y + eyeExtent.y);

        if (eyes != null) for (int i = 0; i < eyes.Length; i++)
        {
            if (eyes[i].isActiveAndEnabled == false) continue;
            if (eyes[i].TexturePtr == null) continue;
            if (eyes[i].imageTransform != null && eyes[i].imageTransform.gameObject.activeSelf == false) continue;
            if (eyes[i].imageTransform != null && !eyes[i].imageTransform.IsChildOf(svrCamera.transform)) continue;   // svr only

            var eyeRectMin = eyes[i].clipLowerLeft; eyeRectMin /= eyeRectMin.w;
            var eyeRectMax = eyes[i].clipUpperRight; eyeRectMax /= eyeRectMax.w;

            if (eyes[i].Side == SvrEye.eSide.Left || eyes[i].Side == SvrEye.eSide.Both)
            {
                leftScreen = Rect.MinMaxRect(
                    leftCenter.x + eyeExtent.x * eyeRectMin.x, 
                    leftCenter.y + eyeExtent.y * eyeRectMin.y, 
                    leftCenter.x + eyeExtent.x * eyeRectMax.x, 
                    leftCenter.y + eyeExtent.y * eyeRectMax.y);
                Graphics.DrawTexture(leftScreen, eyes[i].TexturePtr, textureRect, 0, 0, 0, 0);
            }
            if (eyes[i].Side == SvrEye.eSide.Right || eyes[i].Side == SvrEye.eSide.Both)
            {
                rightScreen = Rect.MinMaxRect(
                    rightCenter.x + eyeExtent.x * eyeRectMin.x,
                    rightCenter.y + eyeExtent.y * eyeRectMin.y,
                    rightCenter.x + eyeExtent.x * eyeRectMax.x,
                    rightCenter.y + eyeExtent.y * eyeRectMax.y);
                Graphics.DrawTexture(rightScreen, eyes[i].TexturePtr, textureRect, 0, 0, 0, 0);
            }
        }

        if (overlays != null) for (int i = 0; i < overlays.Length; i++)
        {
            if (overlays[i].isActiveAndEnabled == false) continue;
            if (overlays[i].TexturePtr == null) continue;
            if (overlays[i].imageTransform != null && overlays[i].imageTransform.gameObject.activeSelf == false) continue;
            if (overlays[i].imageTransform != null && !overlays[i].imageTransform.IsChildOf(svrCamera.transform)) continue;   // svr only

            var eyeRectMin = overlays[i].clipLowerLeft; eyeRectMin /= eyeRectMin.w;
            var eyeRectMax = overlays[i].clipUpperRight; eyeRectMax /= eyeRectMax.w;

            textureRect.Set(overlays[i].uvLowerLeft.x, overlays[i].uvLowerLeft.y,
                overlays[i].uvUpperRight.x - overlays[i].uvLowerLeft.x,
                overlays[i].uvUpperRight.y - overlays[i].uvLowerLeft.y);

            if (overlays[i].Side == SvrOverlay.eSide.Left || overlays[i].Side == SvrOverlay.eSide.Both)
            {
                leftScreen = Rect.MinMaxRect(
                    leftCenter.x + eyeExtent.x * eyeRectMin.x,
                    leftCenter.y + eyeExtent.y * eyeRectMin.y,
                    leftCenter.x + eyeExtent.x * eyeRectMax.x,
                    leftCenter.y + eyeExtent.y * eyeRectMax.y);
                Graphics.DrawTexture(leftScreen, overlays[i].TexturePtr, textureRect, 0, 0, 0, 0);
            }
            if (overlays[i].Side == SvrOverlay.eSide.Right || overlays[i].Side == SvrOverlay.eSide.Both)
            {
                rightScreen = Rect.MinMaxRect(
                    rightCenter.x + eyeExtent.x * eyeRectMin.x,
                    rightCenter.y + eyeExtent.y * eyeRectMin.y,
                    rightCenter.x + eyeExtent.x * eyeRectMax.x,
                    rightCenter.y + eyeExtent.y * eyeRectMax.y);
                Graphics.DrawTexture(rightScreen, overlays[i].TexturePtr, textureRect, 0, 0, 0, 0);
            }
        }
 
	}

	public override void Shutdown()
	{
        base.Shutdown();
    }
}
