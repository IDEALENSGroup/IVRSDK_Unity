using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

class SvrPluginAndroid : SvrPlugin
{
	[DllImport("svrplugin")]
	private static extern IntPtr GetRenderEventFunc();

    [DllImport("svrplugin")]
    private static extern bool SvrIsInitialized();

    [DllImport("svrplugin")]
    private static extern bool SvrIsRunning();

    [DllImport("svrplugin")]
    private static extern bool SvrCanBeginVR();

    [DllImport("svrplugin")]
	private static extern void SvrInitializeEventData(IntPtr activity);

	[DllImport("svrplugin")]
	private static extern void SvrSubmitFrameEventData(int frameIndex, float fieldOfView, int frameType);

    [DllImport("svrplugin")]
    private static extern void SvrSetupLayerCoords(int layerIndex, float[] lowerLeft, float[] lowerRight, float[] upperLeft, float[] upperRight);
    [DllImport("svrplugin")]
    private static extern void SvrSetupLayerData(int layerIndex, int sideMask, int textureId, int textureType, int layerFlags);

    [DllImport("svrplugin")]
	private static extern void SvrSetTrackingModeEventData(int mode);

	[DllImport("svrplugin")]
	private static extern void SvrSetPerformanceLevelsEventData(int newCpuPerfLevel, int newGpuPerfLevel);

    [DllImport("svrplugin")]
    private static extern void SvrSetEyeEventData(int sideMask, int layerMask);

    [DllImport("svrplugin")]
	private static extern void SvrSetColorSpace(int colorSpace);

    [DllImport("svrplugin")]
    private static extern void SvrSetFrameOption(uint frameOption);

    [DllImport("svrplugin")]
    private static extern void SvrUnsetFrameOption(uint frameOption);

    [DllImport("svrplugin")]
    private static extern void SvrSetVSyncCount(int vSyncCount);

    [DllImport("svrplugin")]
    private static extern int SvrGetPredictedPose(ref float rx,
                                                   ref float ry,
                                                   ref float rz,
                                                   ref float rw,
                                                   ref float px,
                                                   ref float py,
                                                   ref float pz,
                                                   int frameIndex,
                                                   bool isMultiThreadedRender);
    [DllImport("svrplugin")]
    private static extern int SvrGetEyePose(ref int leftStatus,
                                            ref int rightStatus,
                                            ref int combinedStatus,
                                            ref float leftOpenness,
                                            ref float rightOpenness,
                                            ref float leftDirectionX,
                                            ref float leftDirectionY,
                                            ref float leftDirectionZ,
                                            ref float leftPositionX,
                                            ref float leftPositionY,
                                            ref float leftPositionZ,
                                            ref float rightDirectionX,
                                            ref float rightDirectionY,
                                            ref float rightDirectionZ,
                                            ref float rightPositionX,
                                            ref float rightPositionY,
                                            ref float rightPositionZ,
                                            ref float combinedDirectionX,
                                            ref float combinedDirectionY,
                                            ref float combinedDirectionZ,
                                            ref float combinedPositionX,
                                            ref float combinedPositionY,
                                            ref float combinedPositionZ,
                                            int frameIndex);

    [DllImport("svrplugin")]
    private static extern bool SvrRecenterTrackingPose();

    [DllImport("svrplugin")]
    private static extern int SvrGetTrackingMode();

    [DllImport("svrplugin")]
    private static extern void SvrGetDeviceInfo(ref int displayWidthPixels,
	                                            ref int displayHeightPixels,
	                                            ref float displayRefreshRateHz,
	                                            ref int targetEyeWidthPixels,
	                                            ref int targetEyeHeightPixels,
	                                            ref float targetFovXRad,
	                                       		ref float targetFovYRad,
                                                ref float leftFrustumLeft, ref float leftFrustumRight, ref float leftFrustumBottom, ref float leftFrustumTop, ref float leftFrustumNear, ref float leftEyeFrustumFar,
                                                ref float rightFrustumLeft, ref float rightFrustumRight, ref float rightFrustumBottom, ref float rightFrustumTop, ref float rightFrustumNear, ref float rightFrustumFar,
                                                ref float targetfrustumConvergence, ref float targetFrustumPitch);

    [DllImport("svrplugin")]
    private static extern void SvrSetFrameOffset(float[] delta);

    [DllImport("svrplugin")]
    private static extern void SvrSetFoveationParameters(int textureId, int previousId, float focalPointX, float focalPointY, float foveationGainX, float foveationGainY, float foveationArea, float foveationMinimum);

