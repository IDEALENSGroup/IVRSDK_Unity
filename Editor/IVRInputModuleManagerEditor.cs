using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using IDEALENS.IVR.EventSystems;

namespace IDEALENS.IVR.Editor
{
	/// <summary>
	/// IVR manager editor.
	/// tansir
	/// </summary>
	[CustomEditor(typeof(IVRInputModuleManager)),CanEditMultipleObjects]
	public class IVRInputModuleManagerEditor : UnityEditor.Editor
	{
		private int bannerHeightMax = 100;

		Texture logo;

		//SerializedProperty script;
		SerializedProperty AutoGenerateEventSystem;
		SerializedProperty DoNotDestroyed;
		//SerializedProperty EnableInputModule;
		SerializedProperty AutoGenerateCanvasRaycaster;
		SerializedProperty HandlerButton2CommonClick;
		SerializedProperty EnableGazeTriggerMode;


		protected bool _isExpandBasicSettings;
		protected bool _isExpandTouchpad;

		void OnEnable()
		{
			var resourcePath = GetResourcePath ();
			#if UNITY_5_0
			logo = Resources.LoadAssetAtPath<Texture2D>(GetResourcePath()+"/Resources/idealens_logo.png");
			#else
			logo = AssetDatabase.LoadAssetAtPath<Texture2D>(GetResourcePath()+"/Resources/idealens_logo.png");
			#endif

			//script = serializedObject.FindProperty("m_Script");

			AutoGenerateEventSystem = serializedObject.FindProperty("bAutoGenerateEventSystem");
			DoNotDestroyed = serializedObject.FindProperty("DoNotDestroyed");
			//EnableInputModule = serializedObject.FindProperty("EnableInputModule");
			AutoGenerateCanvasRaycaster = serializedObject.FindProperty("bAutoGenerateCanvasRaycaster");
			HandlerButton2CommonClick = serializedObject.FindProperty("handlerButton2CommonClick");
			EnableGazeTriggerMode = serializedObject.FindProperty("_enableGazeTriggerMode"); 

		}


		private string GetResourcePath()
		{
			var monos = MonoScript.FromScriptableObject(this);
			var path = AssetDatabase.GetAssetPath(monos);
			return path.Substring(0, path.LastIndexOf("/"));
		}

		public override void OnInspectorGUI ()
		{
			serializedObject.Update ();

			if (logo)
			{
				// Logo need have aspect rate 2:1
				int bannerWidth, bannerHeight;
				bannerWidth = Screen.width - 35;
				bannerHeight = (int) (bannerWidth / (float) 2);
				if (bannerHeight > bannerHeightMax)
				{
					bannerHeight = bannerHeightMax;
					bannerWidth = bannerHeight * 2;
				}
				var rect = GUILayoutUtility.GetRect(bannerWidth, bannerHeight, GUI.skin.box);
				GUI.DrawTexture(rect, logo, ScaleMode.ScaleToFit);
			}

			IVRInputModuleManager myScript = target as IVRInputModuleManager;

			//EditorGUILayout.PropertyField(script);
			GUILayout.BeginVertical();

			#region basic settings
			GUILayout.BeginHorizontal ();
			GUILayout.Space (12);
			_isExpandBasicSettings = EditorGUILayout.Foldout (_isExpandBasicSettings, "Basic Settings", true);
			GUILayout.EndHorizontal ();
			if (_isExpandBasicSettings) {
				GUILayout.BeginVertical ("HelpBox");

				EditorGUILayout.PropertyField (AutoGenerateEventSystem,new GUIContent("Generate EventSystem"));
				EditorGUILayout.PropertyField (EnableGazeTriggerMode,new GUIContent("Gaze Auto Trigger Mode"));
				//EditorGUILayout.PropertyField (EnableInputModule,new GUIContent("Enable InputModule"));
				EditorGUILayout.PropertyField (DoNotDestroyed,new GUIContent("Do not Destroy Me"));
				GUILayout.EndVertical ();
			}
			#endregion

			GUILayout.Space (10);

			#region Touchpad
			//EditorGUILayout.PropertyField (AutoGenerateCanvasRaycaster,new GUIContent("Generate Raycaster to Canvas"));
			GUILayout.BeginHorizontal ();
			GUILayout.Space (12);
			_isExpandTouchpad = EditorGUILayout.Foldout (_isExpandTouchpad, "Touchpad Settings", true);
			GUILayout.EndHorizontal ();
			if (_isExpandTouchpad) {
				GUILayout.BeginVertical ("HelpBox");
				EditorGUILayout.PropertyField (HandlerButton2CommonClick,new GUIContent("Default Trigger Button"));
				GUILayout.EndVertical ();
			}
			#endregion

			GUILayout.EndVertical();





			serializedObject.ApplyModifiedProperties ();

		}
	}
}
