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


        private Text label;

		private float _framesPerSecond = 0;

		public SvrManager svrManager;
        // Use this for initialization
        void Start()
        {
            label = GetComponent<Text>();
            if (label == null)
            {
                label = gameObject.AddComponent<Text>();
            }

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
			if (label != null && label.isActiveAndEnabled)
			{
				int fps = Mathf.RoundToInt(_framesPerSecond);
				int refreshRate = Mathf.RoundToInt(IVRManager.Instance.GetDeviceInfo_DisplayRefreshRateHz());
				label.text = string.Format("{0} / {1} FPS", fps, refreshRate);
				label.color = fps < refreshRate ? Color.yellow : Color.green;
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