    [DllImport("svrplugin")]
    private static extern bool SvrPollEvent(ref int eventType, ref uint deviceId, ref float eventTimeStamp, int eventDataCount, uint[] eventData);
    
	//---------------------------------------------------------------------------------------------
	// Conotroller Apis
	//---------------------------------------------------------------------------------------------
    [DllImport("svrplugin")]
    private static extern int SvrControllerStartTracking(string desc);
    
    [DllImport("svrplugin")]
    private static extern void SvrControllerStopTracking(int handle);
    
    [DllImport("svrplugin")]
	private static extern void SvrControllerGetState(int handle, int space, ref SvrControllerState state);

	[DllImport("svrplugin")]
	private static extern void SvrControllerSendMessage (int handle, int what, int arg1, int arg2);

	[DllImport("svrplugin")]
	private static extern int SvrControllerQuery (int handle, int what, IntPtr mem, int size);
	//---------------------------------------------------------------------------------------------

    private enum RenderEvent
	{
		Initialize,
		BeginVr,
		EndVr,
		BeginEye,
		EndEye,
		SubmitFrame,
        Foveation,
		Shutdown,
		RecenterTracking,
		SetTrackingMode,
		SetPerformanceLevels
	};

	public static SvrPluginAndroid Create()
	{
		if(Application.isEditor)
		{
			Debug.LogError("SvrPlugin not supported in unity editor!");
			throw new InvalidOperationException();
		}

		return new SvrPluginAndroid ();
	}


	private SvrPluginAndroid() {}

	private void IssueEvent(RenderEvent e)
	{
		// Queue a specific callback to be called on the render thread
		GL.IssuePluginEvent(GetRenderEventFunc(), (int)e);
	}

    public override bool IsInitialized() { return SvrIsInitialized(); }

    public override bool IsRunning() { return SvrIsRunning(); }

    public override IEnumerator Initialize()
	{
        //yield return new WaitUntil(() => SvrIsInitialized() == false);  // Wait for shutdown

        yield return base.Initialize();

#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

		SvrInitializeEventData(activity.GetRawObject());
#endif
        IssueEvent(RenderEvent.Initialize);
		yield return new WaitUntil (() => SvrIsInitialized () == true);

        yield return null;  // delay one frame - fix for re-init w multi-threaded rendering

        deviceInfo = GetDeviceInfo();
    }

	public override IEnumerator BeginVr(int cpuPerfLevel, int gpuPerfLevel)
	{
        //yield return new WaitUntil(() => SvrIsRunning() == false);  // Wait for EndVr

        yield return base.BeginVr(cpuPerfLevel, gpuPerfLevel);

        // float[6]: x, y, z, w, u, v
        float[] lowerLeft = { -1f, -1f, 0f, 1f, 0f, 0f };
        float[] upperLeft = { -1f,  1f, 0f, 1f, 0f, 1f };
        float[] upperRight = { 1f,  1f, 0f, 1f, 1f, 1f };
        float[] lowerRight = { 1f, -1f, 0f, 1f, 1f, 0f };
        SvrSetupLayerCoords(-1, lowerLeft, lowerRight, upperLeft, upperRight);    // Layers/All

        SvrSetPerformanceLevelsEventData(cpuPerfLevel, gpuPerfLevel);
		
		ColorSpace space = QualitySettings.activeColorSpace;
		if(space == ColorSpace.Gamma)
		{
			// 0 == kColorSpaceLinear from svrApi.h
			SvrSetColorSpace(0);   //Unity will be supplying gamma space eye buffers into warp so we want
								   //to setup a linear color space display surface so no further gamma 
								   //correction is performed
		}
		else
		{
			// 1 == kColorSpaceSRGB from svrApi.h
			SvrSetColorSpace(1);	//Unity will be supplying linear space eye buffers into warp so we want
									//to setup an sRGB color space display surface to properly convert
									//incoming linear values into sRGB
		}
        
		yield return new WaitUntil(() => SvrCanBeginVR() == true);
        IssueEvent (RenderEvent.BeginVr);
        //yield return new WaitUntil(() => SvrIsRunning() == true);
    }

    public override void EndVr()
	{
        base.EndVr();
		IssueEvent (RenderEvent.EndVr);
	}

	public override void BeginEye(int sideMask, float[] frameDelta)
	{
        SvrSetFrameOffset(frameDelta);    // Enabled for foveation head orientation delta
        SvrSetEyeEventData(sideMask, 0);
        IssueEvent (RenderEvent.BeginEye);
	}

