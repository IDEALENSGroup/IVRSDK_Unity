using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IDEALENS.IVR.InputPlugin;

/////////////////////////////
/// Description : Head
/// Author 		: TanSir
/// Date 		: 2019
/// Copyright   : IDEALENS
/////////////////////////////

namespace IDEALENS.IVR
{
	public class IVRTrackHead : IVRTrackObject
	{

		protected override void OnDataUpdate (Vector3 position, Quaternion rotation)
		{
			transform.localPosition = position;
			transform.localRotation = rotation;
		}
	}
}
