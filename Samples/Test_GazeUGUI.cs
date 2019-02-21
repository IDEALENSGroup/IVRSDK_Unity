using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/////////////////////////////
/// Description : Example - UGUI Test
/// Author 		: TanSir
/// Date 		: 2019
/// Copyright   : IDEALENS
/////////////////////////////

namespace IDEALENS.IVR.Example
{
	public class Test_GazeUGUI : MonoBehaviour {

		public Text tip_button;
		public Text tip_slider;
		public Text tip_toggle;
		public Text tip_dropdown;

		int btnClickCount = 0;
		public Button btn;
		public Slider slider;
		public Toggle toggle;
		public Dropdown dropdown;

		// Use this for initialization
		void Start () {
			btnClickCount = 0;

			tip_button.text = "Click Count : "+btnClickCount;
			tip_slider.text = "Progress: 0";
			tip_toggle.text = "Open";
			tip_dropdown.text = "You choose: " + dropdown.options[0].text;


			btn.onClick.AddListener (() => {
				btnClickCount++;
				tip_button.text = "Click Count : "+btnClickCount;
			});

			slider.onValueChanged.AddListener ((float progress) => {
				tip_slider.text = "Progress: "+ progress;
			});

			toggle.onValueChanged.AddListener ((bool value) => {
				tip_toggle.text = value ? "Open" : "Close";
			});

			dropdown.onValueChanged.AddListener ((int index) => {
				tip_dropdown.text = "You choose: " + dropdown.options[index].text;
			});
				
			IVRTouchPad.TouchEvent_OnScroll += OnScroll;
			slider.interactable = false;
		}

		void OnDestroy()
		{
			IVRTouchPad.TouchEvent_OnScroll -= OnScroll;
		}
			
		void OnScroll(Vector2 pos,Vector2 delta)
		{
			slider.value +=delta.x;
		}
	}
}