	public override void EndEye(int sideMask, int layerMask)
	{
        SvrSetEyeEventData(sideMask, layerMask);
        IssueEvent(RenderEvent.EndEye);
	}

    public override void SetTrackingMode(int modeMask)
    {
        SvrSetTrackingModeEventData(modeMask);
		IssueEvent (RenderEvent.SetTrackingMode);
    }

	public override void SetFoveationParameters(int textureId, int previousId, float focalPointX, float focalPointY, float foveationGainX, float foveationGainY, float foveationArea, float foveationMinimum)
	{
		SvrSetFoveationParameters(textureId, previousId, focalPointX, focalPointY, foveationGainX, foveationGainY, foveationArea, foveationMinimum);
	}

    public override void ApplyFoveation()
    {
        IssueEvent(RenderEvent.Foveation);
    }

    public override int GetTrackingMode()
    {
        return SvrGetTrackingMode();
    }

    public override void SetPerformanceLevels(int newCpuPerfLevel, int newGpuPerfLevel)
    {
        SvrSetPerformanceLevelsEventData((int)newCpuPerfLevel, (int)newGpuPerfLevel);
		IssueEvent (RenderEvent.SetPerformanceLevels);
    }

    public override void SetFrameOption(FrameOption frameOption)
    {
        SvrSetFrameOption((uint)frameOption);
    }

    public override void UnsetFrameOption(FrameOption frameOption)
    {
        SvrUnsetFrameOption((uint)frameOption);
    }

    public override void SetVSyncCount(int vSyncCount)
    {
        SvrSetVSyncCount(vSyncCount);
    }

    public override bool RecenterTracking()
	{
        //IssueEvent (RenderEvent.RecenterTracking);
        return SvrRecenterTrackingPose();
	}

    public override int GetPredictedPose(ref Quaternion orientation, ref Vector3 position, int frameIndex)
    {
        orientation.z = -orientation.z;
        position.x = -position.x;
        position.y = -position.y;

        int rv = SvrGetPredictedPose(ref orientation.x, ref orientation.y, ref orientation.z, ref orientation.w,
                            ref position.x, ref position.y, ref position.z, frameIndex, SystemInfo.graphicsMultiThreaded);

        orientation.z = -orientation.z;
        position.x = -position.x;
        position.y = -position.y;

        return rv;
    }
    public override int GetHeadPose(ref HeadPose headPose, int frameIndex)
    {
        headPose.orientation.z = -headPose.orientation.z;
        headPose.position.x = -headPose.position.x;
        headPose.position.y = -headPose.position.y;

        int rv = SvrGetPredictedPose(ref headPose.orientation.x, ref headPose.orientation.y, ref headPose.orientation.z, ref headPose.orientation.w,
                            ref headPose.position.x, ref headPose.position.y, ref headPose.position.z, frameIndex, SystemInfo.graphicsMultiThreaded);

        headPose.orientation.z = -headPose.orientation.z;
        headPose.position.x = -headPose.position.x;
        headPose.position.y = -headPose.position.y;

        return rv;
    }

    public override int GetEyePose(ref EyePose eyePose, int frameIndex = -1)
    {
        int rv = SvrGetEyePose(
            ref eyePose.leftStatus, ref eyePose.rightStatus, ref eyePose.combinedStatus,
            ref eyePose.leftOpenness, ref eyePose.rightOpenness,
            ref eyePose.leftDirection.x, ref eyePose.leftDirection.y, ref eyePose.leftDirection.z,
            ref eyePose.leftPosition.x, ref eyePose.leftPosition.y, ref eyePose.leftPosition.z,
            ref eyePose.rightDirection.x, ref eyePose.rightDirection.y, ref eyePose.rightDirection.z,
            ref eyePose.rightPosition.x, ref eyePose.rightPosition.y, ref eyePose.rightPosition.z,
            ref eyePose.combinedDirection.x, ref eyePose.combinedDirection.y, ref eyePose.combinedDirection.z,
            ref eyePose.combinedPosition.x, ref eyePose.combinedPosition.y, ref eyePose.combinedPosition.z,
            frameIndex);

        return rv;
    }

