using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IDEALENS.IVR.InputPlugin;

/////////////////////////////
/// Description : Handler Visual
/// Author 		: TanSir
/// Date 		: 2019
/// Copyright   : IDEALENS
/////////////////////////////

namespace IDEALENS.IVR
{
	public class IVRRemotVisual : MonoBehaviour
	{
		public enum DeviceType
		{
			H1000 = IVRInput.Controller.Remote_H1000,
			Ximmerse = IVRInput.Controller.Remote_Ximmerse
		}
		[System.Serializable]
		public struct ControllerDisplayState
		{
			public bool clickButton;
			public bool appButton;
			public bool TriggerButton;
			public bool homeButton;
			public bool volumeupButton;
			public bool volumedownButton;
			public bool touching;
			public Vector2 touchPos;
		}
		public DeviceType devices = DeviceType.H1000;
		public ControllerDisplayState displayState;
		[SerializeField]
		private Color normalColor = new Color(200f / 255f, 200f / 255f, 200f / 255f, 1);

		[SerializeField]
		private Color clickColor = Color.red;

		[SerializeField]
		private Color touchpadcolor = new Color(200f / 255f, 200f / 255f, 200f / 255f, 1);
		private Vector3 H1000_CLICK_ANGLE = new Vector3(-210.3f,0,0);
		private Vector3 H1000_NOMAL_ANGLE = new Vector3(-180.0f,0,0);
		private Vector3 XIMMERSE_CLICK_ANGLE = new Vector3(0,0,11.0f);
		private readonly float H1000_TOUCHPAD_RADIUS = 0.3027f;
		private readonly Vector2 H1000__TOUCHPAD_CENTER = new Vector2(0.441f,0.5f);
		private readonly float XIMMERSE_TOUCHPAD_RADIUS = 0.1279f;
		private readonly Vector2 XIMMERSE__TOUCHPAD_CENTER = new Vector2(0.8027f, 0.7061f);
		private Vector3 XIMMERSE_NOMAL_ANGLE = Vector3.zero;
		private Vector3 TriggerClickAngle;
		private Vector3 TriggerNomalAngle;
		public Transform TriggerRotationPoint;
		public Renderer TriggerRender;
		public Renderer AppRender;
		public Renderer HomeRender;
		public Renderer TouchRender;
		public Renderer VolumeUpRender;
		public Renderer VolumeDownRender;
		private MaterialPropertyBlock TriggerBlock;
		private MaterialPropertyBlock AppBlock;
		private MaterialPropertyBlock HomeBlock;
		private MaterialPropertyBlock TouchBlock;
		private MaterialPropertyBlock VolumeUpBlock;
		private MaterialPropertyBlock VolumeDownBlock;

		private int ShaderColor;
		private int Shader_ToucInfo;
		private int Shader_TouchPadColor;
		private int Shader_ShaerDate;
		private Vector4 ShaderDate = Vector4.zero;
		void Start()
		{

			Initialize();
		}

		// Update is called once per frame
		void Update()
		{
			displayState.homeButton = IVRInput.Get(IVRInput.Button.Two);
			displayState.appButton = IVRInput.Get(IVRInput.Button.Back);
			displayState.clickButton = IVRInput.Get(IVRInput.Button.PrimaryTouchpad);
			displayState.TriggerButton = IVRInput.Get(IVRInput.Button.PrimaryIndexTrigger);
			displayState.touching = !displayState.clickButton && IVRInput.Get(IVRInput.Touch.PrimaryTouchpad);
			if (displayState.touching)
				displayState.touchPos = IVRInput.Get(IVRInput.Axis2D.PrimaryTouchpad);

			ControllerState_3DOF Rawstate = TrackPlugin.idealinputState;
			displayState.volumeupButton = (((uint)Rawstate.wButtons & (uint)ControllerButton.CONTROLLER_BUTTON_VOL_UP) != 0);
			displayState.volumedownButton = (((uint)Rawstate.wButtons & (uint)ControllerButton.CONTROLLER_BUTTON_VOL_DOWN) != 0);
			UpdateState();
		}

