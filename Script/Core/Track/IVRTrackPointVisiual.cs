using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IDEALENS.IVR.InputPlugin;

/////////////////////////////
/// Description : Pointer Visual 
/// Author 		: TanSir
/// Date 		: 2019
/// Copyright   : IDEALENS
/////////////////////////////

namespace IDEALENS.IVR
{
	public class IVRTrackPointVisiual : MonoBehaviour {
		private MeshRenderer pointerMesh;
		private float startTime = 0;
		// Use this for initialization
		void Start () {
			pointerMesh = GetComponent<MeshRenderer>();
			pointerMesh.material.SetVector("_TintColor", new Vector4(0.5f, 0.5f, 0.5f, 0.5f * 0.8f));
		}
		void OnEnable()
		{
			IVRBridge.mHandRecentPressDown   += IVRSpace_mHandRecentPressDown;
			IVRBridge.mHandRecentPressing    += IVRSpace_mHandRecentPressing;
			IVRBridge.mHandRecentPressUp     += IVRSpace_mHandRecentPressUp;
		}


		private void IVRSpace_mHandRecentPressUp()
		{
			OnPressUp(ControllerButton.CONTROLLER_BUTTON_APP);
		}

		private void IVRSpace_mHandRecentPressing()
		{
			OnPress(ControllerButton.CONTROLLER_BUTTON_APP);
		}

		private void IVRSpace_mHandRecentPressDown()
		{
			OnPressDown(ControllerButton.CONTROLLER_BUTTON_APP);
		}

		// Update is called once per frame
		void Update()
		{
			if (IVRInput.GetControllerWasRecentered())
			{
				pointerMesh.material.SetVector("_TintColor", new Vector4(0.5f, 0.5f, 0.5f, 0.5f * 0.8f));
			}
			if (IVRInput.GetDown(IVRInput.Touch.PrimaryTouchpad)
				|| IVRInput.GetDown(IVRInput.Button.PrimaryIndexTrigger | IVRInput.Button.PrimaryTouchpad | IVRInput.Button.One))
			{
				pointerMesh.material.SetVector("_TintColor", new Vector4(0.5f, 0.5f, 0.5f, 0.5f));
			}

			if (IVRInput.GetUp(IVRInput.Touch.PrimaryTouchpad)
				|| IVRInput.GetUp(IVRInput.Button.PrimaryIndexTrigger | IVRInput.Button.PrimaryTouchpad | IVRInput.Button.One))
			{
				pointerMesh.material.SetVector("_TintColor", new Vector4(0.5f, 0.5f, 0.5f, 0.5f * 0.8f));
			}
		}
		void OnDisable()
		{
			IVRBridge.mHandRecentPressDown   -= IVRSpace_mHandRecentPressDown;
			IVRBridge.mHandRecentPressing    -= IVRSpace_mHandRecentPressing;
			IVRBridge.mHandRecentPressUp     -= IVRSpace_mHandRecentPressUp;
		}

		void OnPressDown(ControllerButton type)
		{
			if ((type & ControllerButton.CONTROLLER_BUTTON_APP) != 0)
			{
				startTime = Time.unscaledTime;
			}
		}

		void OnPress(ControllerButton type)
		{
			if ((type & ControllerButton.CONTROLLER_BUTTON_APP) == 0) return;
			if (startTime == 0) return;
			float time = Time.unscaledTime - startTime - 0.5f;
			/*if (time < 0.5f && time > 0)
			{
				if (pointerMesh.material.mainTexture != pointerLoading)
				{
					pointerMesh.material.mainTexture = pointerLoading;
					//pointer.localEulerAngles = new Vector3(pointer.localEulerAngles.x, 0, 0);
				}

				float radio = Mathf.Lerp(360, 0, time * 2.0f);
				//Debug.Log(radio);
				pointerMesh.material.SetFloat("_FillAmount", radio);
			}
			else if (time > 0.5f)
			{
				pointerMesh.material.SetFloat("_FillAmount", 360);
			}*/
		}

		void OnPressUp(ControllerButton type)
		{
			if ((type & ControllerButton.CONTROLLER_BUTTON_APP) != 0)
			{
				startTime = 0;
			}
			//pointerMesh.material.SetFloat("_FillAmount", 360);

		}
	}
}

