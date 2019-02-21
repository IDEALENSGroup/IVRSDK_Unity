using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using IDEALENS.IVR.InputPlugin;

/////////////////////////////
/// Description : Example - TouchPad Test
/// Author 		: TanSir
/// Date 		: 2019
/// Copyright   : IDEALENS
/////////////////////////////

namespace IDEALENS.IVR.Example
{
	public class Test_Touchpad : MonoBehaviour {

		public Text text_scrollDelta;
		public Text text_scroll;
		public Text text_pressDownTip;
		public Text text_pressUpTip;

		public Text text_singleTapTip;
		public Text text_doubleTapTip;
		public Text text_longPressTapTip;
		public Text text_swipTapTip;
		public Text text_backButtonTip;

        public Button btn_tap;
        public Text text_tapswipTip;

		public GameObject obj_touchPointer;

		// Use this for initialization
		void Start () {

			Init ();

            btn_tap.onClick.AddListener(() =>
            {
                string str = "You tapped me!";
                text_tapswipTip.text = str;
                Debug.Log(str);
            });


            IVRTouchPad.TouchEvent_OnSingleTap += OnTestSingleTap;
			IVRTouchPad.TouchEvent_OnDoubleTap += OnTestDoubleTap;
			IVRTouchPad.TouchEvent_OnSwipe += OnTestSwip;
			IVRTouchPad.TouchEvent_OnLongPress += OnTestLongPress;
			IVRTouchPad.ButtonEvent_OnBackPress += OnBackPress;
			IVRTouchPad.ButtonEvent_OnAppPress += OnAppPress;

			IVRTouchPad.TouchEvent_OnPressDown += OnPressDown;
			IVRTouchPad.TouchEvent_OnPressUp += OnPressUp;

			IVRTouchPad.TouchEvent_OnScroll += OnScroll;
		}

		void OnDestroy()
		{
			IVRTouchPad.TouchEvent_OnSingleTap -= OnTestSingleTap;
			IVRTouchPad.TouchEvent_OnDoubleTap -= OnTestDoubleTap;
			IVRTouchPad.TouchEvent_OnSwipe -= OnTestSwip;
			IVRTouchPad.TouchEvent_OnLongPress -= OnTestLongPress;
			IVRTouchPad.ButtonEvent_OnBackPress -= OnBackPress;
			IVRTouchPad.ButtonEvent_OnAppPress -= OnAppPress;

			IVRTouchPad.TouchEvent_OnPressDown -= OnPressDown;
			IVRTouchPad.TouchEvent_OnPressUp -= OnPressUp;

			IVRTouchPad.TouchEvent_OnScroll -= OnScroll;
		}

        void Update()
        {
			//Vector2 touchPadDelta = IVRInput.GetDelta (IVRInput.Axis2D.PrimaryTouchpad);
			//text_scrollDelta.text = "("+touchPadDelta.x+","+touchPadDelta.y+")";
        }

		void OnScroll(Vector2 touchPos,Vector2 moveDelta)
		{
			text_scrollDelta.text = "TouchPos Delta:" + moveDelta.ToString ("0.000");
			text_scroll.text = "TouchPos:" + touchPos.ToString ();
			obj_touchPointer.transform.localPosition = touchPos * 140.0f;
		}

		void OnPressDown()
		{
			text_pressDownTip.text = "TP PressDown: True";
			text_pressUpTip.text = "TP PressUp: False";
		}
		void OnPressUp()
		{
			text_pressDownTip.text = "TP PressDown: False";
			text_pressUpTip.text = "TP PressUp: True";
			text_backButtonTip.text = "BackButton: False";
		}

		void OnTestSingleTap()
		{
			text_singleTapTip.text = "Single Tap: True";
			Debug.Log ("Single Tap!");

			StopCoroutine ("_resetSingleTapText");
			StartCoroutine (_resetSingleTapText ());
		}

		void OnTestDoubleTap()
		{
			text_doubleTapTip.text = "Double Tap: True";
			Debug.Log ("Double Tap!");

			StopCoroutine ("_resetDoubleTapText");
			StartCoroutine (_resetDoubleTapText ());
		}

		void OnTestSwip(SwipEnum swip)
		{
			string strHead = "Swip Direction: ";
			switch (swip) {
			case SwipEnum.MOVE_BACK:
				text_swipTapTip.text = strHead + "BACK!";
				break;
			case SwipEnum.MOVE_DOWN:
				text_swipTapTip.text =  strHead + "DOWN!";
				break;
			case SwipEnum.MOVE_FOWRAD:
				text_swipTapTip.text =  strHead + "FOWRAD!";
				break;
			case SwipEnum.MOVE_UP:
				text_swipTapTip.text =  strHead + "UP!";
				break;
			}
			Debug.Log (swip);

			StopCoroutine ("_resetSwipText");
			StartCoroutine (_resetSwipText ());
		}

		void OnTestLongPress()
		{
			text_longPressTapTip.text = "LongPress: True";
			Debug.Log ("LongPress");

			StopCoroutine ("_resetLongPressTapText");
			StartCoroutine (_resetLongPressTapText ());
		}

		void OnBackPress()
		{
			text_backButtonTip.text = "BackButton: True";
			Debug.Log ("Back Button Press!");

		}

		void OnAppPress()
		{
			Debug.Log ("App Button Press!");

		}

		void Init()
		{
			text_pressDownTip.text = "TP PressDown: False";
			text_pressUpTip.text = "TP PressUp: False";
			text_singleTapTip.text = "Single Tap: False";
			text_doubleTapTip.text = "Double Tap: False";
			text_swipTapTip.text = "Swip Direction: None";
			text_longPressTapTip.text = "LongPress: False";
			text_backButtonTip.text = "BackButton: False";
		}
			

		#region IEnumerator
		IEnumerator _resetSwipText()
		{
			yield return new WaitForSeconds (1.5f);
			text_swipTapTip.text = "Swip Direction: None";
		}
		IEnumerator _resetSingleTapText()
		{
			yield return new WaitForSeconds (1.5f);
			text_singleTapTip.text = "Single Tap: False";
		}
		IEnumerator _resetDoubleTapText()
		{
			yield return new WaitForSeconds (1.5f);
			text_doubleTapTip.text = "Double Tap: False";
		}
		IEnumerator _resetLongPressTapText()
		{
			yield return new WaitForSeconds (1.5f);
			text_longPressTapTip.text = "LongPress Tap: False";
		}
		#endregion
	}
}