		private void OnValidate()
		{
			if (!Application.isPlaying)
			{
				Initialize();
				UpdateState();
			}
		}

		private void Initialize()
		{
			switch (devices)
			{
			case DeviceType.H1000:
				TriggerClickAngle = H1000_CLICK_ANGLE;
				TriggerNomalAngle = H1000_NOMAL_ANGLE;
				break;
			case DeviceType.Ximmerse:
				TriggerClickAngle = XIMMERSE_CLICK_ANGLE;
				TriggerNomalAngle = XIMMERSE_NOMAL_ANGLE;
				break;
			default:
				break;
			}
			if (TriggerBlock == null)
				TriggerBlock = new MaterialPropertyBlock();
			if (AppBlock == null)
				AppBlock = new MaterialPropertyBlock();
			if (HomeBlock == null)
				HomeBlock = new MaterialPropertyBlock();
			if (TouchBlock == null)
				TouchBlock = new MaterialPropertyBlock();
			if (VolumeUpBlock == null)
				VolumeUpBlock = new MaterialPropertyBlock();
			if (VolumeDownBlock == null)
				VolumeDownBlock = new MaterialPropertyBlock();

			ShaderColor = Shader.PropertyToID("_Color");
			Shader_ToucInfo = Shader.PropertyToID("_ToucInfo");
			Shader_ShaerDate = Shader.PropertyToID("_ShaerDate");
			Shader_TouchPadColor = Shader.PropertyToID("_TouchPadColor");

			switch (devices)
			{
			case DeviceType.H1000:
				ShaderDate.x = H1000__TOUCHPAD_CENTER.x;
				ShaderDate.y = H1000__TOUCHPAD_CENTER.y;
				ShaderDate.z = H1000_TOUCHPAD_RADIUS;
				break;
			case DeviceType.Ximmerse:
				ShaderDate.x = XIMMERSE__TOUCHPAD_CENTER.x;
				ShaderDate.y = XIMMERSE__TOUCHPAD_CENTER.y;
				ShaderDate.z = XIMMERSE_TOUCHPAD_RADIUS;
				break;
			default:
				break;
			}
		}
		private void UpdateState()
		{
			if (TouchRender)
			{
				//TouchBlock.SetFloat(Shader_isTouching, displayState.touching ? 1 : 0);
				ShaderDate.w = displayState.touching ? 1 : 0;
				TouchBlock.SetVector(Shader_ShaerDate, ShaderDate);
				TouchBlock.SetColor(Shader_TouchPadColor, touchpadcolor);
				TouchBlock.SetVector(Shader_ToucInfo, new Vector4(displayState.touchPos.x, displayState.touchPos.y, 0, 0));
				UpdateColor(displayState.clickButton, TouchBlock, TouchRender);
			}
			if (TriggerRender)
			{
				if (TriggerRotationPoint)
				{
					Vector3 rotation = displayState.TriggerButton ? TriggerClickAngle : TriggerNomalAngle;
					TriggerRotationPoint.localRotation = Quaternion.Euler(rotation);
				}
				UpdateColor(displayState.TriggerButton, TriggerBlock, TriggerRender);
			}
			if (AppRender)
				UpdateColor(displayState.appButton, AppBlock, AppRender);
			if (HomeRender)
				UpdateColor(displayState.homeButton, HomeBlock, HomeRender);
			if (VolumeUpRender)
				UpdateColor(displayState.volumeupButton, VolumeUpBlock, VolumeUpRender);
			if (VolumeDownRender)
				UpdateColor(displayState.volumedownButton, VolumeDownBlock, VolumeDownRender);
		}
		private void UpdateColor(bool button, MaterialPropertyBlock block,Renderer renderer)
		{
			if (button)
			{
				block.SetColor(ShaderColor, clickColor);
			}
			else
			{
				block.SetColor(ShaderColor, normalColor);
			}
			renderer.SetPropertyBlock(block);
		}
	}

}
