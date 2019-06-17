using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using IDEALENS.IVR.EventSystems;
using UnityEngine.UI;

/////////////////////////////
/// Description : Example - Gaze object and trigger event test
/// Author 		: TanSir
/// Date 		: 2019
/// Copyright   : IDEALENS
/////////////////////////////
using IDEALENS.IVR.InputPlugin;

namespace IDEALENS.IVR.Example
{
	public class Test_GazeObject : MonoBehaviour,
                                                IPointerHoverHandler,
                                                IPointerClickHandler,
                                                IPointerEnterHandler,
                                                IPointerExitHandler,
        
												IBeginDragHandler,
												IDragHandler,
												IEndDragHandler{


		private Vector3 rot = new Vector3(0,3,0);

		IEnumerator Rotation()
		{
			while (true) {
				transform.Rotate (rot);
				yield return null;
			}
		}

		/// <summary>
		/// Pointer Hover
		/// </summary>
		public void OnPointerHover(PointerEventData eventData)
		{
			//Debug.Log (gameObject.name + " OnPointerHover");
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.dragging) {
				return;
			}
			rot = new Vector3 (rot.z, rot.x, rot.y);
			Debug.Log (gameObject.name + " OnPointerClick");
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			StartCoroutine ("Rotation");
			Debug.Log (gameObject.name + " OnPointerEnter");
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			StopCoroutine ("Rotation");
			Debug.Log (gameObject.name + " OnPointerExit");
		}

        #region Drag
		public void OnEndDrag(PointerEventData eventData)
        {
            gameObject.transform.parent = null;
            Debug.Log(gameObject.name + " OnPointerDragEnd");
        }

		public void OnBeginDrag(PointerEventData eventData)
        {
			if (TrackPlugin.isConnected) {
				gameObject.transform.parent = IVRManager.Instance.hand;
			} else {
				gameObject.transform.parent = IVRManager.Instance.head;
			}

            Debug.Log(gameObject.name + " OnPointerDragBegin");
        }

		public void OnDrag(PointerEventData eventData)
        {
            Debug.Log(gameObject.name + " OnPointerDragging");
        }
        #endregion
    }
}

