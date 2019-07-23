using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using IDEALENS.IVR.InputPlugin;
using System;

/////////////////////////////
/// Description : Example - API TEST
/// Author 		: TanSir
/// Date 		: 2019
/// Copyright   : IDEALENS
/////////////////////////////
/// 
namespace IDEALENS.IVR.Example
{
	public class IVRAPITest : MonoBehaviour
	{
		public Button btn_openVideo;
		public Button btn_openImage;

		void Start ()
		{
			// InnerCard = 0,
			// ExtCard = 1,
			// OTG = 2
			// Provide IVRPlugin.GetStoragePathByType method to get storage root path,

			btn_openVideo.onClick.AddListener (() => {
				if(IVRContext.CurrentVersion >= IVRPlugin.CreateVersion(1,1,0))
				{
					

					//can also pass "/storage/emulated/0/xxx.mp4" as the parameter
					// Notice: default set 2D screen mode and one-shot play,when video play finished, video player will be quit.
					IVRPlugin.OpenCinema(IVRPlugin.GetStoragePathByType(StorageType.InnerCard)+"/test.mp4");
				}
			});

			btn_openImage.onClick.AddListener (() => {
				if(IVRContext.CurrentVersion >= IVRPlugin.CreateVersion(1,1,0))
				{
					//can also pass "/storage/emulated/0/xxx.jpg" as the parameter
					// Notice: default set 2D screen mode
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

