using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using IDEALENS.IVR.Utility;

/////////////////////////////
/// Description : InputManager管理器
/// Author 		: TanSir
/// Date 		: 2019
/// Copyright   : IDEALENS
/////////////////////////////
using IDEALENS.IVR.InputPlugin;

namespace IDEALENS.IVR.EventSystems
{
	/// <summary>
	/// InputModuleManager
	/// tansir
	/// </summary>
	public class IVRInputModuleManager : MonoBehaviour {

		/// <summary>
		/// Instance
		/// </summary>
		private static IVRInputModuleManager instance = null;
		public static IVRInputModuleManager Instance{
			get{ 
				return instance;
			}
		}

		/// <summary>
		/// Destroy?
		/// </summary>
		public bool DoNotDestroyed = false;


		/// <summary>
		/// Add IVRGraphicRaycaster to Canvas automaticlly
		/// </summary>
		public bool bAutoGenerateCanvasRaycaster = false;

		/// <summary>
		/// Generate EventSystem automaticlly
		/// </summary>
		public bool bAutoGenerateEventSystem = true;

		[Tooltip("Use the handler buttons as common click")]
		[SerializeField, SetProperty("HandlerButton2CommonClick")]
		private HandlerTriggerButtonType handlerButton2CommonClick = HandlerTriggerButtonType.Button_TP;
		public HandlerTriggerButtonType HandlerButton2CommonClick
		{
			get{ 
				return handlerButton2CommonClick;
			}
			set{ 
				handlerButton2CommonClick = value;

				SetHandlerDefaultButton (handlerButton2CommonClick);
			}
		}

		[Tooltip("Enable/Disable Gaze Trigger Mode ?")]
		[SerializeField, SetProperty("EnableGazeTriggerMode")]
		private bool _enableGazeTriggerMode = false;
		public bool EnableGazeTriggerMode
		{
			get{ 
				return _enableGazeTriggerMode;
			}
			set{ 
				_enableGazeTriggerMode = value;
				if (inputModule != null) {
					inputModule.Impl.EnableGazeAutoTriggerMode = _enableGazeTriggerMode;
				}
			}
		}

		/// <summary>
		/// EventSystem 
		/// </summary>
		private GameObject eventSystem = null;

		/// <summary>
		/// InputModule
		/// </summary>
		public IVRInputModule InputModule
		{
			get{ 
				return inputModule;
			}	
		}
		private IVRInputModule inputModule = null;


		/// <summary>
		/// Do not put InitEvenySystem in Start()
		/// </summary>
		/// 
		void Awake()
		{
			if (instance == null)
				instance = this;
			
			InitEvenySystem ();
		}

		// Use this for initialization
		void InitEvenySystem () {

			IVRManager ivrManager = FindObjectOfType<IVRManager> ();
			if (ivrManager == null) {
				IVRLog.Error ("Don't Forget to add IVRManager(Prefab)!!!");
				return;
			}

			// Find EventSystem
			if (EventSystem.current == null) { 

				// Find InputModule
				EventSystem _es = FindObjectOfType<EventSystem> ();
				if (_es != null) {
					eventSystem = _es.gameObject;
				}
			} else {
				eventSystem = EventSystem.current.gameObject;
				if (eventSystem)
					eventSystem.name = "[IVR_EventSystem]";
			}
				

			// Auto Generate EventSystem?
			if (bAutoGenerateEventSystem) {

				// Get EventSystem
				if (eventSystem == null) {
					eventSystem = new GameObject ("[IVR_EventSystem]", typeof(EventSystem));
					eventSystem.AddComponent<IVRInputModule> ();
				}

				//if (bAutoGenerateCanvasRaycaster)
				//	AutoGenerateCanvasRaycaster ();

				// IVR Input Module
				inputModule = eventSystem.GetComponent<IVRInputModule> ();
				if (inputModule == null) {
					CreateInputModule ();
				}

				//if (!EnableInputModule) {
				//	disableAllInputModule ();
				//}
			} else {
				if(eventSystem == null)
					IVRLog.Error ("EventSystem does not exist,Please add EventSystem component by yourself,OR you can Set 'Gernerrate EventSystem' = true on IVRInputModuleManager inspector !!!");
			}



			if (eventSystem != null) {
				// Exsit Stardard InputModule
				StandaloneInputModule _sim = eventSystem.GetComponent<StandaloneInputModule> ();
				if (_sim != null)
					_sim.enabled = false;
				
				// set EventSytem as child node
				eventSystem.gameObject.transform.parent = transform;
			}


			if (inputModule != null) {
				// set default key for handler
				SetHandlerDefaultButton (handlerButton2CommonClick);
				inputModule.Impl.EnableGazeAutoTriggerMode = EnableGazeTriggerMode;
			}
				

			// Destroy?
			if (DoNotDestroyed) {
				DontDestroyOnLoad (gameObject);
			}

		}

