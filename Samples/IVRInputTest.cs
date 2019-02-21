using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using IDEALENS.IVR.InputPlugin;

namespace IDEALENS.IVR.Example
{
	public class IVRInputTest : MonoBehaviour
	{
		public class BoolMonitor
		{
			public delegate bool BoolGenerator ();

			private string m_name = "";
			private BoolGenerator m_generator;
			private bool m_prevValue = false;
			private bool m_currentValue = false;
			private bool m_currentValueRecentlyChanged = false;
			private float m_displayTimeout = 0.0f;
			private float m_displayTimer = 0.0f;

			public BoolMonitor (string name, BoolGenerator generator, float displayTimeout = 0.5f)
			{
				m_name = name;
				m_generator = generator;
				m_displayTimeout = displayTimeout;
			}

			public void Update ()
			{
				m_prevValue = m_currentValue;
				m_currentValue = m_generator ();

				if (m_currentValue != m_prevValue) {
					m_currentValueRecentlyChanged = true;
					m_displayTimer = m_displayTimeout;
				}

				if (m_displayTimer > 0.0f) {
					m_displayTimer -= Time.deltaTime;

					if (m_displayTimer <= 0.0f) {
						m_currentValueRecentlyChanged = false;
						m_displayTimer = 0.0f;
					}
				}
			}

			public void AppendToStringBuilder (ref StringBuilder sb)
			{
				sb.Append (m_name);

				if (m_currentValue && m_currentValueRecentlyChanged)
					sb.Append (": *True*\n");
				else if (m_currentValue)
					sb.Append (":  True \n");
				else if (!m_currentValue && m_currentValueRecentlyChanged)
					sb.Append (": *False*\n");
				else if (!m_currentValue)
					sb.Append (":  False \n");
			}
		}

		public Text uiText;
		private List<BoolMonitor> monitors;
		private StringBuilder data;

		void Start ()
		{
			if (uiText != null) {
				uiText.supportRichText = false;
			}

			data = new StringBuilder (2048);

			monitors = new List<BoolMonitor> () {
				// virtual
				new BoolMonitor ("One", () => IVRInput.Get (IVRInput.Button.One)),
				new BoolMonitor ("OneDown", () => IVRInput.GetDown (IVRInput.Button.One)),
				new BoolMonitor ("OneUp", () => IVRInput.GetUp (IVRInput.Button.One)),
				new BoolMonitor ("Two", () => IVRInput.Get (IVRInput.Button.Two)),
				new BoolMonitor ("TwoDown", () => IVRInput.GetDown (IVRInput.Button.Two)),
				new BoolMonitor ("TwoUp", () => IVRInput.GetUp (IVRInput.Button.Two)),
				new BoolMonitor ("PrimaryIndexTrigger", () => IVRInput.Get (IVRInput.Button.PrimaryIndexTrigger)),
				new BoolMonitor ("PrimaryIndexTriggerDown", () => IVRInput.GetDown (IVRInput.Button.PrimaryIndexTrigger)),
				new BoolMonitor ("PrimaryIndexTriggerUp", () => IVRInput.GetUp (IVRInput.Button.PrimaryIndexTrigger)),
				new BoolMonitor ("Up", () => IVRInput.Get (IVRInput.Button.Up)),
				new BoolMonitor ("Down", () => IVRInput.Get (IVRInput.Button.Down)),
				new BoolMonitor ("Left", () => IVRInput.Get (IVRInput.Button.Left)),
				new BoolMonitor ("Right", () => IVRInput.Get (IVRInput.Button.Right)),
				new BoolMonitor ("Touchpad (Touch)", () => IVRInput.Get (IVRInput.Touch.PrimaryTouchpad)),
				new BoolMonitor ("TouchpadDown (Touch)", () => IVRInput.GetDown (IVRInput.Touch.PrimaryTouchpad)),
				new BoolMonitor ("TouchpadUp (Touch)", () => IVRInput.GetUp (IVRInput.Touch.PrimaryTouchpad)),
				new BoolMonitor ("TouchpadDown (Click)", () => IVRInput.GetDown (IVRInput.Button.PrimaryTouchpad)),
				new BoolMonitor ("TouchpadUp (Click)", () => IVRInput.GetUp (IVRInput.Button.PrimaryTouchpad)),
				new BoolMonitor ("Touchpad (Click)", () => IVRInput.Get (IVRInput.Button.PrimaryTouchpad)),
				// raw
				new BoolMonitor ("Start", () => IVRInput.Get (IVRInput.RawButton.Start)),
				new BoolMonitor ("StartDown", () => IVRInput.GetDown (IVRInput.RawButton.Start)),
				new BoolMonitor ("StartUp", () => IVRInput.GetUp (IVRInput.RawButton.Start)),
				new BoolMonitor ("Back", () => IVRInput.Get (IVRInput.RawButton.Back)),
				new BoolMonitor ("BackDown", () => IVRInput.GetDown (IVRInput.RawButton.Back)),
				new BoolMonitor ("BackUp", () => IVRInput.GetUp (IVRInput.RawButton.Back)),
				new BoolMonitor ("A", () => IVRInput.Get (IVRInput.RawButton.A)),
				new BoolMonitor ("ADown", () => IVRInput.GetDown (IVRInput.RawButton.A)),
				new BoolMonitor ("AUp", () => IVRInput.GetUp (IVRInput.RawButton.A)),
				new BoolMonitor ("Recented", () => IVRInput.GetControllerWasRecentered ()),
			};
		}

		void Update ()
		{
			IVRInput.Controller activeController = IVRInput.GetActiveController ();

			data.Length = 0;


			string activeControllerName = activeController.ToString ();
			data.AppendFormat ("Active: {0}\n", activeController);

			string connectedControllerNames = IVRInput.GetConnectedControllers ().ToString ();
			data.AppendFormat ("Connected: {0}\n", connectedControllerNames);

			Vector2 primaryTouchpad = IVRInput.Get (IVRInput.Axis2D.PrimaryTouchpad);
			data.AppendFormat ("PrimaryTouchpad: ({0:F3}, {1:F3})\n", primaryTouchpad.x, primaryTouchpad.y);

			Vector2 primaryTouchpadDelta = IVRInput.GetDelta (IVRInput.Axis2D.PrimaryTouchpad);
			data.AppendFormat ("primaryTouchpadDelta: ({0:F3}, {1:F3})\n", primaryTouchpadDelta.x, primaryTouchpadDelta.y);


			bool IsDrag = IVRInput.IsDrag ();
			data.AppendFormat ("IsDrag: {0}\n", IsDrag);

			for (int i = 0; i < monitors.Count; i++) {
				monitors [i].Update ();
				monitors [i].AppendToStringBuilder (ref data);
			}

			if (uiText != null) {
				uiText.text = data.ToString ();
			}

			if (IVRInput.GetDown (IVRInput.Button.One, IVRInput.Controller.Gamepad)) {
				//The Button One 
			}
			if (IVRInput.Get (IVRInput.Button.One)) {
				//The Button One
			}
			if (IVRInput.GetUp (IVRInput.Button.One)) {
				//The Button One 
			}
		}
	}
}

