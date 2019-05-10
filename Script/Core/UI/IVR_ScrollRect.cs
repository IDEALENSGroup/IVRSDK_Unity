using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IDEALENS.IVR.UI
{
	public class IVR_ScrollRect : ScrollRect {

		/// <summary>
		/// Disable Drag
		/// </summary>
		public bool DisableDragEvent = true;

		/// <summary>
		/// Disable Scroll
		/// </summary>
		public bool DisableScrollEvent = false;

		protected override void OnEnable ()
		{
			base.OnEnable ();
		}

		public override void OnScroll (UnityEngine.EventSystems.PointerEventData data)
		{
			if (DisableScrollEvent)
				return;
			base.OnScroll (data);
		}
		
		public override void OnBeginDrag (UnityEngine.EventSystems.PointerEventData eventData)
		{
			if (DisableDragEvent)
				return;
			base.OnBeginDrag (eventData);
		}

		public override void OnDrag (UnityEngine.EventSystems.PointerEventData eventData)
		{
			if (DisableDragEvent)
				return;
			base.OnDrag (eventData);
		}

		public override void OnEndDrag (UnityEngine.EventSystems.PointerEventData eventData)
		{
			if (DisableDragEvent)
				return;
			base.OnEndDrag (eventData);
		}
	}
}

