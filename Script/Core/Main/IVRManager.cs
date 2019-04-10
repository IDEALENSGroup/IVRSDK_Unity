using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using IDEALENS.IVR.EventSystems;
using IDEALENS.IVR.InputPlugin;
using System;

/////////////////////////////
/// Description : IVRManager - 处理应用初始化
/// Author 		: TanSir
/// Date 		: 2019
/// Copyright   : IDEALENS
/////////////////////////////
using IDEALENS.IVR.Utility;

namespace IDEALENS.IVR
{
	/// <summary>
	/// IVR manager.
	/// tansir
	/// </summary>

	public class IVRManager : MonoBehaviour
	{

		/// <summary>
		/// Instance
		/// </summary>
		private static IVRManager instance;
		public static IVRManager Instance {
			get {
				if (instance == null)
					instance = FindObjectOfType<IVRManager> ();
				if (instance == null)
					Debug.LogError ("Instance object component not found");
				return instance;
			}
		}


		/// <summary>
		/// Gets the cam manager.
		/// </summary>
		public static SvrManager IVRCam {
			get {
				return SvrManager.Instance;
			}
		}

		/// <summary>
		/// reset position.(Default true)
		/// </summary>
		[Tooltip ("Reset tracking,when you trigger longpress(2s.) event,default 'true'")]
		[SerializeField, SetProperty("EnableResetPosition")]
		public bool enableResetPosition = true;
		public bool EnableResetPosition
		{
			get{ 
				return enableResetPosition;
			}
			set{ 
				enableResetPosition = value;
				IVRBridge.enableResetPosition = enableResetPosition;
			}
		}


		/// <summary>
		/// The dont destroy.
		/// </summary>
		[Tooltip ("Do not Destroy this object,default 'false'")]
		public bool dontDestroy = false;


		/// <summary>
		/// CameraRig
		/// </summary>
		public Transform head;
		public Transform gaze;
		public Camera monoCamera;
		public Camera leftCamera;
		public Camera rightCamera;
		public Camera leftOverlay;
		public Camera rightOverlay;
		public Camera monoOverlay;
		public SvrOverlay fadeOverlay;
		public SvrOverlay reticleOverlay;

		/// <summary>
		/// Hand
		/// </summary>
		public Transform hand;


		// Android Device is Running
		public static bool IsRunDevice { 
			get{
				return (Application.platform == RuntimePlatform.Android);
			}
		}

		void Awake ()
		{
			// SnapdragonVR
			ConfigureSnapdragonVR ();

			// Open Log
			IVRLog.isLog = IVRSettings.IsDebugMode;

			// SvrManager? 
			SvrManager svrManager = GameObject.FindObjectOfType<SvrManager> ();
			if (svrManager == null) {
				IVRLog.Error ("SvrManager not init,Please add it.");
				return;
			}

			// IVRInputModuleManager? 
			IVRInputModuleManager inputManager = GameObject.FindObjectOfType<IVRInputModuleManager> ();
			if (inputManager == null) {
				IVRLog.Error ("Don't Forget Add IVRInputModuleManager(Prefab)!!!");
				return;
			}
				
			// add script to head
			if (head != null) {
				IVRPhysicsRaycaster raycast = head.GetComponent<IVRPhysicsRaycaster> ();
				if (raycast == null) {
					head.gameObject.AddComponent<IVRPhysicsRaycaster> ();
				} else {
					IVRLog.Warning ("IVRPhysicsRaycaster already add!");
				}
			} else {
				IVRLog.Error ("Head Object is null!");
			}
				
			// init bridge
			IVRBridge.enableResetPosition = enableResetPosition;
			IVRBridge.Init (gameObject);

			// init svr service and get interface
			InitVRService ();

			// default set left/right open
			EnableCameraMode ();

			// Open Overlay Cameras
			EnableOverlayMode (true);

			// Get Config and show fps
			OnFPSState ();

			if (dontDestroy)
				DontDestroyOnLoad (gameObject);

			Input.backButtonLeavesApp = false;
		}
			
		/// <summary>
		/// Init SVR Service
		/// </summary>
		void InitVRService()
		{
			IIVRService svrService = new SVRService();
			IVRBridge.ivrClient.Set_ServiceImpl (svrService);
		}

		void Start()
		{
			IVRBridge.OnHandlerConnectService ();
			TrackPlugin.OnHandlerConnected += OnHandlerConnected;
		}
			
		void Update ()
		{
			IVRBridge.Update ();
		}

