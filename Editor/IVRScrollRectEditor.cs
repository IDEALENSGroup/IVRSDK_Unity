using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UI;
using IDEALENS.IVR.UI;
using UnityEditor;

[CustomEditor(typeof(IVR_ScrollRect),false)]
public class IVRScrollRectEditor : ScrollRectEditor {

	SerializedProperty b_disableDrag;
	SerializedProperty b_disableScroll;

	protected override void OnEnable()
	{
		base.OnEnable ();
		b_disableDrag = serializedObject.FindProperty ("DisableDragEvent");
		b_disableScroll = serializedObject.FindProperty ("DisableScrollEvent");
	}

	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
		serializedObject.Update ();
		EditorGUILayout.PropertyField (b_disableDrag);
		EditorGUILayout.PropertyField (b_disableScroll);
		serializedObject.ApplyModifiedProperties ();
	}
}
