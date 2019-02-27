using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/////////////////////////////
/// Description : Show FPS From SVR
/// Author 		: TanSir
/// Date 		: 2019
/// Copyright   : IDEALENS
/////////////////////////////

namespace IDEALENS.IVR.Tool
{
    public class IVRFPSTool : MonoBehaviour
    {


		public Text label_fps;
		public Text _orientationText;
		public Text _positionText;

		private float _framesPerSecond = 0;

		public SvrManager svrManager;
        // Use this for initialization
        void Start()
        {

			svrManager = SvrManager.Instance;
			Debug.Assert(svrManager != null, "SvrManager object not found");
			if (svrManager != null)
			{
				StartCoroutine(CalculateFramesPerSecond());
			}
        }

        // Update is called once per frame
        void Update()
        {
        }

		void LateUpdate()
		{
			if (svrManager == null)
				return;

			var headTransform = svrManager.head;

			//transform.position = headTransform.position;
			//transform.rotation = headTransform.rotation;

			Quaternion orientation = headTransform.localRotation;
			if (_orientationText != null && _orientationText.isActiveAndEnabled)
			{
				_orientationText.text = string.Format("{0:F2}, {1:F2}, {2:F2}, {3:F2}", orientation.x, orientation.y, orientation.z, orientation.w);
				_orientationText.color = (svrManager.status.pose & (int)SvrPlugin.TrackingMode.kTrackingOrientation) == 0 ? Color.red : Color.green;
			}

			Vector3 position = headTransform.localPosition;
			if (_positionText != null && _positionText.isActiveAndEnabled)
			{
				_positionText.text = string.Format("{0:F2}, {1:F2}, {2:F2}", position.x, position.y, position.z);
				_positionText.color = (svrManager.status.pose & (int)SvrPlugin.TrackingMode.kTrackingPosition) == 0 && svrManager.settings.trackPosition && (SvrPlugin.Instance.GetTrackingMode() & (int)SvrPlugin.TrackingMode.kTrackingPosition) != 0 ? Color.red : Color.green;
			}

			if (label_fps != null && label_fps.isActiveAndEnabled)
			{
				int fps = Mathf.RoundToInt(_framesPerSecond);
				int refreshRate = Mathf.RoundToInt(IVRPlugin.GetDeviceInfo_DisplayRefreshRateHz());
				label_fps.text = string.Format("{0} / {1} FPS", fps, refreshRate);
				label_fps.color = fps < refreshRate ? Color.yellow : Color.green;
			}
		}

		private IEnumerator CalculateFramesPerSecond()
		{
			int lastFrameCount = 0;

			while (true)
			{
				yield return new WaitForSecondsRealtime(1.0f);

				var currentFrameCount = svrManager.FrameCount;
				var elapsedFrames = currentFrameCount - lastFrameCount;
				_framesPerSecond = elapsedFrames / 1.0f;
				lastFrameCount = currentFrameCount;
			}
		}

		void OnDestroy()
		{
			StopAllCoroutines ();
		}


    }
}