    public override DeviceInfo GetDeviceInfo()
	{
		DeviceInfo info = new DeviceInfo();

		SvrGetDeviceInfo (ref info.displayWidthPixels,
		                  ref info.displayHeightPixels,
		                  ref info.displayRefreshRateHz,
		                  ref info.targetEyeWidthPixels,
		                  ref info.targetEyeHeightPixels,
		                  ref info.targetFovXRad,
		                  ref info.targetFovYRad,
                          ref info.targetFrustumLeft.left, ref info.targetFrustumLeft.right, ref info.targetFrustumLeft.bottom, ref info.targetFrustumLeft.top, ref info.targetFrustumLeft.near, ref info.targetFrustumLeft.far,
                          ref info.targetFrustumRight.left, ref info.targetFrustumRight.right, ref info.targetFrustumRight.bottom, ref info.targetFrustumRight.top, ref info.targetFrustumRight.near, ref info.targetFrustumRight.far,
                          ref info.targetFrustumConvergence, ref info.targetFrustumPitch);

		return info;
	}

    public override void SubmitFrame(int frameIndex, float fieldOfView, int frameType)
	{
        int i;
        int layerCount = 0;
        if (eyes != null) for (i = 0; i < eyes.Length; i++)
        {
            var eye = eyes[i];
            if (eyes[i].isActiveAndEnabled == false || eye.TextureId == 0 || eye.Side == 0) continue;
            if (eye.imageTransform != null && eye.imageTransform.gameObject.activeSelf == false) continue;
            SvrSetupLayerData(layerCount, (int)eye.Side, eye.TextureId, eye.ImageType == SvrEye.eType.EglTexture ? 2 : 0, eye.layerDepth > 0 ? 0x0 : 0x2);
            float[] lowerLeft = { eye.clipLowerLeft.x, eye.clipLowerLeft.y, eye.clipLowerLeft.z, eye.clipLowerLeft.w, eye.uvLowerLeft.x, eye.uvLowerLeft.y };
            float[] upperLeft = { eye.clipUpperLeft.x, eye.clipUpperLeft.y, eye.clipUpperLeft.z, eye.clipUpperLeft.w, eye.uvUpperLeft.x, eye.uvUpperLeft.y };
            float[] upperRight = { eye.clipUpperRight.x, eye.clipUpperRight.y, eye.clipUpperRight.z, eye.clipUpperRight.w, eye.uvUpperRight.x, eye.uvUpperRight.y };
            float[] lowerRight = { eye.clipLowerRight.x, eye.clipLowerRight.y, eye.clipLowerRight.z, eye.clipLowerRight.w, eye.uvLowerRight.x, eye.uvLowerRight.y };
            SvrSetupLayerCoords(layerCount, lowerLeft, lowerRight, upperLeft, upperRight);
            layerCount++;
        }

        if (overlays != null) for (i = 0; i < overlays.Length; i++)
        {
            var overlay = overlays[i];
            if (overlay.isActiveAndEnabled == false || overlay.TextureId == 0 || overlay.Side == 0) continue;
            if (overlay.imageTransform != null && overlay.imageTransform.gameObject.activeSelf == false) continue;
            SvrSetupLayerData(layerCount, (int)overlay.Side, overlay.TextureId, overlay.ImageType == SvrOverlay.eType.EglTexture ? 2 : 0, 0x1);
            float[] lowerLeft = { overlay.clipLowerLeft.x, overlay.clipLowerLeft.y, overlay.clipLowerLeft.z, overlay.clipLowerLeft.w, overlay.uvLowerLeft.x, overlay.uvLowerLeft.y };
            float[] upperLeft = { overlay.clipUpperLeft.x, overlay.clipUpperLeft.y, overlay.clipUpperLeft.z, overlay.clipUpperLeft.w, overlay.uvUpperLeft.x, overlay.uvUpperLeft.y };
            float[] upperRight = { overlay.clipUpperRight.x, overlay.clipUpperRight.y, overlay.clipUpperRight.z, overlay.clipUpperRight.w, overlay.uvUpperRight.x, overlay.uvUpperRight.y };
            float[] lowerRight = { overlay.clipLowerRight.x, overlay.clipLowerRight.y, overlay.clipLowerRight.z, overlay.clipLowerRight.w, overlay.uvLowerRight.x, overlay.uvLowerRight.y };
            SvrSetupLayerCoords(layerCount, lowerLeft, lowerRight, upperLeft, upperRight);
            layerCount++;
        }

        for (i = layerCount; i < SvrManager.RenderLayersMax; i++)
        {
            SvrSetupLayerData(i, 0, 0, 0, 0);
        }

        SvrSubmitFrameEventData(frameIndex, fieldOfView, frameType);
		IssueEvent (RenderEvent.SubmitFrame);
	}

	public override void Shutdown()
	{
        IssueEvent (RenderEvent.Shutdown);

        base.Shutdown();
	}

