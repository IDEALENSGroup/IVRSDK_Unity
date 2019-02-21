using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/////////////////////////////
/// Description : Example - Handler Test
/// Author 		: TanSir
/// Date 		: 2019
/// Copyright   : IDEALENS
/////////////////////////////
using UnityEngine.UI;
using IDEALENS.IVR.InputPlugin;

namespace IDEALENS.IVR.Example
{

	public class Test_Handler : MonoBehaviour
	{
		public Text text_rotation;
		public GameObject touchPointer;

		// Use this for initialization
		void Start ()
		{
		
		}
	
		// Update is called once per frame
		void Update ()
		{
			Vector2 handTouchpad = IVRInput.Get(IVRInput.Axis2D.PrimaryTouchpad);
			Quaternion handRot = TrackPlugin.GetRemoteRotation();
			string result = "Touchpad:" + handTouchpad.ToString () + "\n";
			result += "Rotation:" + handRot.eulerAngles.ToString () + "\n";
			text_rotation.text = result;

			touchPointer.transform.localPosition = new Vector3 (handTouchpad.x * 140.0f, handTouchpad.y*140.0f, 0);
		}
	}
}