		void OnDestroy()
		{
			IVRBridge.Dispose ();
			TrackPlugin.OnHandlerConnected -= OnHandlerConnected;
		}



		#region Android Interface
		/// <summary>
		/// 显示Toast(兼容以前的接口而保留,使用IVRPlugin.ShowToast()代替)
		/// </summary>
		/// <param name="mssage">信息内容.</param>
		/// <param name="time">显示时间.</param>
		[System.Obsolete]
		public void Show (string mssage, int time = 3000)
		{
			IVRPlugin.ShowToast (mssage, time);
		}
		#endregion
			

		/// <summary>
		/// get config file to show fps state 
		/// </summary>
		private void OnFPSState()
		{
			IVRPlugin.InitConfigFile ();
			bool isOpenFPS = IVRPlugin.GetConfigFPSState ();
			//bool isOpenFPS = true;
			if (isOpenFPS) {
				GameObject FPSObj = (GameObject)Instantiate (Resources.Load ("Prefabs/DebugFPSCanvas"));
				FPSObj.transform.parent = head;
			}
		}

		#region SnapdragonVR
		private void ConfigureSnapdragonVR ()
		{
			IVRCam.head = head;
			IVRCam.gaze = gaze;
			IVRCam.monoCamera = monoCamera;
			IVRCam.leftCamera = leftCamera;
			IVRCam.rightCamera = rightCamera;
			IVRCam.leftOverlay = leftOverlay;
			IVRCam.rightOverlay = rightOverlay;
			IVRCam.monoOverlay = monoOverlay;
			IVRCam.fadeOverlay = fadeOverlay;
			IVRCam.reticleOverlay = reticleOverlay;

			IVRCam.settings.cpuPerfLevel = SvrManager.SvrSettings.ePerfLevel.Maximum;
			IVRCam.settings.gpuPerfLevel = SvrManager.SvrSettings.ePerfLevel.Maximum;
			IVRCam.settings.eyeAntiAliasing = SvrManager.SvrSettings.eAntiAliasing.k4;

			// Unity 2018+
			//if (IVRPlugin.IsUnityVersion_2018Plus ()) {
			//	IVRCam.settings.vSyncCount = SvrManager.SvrSettings.eVSyncCount.k1;
			//} 
			// Unity 5.6+
			//if (IVRPlugin.IsUnityVersion_56Plus ()) {
			IVRCam.settings.vSyncCount = SvrManager.SvrSettings.eVSyncCount.k1;
			//}

			// 初始化
			IVRCam.Init ();

			Debug.Log ("Quality Settings:" + QualitySettings.antiAliasing);
			Debug.Log ("SyncCount Settings:" + QualitySettings.vSyncCount);

			// 参数配置
			/*
			svrManager.settings.poseStatusFade = true;
			svrManager.settings.trackEyes = true;
			svrManager.settings.trackPosition = true;
			svrManager.settings.trackPositionScale = 1.0f;
			svrManager.settings.headHeight = 0.075f;
			svrManager.settings.headDepth = 0.0805f;
			svrManager.settings.interPupilDistance = 0.064f;
			svrManager.settings.eyeFovMargin = 0;
			svrManager.settings.eyeResolutionScaleFactor = 1.0f;
			svrManager.settings.eyeDepth = SvrManager.SvrSettings.eDepth.k24;
			svrManager.settings.eyeAntiAliasing = SvrManager.SvrSettings.eAntiAliasing.k2;
			svrManager.settings.overlayResolutionScaleFactor = 1.0f;
			svrManager.settings.overlayDepth = SvrManager.SvrSettings.eDepth.k16;
			svrManager.settings.overlayAntiAliasing = SvrManager.SvrSettings.eAntiAliasing.k1;
			svrManager.settings.vSyncCount = SvrManager.SvrSettings.eVSyncCount.k0;
			svrManager.settings.chromaticAberationCorrection = SvrManager.SvrSettings.eChromaticAberrationCorrection.kEnable;
			svrManager.settings.masterTextureLimit = SvrManager.SvrSettings.eMasterTextureLimit.k0;
			svrManager.settings.cpuPerfLevel = SvrManager.SvrSettings.ePerfLevel.Maximum;
			svrManager.settings.gpuPerfLevel = SvrManager.SvrSettings.ePerfLevel.Maximum;
			svrManager.settings.foveationGain = Vector2.zero;
			svrManager.settings.foveationArea = 0;
			svrManager.settings.foveationMinimum = 0.25f;
			svrManager.settings.frustumType = SvrManager.SvrSettings.eFrustumType.Device;
			svrManager.settings.displayType = SvrManager.SvrSettings.eEyeBufferType.StereoSeperate;*/
		}
		#endregion

