using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using IDEALENS.IVR.InputPlugin;
using System;

namespace IDEALENS.IVR.Example
{
	public class IVRAPITest : MonoBehaviour
	{
		public Button btn_openVideo;
		public Button btn_openImage;

		void Start ()
		{
			btn_openVideo.onClick.AddListener (() => {
				if(IVRContext.CurrentVersion >= IVRPlugin.CreateVersion(1,1,0))
				{
					IVRPlugin.OpenCinema(IVRPlugin.GetStoragePathByType(StorageType.InnerCard)+"/test.mp4");
				}
			});

			btn_openImage.onClick.AddListener (() => {
				if(IVRContext.CurrentVersion >= IVRPlugin.CreateVersion(1,1,0))
				{
					IVRPlugin.OpenImageViewer(IVRPlugin.GetStoragePathByType(StorageType.InnerCard)+"/test.jpg");
				}
			});
		}

		void OnDestroy()
		{
			btn_openVideo.onClick.RemoveAllListeners ();
			btn_openImage.onClick.RemoveAllListeners ();
		}
	}
}

