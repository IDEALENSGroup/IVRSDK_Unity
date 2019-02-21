using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using IDEALENS.IVR.InputPlugin;
using IDEALENS.IVR.EventSystems;

/////////////////////////////
/// Description : Example - Main Menu
/// Author 		: TanSir
/// Date 		: 2019
/// Copyright   : IDEALENS
/////////////////////////////

namespace IDEALENS.IVR.Example
{
	public class Test_DemoMenu : MonoBehaviour {
		
		public static Test_DemoMenu Instance;
		public Text text_tip;
		public Text text_version;

		void OnEnable()
		{
			Instance = this;
			text_version.text = "Version:"+IVRContext.getVersion ();
		}


		// Use this for initialization
		void Start () {
			IVRTouchPad.ButtonEvent_OnBackPress += OnBack;

		}

		void OnDestroy()
		{
			IVRTouchPad.ButtonEvent_OnBackPress -= OnBack;
			Instance = null;
		}

		void Update()
		{
			/*
			if (IVRInput.GetDown (IVRInput.Button.PrimaryTouchpad)) {
				Debug.Log ("Key Down........");
			}

			if (IVRInput.Get (IVRInput.Button.PrimaryTouchpad)) {
				Debug.Log ("Key........");
			}

			if (IVRInput.GetUp (IVRInput.Button.PrimaryTouchpad)) {
				Debug.Log ("Key Up........");
			}*/
			if (Input.GetKeyDown (KeyCode.Q)) {
				IVRInputModuleManager.Instance.EnableInputModule ();
			}
			if (Input.GetKeyDown (KeyCode.W)) {
				IVRInputModuleManager.Instance.DisableInputModule ();
			}
		}

		void OnBack()
		{
            Application.Quit();
		}

		public void ClickToScene(Test_ButtonAndTip buttonTip)
		{
			UnityEngine.SceneManagement.SceneManager.LoadScene (buttonTip.SceneName);
		}

		public void ShowTip(Test_ButtonAndTip buttonTip)
		{
			text_tip.text = buttonTip.Description;
		}

		public void ClearTip()
		{
			text_tip.text = "Please select example to show sdk features";
		}

		public void ShowAndroidSystemToast()
		{
			IVRManager.ShowToast ("ShowToast");
		}

		public void ShowAndroidSystemDialog()
		{
			IVRManager.ShowDialog ("Android Dialog","Msg Here","Sure",()=>{
				Debug.Log("Show Dialog");
			});
		}
			
	}
}