		#region Handler Callback
		/// <summary>
		/// 手柄已连接状态
		/// </summary>
		private void OnHandlerConnected()
		{
			// 连接后重置视角
			if (IVRPlugin.IsRunningOnAndroidDevice && TrackPlugin.isConnected) {

				StartCoroutine (ResetHandlerDelay ());
			}
		}
		private IEnumerator ResetHandlerDelay()
		{
			yield return new WaitForSeconds (0.2f);
			IVRPlugin.IVR_ResetPose ();
			TrackPlugin.RecenterTrackingOrigin();

		}
		#endregion

		/// <summary>
		/// Camera Mode
		/// </summary>
		public void EnableCameraMode ()
		{
			leftCamera.gameObject.SetActive (true);
			rightCamera.gameObject.SetActive (true);
			monoCamera.gameObject.SetActive (false);
		}

		/// <summary>
		/// Open/Close Overlay Mode(该模式开启后可以解决锚点抖动的问题)
		/// </summary>
		public void EnableOverlayMode (bool isEnable)
		{
			if (isEnable) {

				monoCamera.gameObject.SetActive (false);
				leftOverlay.gameObject.SetActive (true);
				rightOverlay.gameObject.SetActive (true);

				leftCamera.cullingMask &= ~(1 << LayerMask.NameToLayer ("Overlay_Object"));
				rightCamera.cullingMask &= ~(1 << LayerMask.NameToLayer ("Overlay_Object"));

				leftOverlay.cullingMask = 1 << LayerMask.NameToLayer ("Overlay_Object");
				rightOverlay.cullingMask = 1 << LayerMask.NameToLayer ("Overlay_Object");

				// gaze object
				gaze.gameObject.layer = LayerMask.NameToLayer ("Overlay_Object");

			} else {
				//隐藏overlay camera
				monoOverlay.gameObject.SetActive (false);
				leftOverlay.gameObject.SetActive (false);
				rightOverlay.gameObject.SetActive (false);

				// 开启eyes camera的渲染
				monoCamera.cullingMask |= ~(1 << LayerMask.NameToLayer ("Overlay_Object"));
				leftCamera.cullingMask |= ~(1 << LayerMask.NameToLayer ("Overlay_Object"));
				rightCamera.cullingMask |= ~(1 << LayerMask.NameToLayer ("Overlay_Object"));

				// 只渲染Overlay_Object层级物体
				monoOverlay.cullingMask = 1 << LayerMask.NameToLayer ("Default");
				leftOverlay.cullingMask = 1 << LayerMask.NameToLayer ("Default");
				rightOverlay.cullingMask = 1 << LayerMask.NameToLayer ("Default");

				// gaze object
				gaze.gameObject.layer = LayerMask.NameToLayer ("Default");
			}
		}
			
		/// <summary>
		/// Close my eyes.
		/// </summary>
		public void CloseMyEyesCamera ()
		{
			int layer = 1 << 0;
			leftCamera.cullingMask &= ~layer;
			rightCamera.cullingMask &= ~layer;
		}

		/// <summary>
		/// Open my eyes.
		/// </summary>
		public void OpenMyEyesCamera ()
		{
			leftCamera.cullingMask = -1;
			rightCamera.cullingMask = -1;
			EnableOverlayMode (true);
		}

		/// <summary>
		/// Reset View
		/// </summary>
		public void RecenterTracking ()
		{
			IVRPlugin.IVR_ResetPose ();
		}

		/// <summary>
		/// Get Camera
		/// </summary>
		public Camera GetMainCamera ()
		{
			return monoCamera;
		}

		#region "Head" 
		public Quaternion GetHeadRotation()
		{
			return head.rotation;
		}
		public Vector3 GetHeadPosition()
		{
			return head.position;
		}
		public Vector3 GetHeadForward()
		{
			return head.forward;
		}
		public Transform GetHead()
		{
			return head;
		}
		#endregion

		#region Unity Apllication Method
		void OnApplicationPause(bool pause)
		{
			if (pause)
			{
				IVRBridge.OnHandlerDisconnectService ();
			}
			else
			{
				StartCoroutine(IVRBridge.Resume());
				IVRBridge.OnHandlerConnectService ();
			}
		}

		void OnApplicationQuit()
		{
			IVRBridge.OnHandlerDisconnectService ();
		}
		#endregion

	}
}