using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using IDEALENS.IVR.EventSystems;

/////////////////////////////
/// Description : 锚点对Dropdown控件的处理
/// Author 		: TanSir
/// Date 		: 2019
/// Copyright   : IDEALENS
/////////////////////////////

namespace IDEALENS.IVR.UI
{
	public class IVR_UIDropdown : Dropdown
	{
		private GameObject currentBlocker;

		public override void OnPointerClick (PointerEventData eventData)
		{
			base.OnPointerClick (eventData);
			FixTemplateAndBlockerRaycasters ();
		}

		public override void OnSubmit (BaseEventData eventData)
		{
			base.OnSubmit (eventData);
			FixTemplateAndBlockerRaycasters ();
		}

		private void FixTemplateAndBlockerRaycasters ()
		{
			if (template != null) {
				FixRaycaster (template.gameObject, false);
			}
			FixRaycaster (currentBlocker, true);
		}

		protected override GameObject CreateBlocker (Canvas rootCanvas)
		{
			currentBlocker = base.CreateBlocker (rootCanvas);
			return currentBlocker;
		}

		protected override GameObject CreateDropdownList (GameObject template)
		{
			GameObject dropdown = base.CreateDropdownList (template);
			FixRaycaster (dropdown, false);
			return dropdown;
		}

		private void FixRaycaster (GameObject go, bool shouldCopyProperties)
		{
			if (go == null) {
				return;
			}

			GraphicRaycaster oldRaycaster = go.GetComponent<GraphicRaycaster> ();
			Destroy (oldRaycaster);

			bool addedRaycaster;
			IVRGraphicRaycaster raycaster;
			raycaster = GetOrAddComponent<IVRGraphicRaycaster> (go, out addedRaycaster);

			if (shouldCopyProperties) {
				IVRGraphicRaycaster templateRaycaster = GetTemplateRaycaster ();
				if (addedRaycaster && templateRaycaster != null) {
					CopyRaycasterProperties (templateRaycaster, raycaster);
				}
			}
		}

		private IVRGraphicRaycaster GetTemplateRaycaster ()
		{
			if (template == null) {
				return null;
			}

			return template.GetComponent<IVRGraphicRaycaster> ();
		}

		private void CopyRaycasterProperties (IVRGraphicRaycaster source, IVRGraphicRaycaster dest)
		{
			if (source == null || dest == null) {
				return;
			}

			dest.blockingMask = source.blockingMask;
			dest.blockingObjects = source.blockingObjects;
			dest.ignoreReversedGraphics = source.ignoreReversedGraphics;
		}

		private static T GetOrAddComponent<T> (GameObject go, out bool addedComponent) where T : Component
		{
			T comp = go.GetComponent<T> ();
			addedComponent = false;
			if (!comp) {
				comp = go.AddComponent<T> ();
				addedComponent = true;
			}
			return comp;
		}
	}

}
