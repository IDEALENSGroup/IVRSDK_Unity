using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IDEALENS.IVR
{
	public class SVRService : IIVRService {

		public string serviceName = "SVR Service";

		public string GetServiceName()
		{
			return serviceName;
		}

		public void RecenterTracking ()
		{
			SvrPlugin.Instance.RecenterTracking ();
		}

		public float GetDeviceInfo_DisplayRefreshRateHz()
		{
			return SvrPlugin.Instance.deviceInfo.displayRefreshRateHz;
		}
			
		public void SetVRCameraFadeIn ()
		{
			SvrManager.Instance.SetOverlayFade (SvrManager.eFadeState.FadeIn);
		}

		public void SetVRCameraFadeOut()
		{
			SvrManager.Instance.SetOverlayFade (SvrManager.eFadeState.FadeOut);
		}
	}
}