		/// <summary>
		/// Set the handler default button.
		/// </summary>
		/// <param name="handlerButton2CommonClick">Handler button2 common click.</param>
		public void SetHandlerDefaultButton(HandlerTriggerButtonType handlerButtonType)
		{
			IVRInput.Button buttons = IVRInput.Button.None;

			//if ((handlerButton & ButtonCommon.CONTROLLER_BUTTON_TP_CLICK) !=0)
			//	buttons |= IVRInput.Button.PrimaryTouchpad;
			
			//if ((handlerButton & ButtonCommon.CONTROLLER_BUTTON_TRIGGER) != 0)
			//	buttons |= IVRInput.Button.PrimaryIndexTrigger;

			//if ((handlerButton & ButtonCommon.CONTROLLER_BUTTON_TRIGGER_AND_TP) != 0) {
			//	buttons |= IVRInput.Button.PrimaryTouchpad;
			//	buttons |= IVRInput.Button.PrimaryIndexTrigger;
			//}

			switch (handlerButtonType) {
			case HandlerTriggerButtonType.Button_TP:
				buttons |= IVRInput.Button.PrimaryTouchpad;
				break;
			case HandlerTriggerButtonType.Button_Trigger:
				buttons |= IVRInput.Button.PrimaryIndexTrigger;
				break;
			case HandlerTriggerButtonType.Button_TPAndTrigger:
				buttons |= IVRInput.Button.PrimaryTouchpad;
				buttons |= IVRInput.Button.PrimaryIndexTrigger;
				break;
			}

			IVRBridge.HandButtons = buttons;
			if (inputModule != null) {
				inputModule.handlerTriggerButtonType = handlerButtonType;
				inputModule.Impl.HandButtons = IVRBridge.HandButtons;
			}
		}

		/// <summary>
		/// Disable InputModule
		/// </summary>
		private void disableAllInputModule()
		{
			SetActiveGaze (false);
		}

		/// <summary>
		/// Active Anchor?
		/// </summary>
		/// <param name="value">If set to <c>true</c> value.</param>
		private void SetActiveGaze(bool value)
		{
			if (inputModule != null) {
				inputModule.enabled = value;
				ActivatePointer (inputModule.enabled ? true : false);
			} else {
				if (value) {
					CreateInputModule ();
				}
			}
		}

		/// <summary>
		/// Active Reticle
		/// </summary>
		/// <param name="value">If set to <c>true</c> value.</param>
		private void ActivatePointer(bool value)
		{
			
		}

		/// <summary>
		/// Create InputModule
		/// </summary>
		private void CreateInputModule()
		{
			if (inputModule == null) {
				eventSystem.SetActive (false);
				inputModule = eventSystem.AddComponent<IVRInputModule> ();
				eventSystem.SetActive (true);
			}
		}

		/// <summary>
		/// Add IVRGraphicRaycaster to Canvas automaticlly
		/// </summary>
		public void AutoGenerateCanvasRaycaster()
		{
			Canvas[] allCanvas = FindObjectsOfType<Canvas> ();
			for (int i = 0; i < allCanvas.Length; i++) {
				if (allCanvas [i] != null) {
					IVRGraphicRaycaster ivrRaycaster = allCanvas [i].GetComponent<IVRGraphicRaycaster> ();
					if (ivrRaycaster == null) {
						ivrRaycaster = allCanvas [i].gameObject.AddComponent<IVRGraphicRaycaster> ();
						ivrRaycaster.blockingObjects = IVRGraphicRaycaster.BlockingObjects.None;
					}
				}
			}
		}

		#region Enable/Disable InputModule
		public void EnableInputModule()
		{
			if (inputModule != null) {
				inputModule.eventSystem.enabled = true;
				inputModule.enabled = true;
			}
		}
		public void DisableInputModule()
		{
			if (inputModule != null) {
				inputModule.eventSystem.enabled = false;
				inputModule.enabled = false;
			}
		}
		#endregion
			
	}


	
}

