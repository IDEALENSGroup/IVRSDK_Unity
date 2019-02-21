using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/////////////////////////////
/// Description : Set Layer
/// Author 		: TanSir
/// Date 		: 2019
/// Copyright   : IDEALENS
/////////////////////////////

namespace IDEALENS.IVR.Utility
{
	public class IVRObjectLayerSet : MonoBehaviour {

		public bool includeChildren = false;

		public string layerName;

		//[SerializeField]
		//protected LayerMask layerMask;

		// Use this for initialization
		void Start () {
			
			if (includeChildren) {
				Transform[] transformAllObjects = transform.GetComponentsInChildren<Transform> ();
				for (int i = 0; i < transformAllObjects.Length; i++) {
					transformAllObjects [i].gameObject.layer = LayerMask.NameToLayer (layerName);
				}
			} else {
				transform.gameObject.layer = LayerMask.NameToLayer (layerName);
			}

		}

	}
}

