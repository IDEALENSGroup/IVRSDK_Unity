using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using IDEALENS.IVR.InputPlugin;

namespace IDEALENS.IVR.Example
{
	public class Test_BackToMenu : MonoBehaviour
	{
		public string backSceneName;
		// Use this for initialization
		void Start ()
		{
		}

		void OnDestroy()
		{
		}

		void Update()
		{
			if (IVRInput.GetDown (IVRInput.Button.Back)) {
				OnBackCallback ();
			}
		}

		void OnBackCallback()
		{
			IVRManager.Instance.SetFadeOutAction (() => {
				SceneManager.LoadScene (backSceneName);
			});

		}

	
	}
}