    public override bool PollEvent(ref SvrManager.SvrEvent frameEvent)
    {
        uint[] dataBuffer = new uint[2];
        int dataCount = Marshal.SizeOf(frameEvent.eventData) / sizeof(uint);
		int eventType = 0;
        bool isEvent = SvrPollEvent(ref eventType, ref frameEvent.deviceId, ref frameEvent.eventTimeStamp, dataCount, dataBuffer);
		frameEvent.eventType = (SvrManager.svrEventType)(eventType);
        switch (frameEvent.eventType)
        {
            case SvrManager.svrEventType.kEventThermal:
                //Debug.LogFormat("PollEvent: data {0} {1}", dataBuffer[0], dataBuffer[1]);
                frameEvent.eventData.thermal.zone = (SvrManager.svrThermalZone)dataBuffer[0];
                frameEvent.eventData.thermal.level = (SvrManager.svrThermalLevel)dataBuffer[1];
                break;
        }
        return isEvent;
    }

	//---------------------------------------------------------------------------------------------
	//Controller Apis
	//---------------------------------------------------------------------------------------------

	/// <summary>
	/// Controllers the start tracking.
	/// </summary>
	/// <returns>The start tracking.</returns>
	/// <param name="desc">Desc.</param>
	//---------------------------------------------------------------------------------------------
    public override int ControllerStartTracking(string desc)
    {
        return SvrControllerStartTracking(desc);
    }
    
	/// <summary>
	/// Controllers the stop tracking.
	/// </summary>
	/// <param name="handle">Handle.</param>
	//---------------------------------------------------------------------------------------------
	public override void ControllerStopTracking(int handle)
    {
        SvrControllerStopTracking(handle);
    }

	/// <summary>
	/// Dumps the state.
	/// </summary>
	/// <param name="state">State.</param>
	//---------------------------------------------------------------------------------------------
	private void dumpState(SvrControllerState state)
	{
		String s = "{" + state.rotation + "}\n";
		s += "[" + state.position + "]\n";
		s += "<" + state.timestamp + ">\n";

		Debug.Log (s);
	}
    
	/// <summary>
	/// Controllers the state of the get.
	/// </summary>
	/// <returns>The get state.</returns>
	/// <param name="handle">Handle.</param>
	//---------------------------------------------------------------------------------------------
	public override SvrControllerState ControllerGetState(int handle, int space)
    {
		SvrControllerState state = new SvrControllerState();
		SvrControllerGetState (handle, space, ref state);
		//dumpState (state);
 		return state;
    }

	/// <summary>
	/// Controllers the send event.
	/// </summary>
	/// <param name="handle">Handle.</param>
	/// <param name="what">What.</param>
	/// <param name="arg1">Arg1.</param>
	/// <param name="arg2">Arg2.</param>
	//---------------------------------------------------------------------------------------------
	public override void ControllerSendMessage(int handle, SvrController.svrControllerMessageType what, int arg1, int arg2)
	{
		SvrControllerSendMessage (handle, (int)what, arg1, arg2);
	}

	/// <summary>
	/// Controllers the query.
	/// </summary>
	/// <returns>The query.</returns>
	/// <param name="handle">Handle.</param>
	/// <param name="what">What.</param>
	/// <param name="mem">Mem.</param>
	/// <param name="size">Size.</param>
	//---------------------------------------------------------------------------------------------
	public override object ControllerQuery(int handle, SvrController.svrControllerQueryType what)
	{
		int memorySize = 0;
		IntPtr memory = IntPtr.Zero;
		object result = null;

		System.Type typeOfObject = null;

		switch(what)
		{
			case SvrController.svrControllerQueryType.kControllerBatteryRemaining:
				{
					typeOfObject = typeof(int);
					memorySize = System.Runtime.InteropServices.Marshal.SizeOf (typeOfObject);
					memory = System.Runtime.InteropServices.Marshal.AllocHGlobal (memorySize);	
				}
				break;
			case SvrController.svrControllerQueryType.kControllerControllerCaps:
				{
                    typeOfObject = typeof(SvrControllerCaps);
					memorySize = System.Runtime.InteropServices.Marshal.SizeOf (typeOfObject);
					memory = System.Runtime.InteropServices.Marshal.AllocHGlobal (memorySize);	
				}
				break;				
		}

		int writtenBytes = SvrControllerQuery (handle, (int)what, memory, memorySize);

		if (memorySize == writtenBytes) {
			result = System.Runtime.InteropServices.Marshal.PtrToStructure (memory, typeOfObject);
		}
			
		if (memory != IntPtr.Zero) {
			Marshal.FreeHGlobal (memory);
		}
			
		return result;
	}
}
