using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace IDEALENS.IVR.Editor
{
	/// <summary>
	/// IVR manager editor.
	/// tansir
	/// </summary>
	[CustomEditor (typeof(IVRManager)),CanEditMultipleObjects]
	public class IVRManagerEditor : UnityEditor.Editor
	{
		private int bannerHeightMax = 100;

		Texture logo;

		SerializedProperty script;
		SerializedProperty HideCameraRig;
		SerializedProperty VRPlatform;
		SerializedProperty VRCameraEyeMode;
		SerializedProperty dontDestroy;
		SerializedProperty antiAliasingQualcomm;
		//SerializedProperty enableOverlayMode;
		SerializedProperty enableResetPosition;

		//SerializedProperty eyeBufferType;
		//SerializedProperty cpuPerfLevel;
		//SerializedProperty gpuPerfLevel;

		SerializedProperty head;
		SerializedProperty gaze;
		SerializedProperty monoCamera;
		SerializedProperty leftCamera;
		SerializedProperty rightCamera;
		SerializedProperty leftOverlay;
		SerializedProperty rightOverlay;
		SerializedProperty monoOverlay;
		SerializedProperty fadeOverlay;
		//SerializedProperty reticleOverlay;

		SerializedProperty hand;


		protected bool _isExpandBasicSettings;
		protected bool _isExpandCameraRig;
		protected bool _isExpandHandController;


		void OnEnable ()
		{
			var resourcePath = GetResourcePath ();
			#if UNITY_5_0
			logo = Resources.LoadAssetAtPath<Texture2D>(GetResourcePath()+"/Resources/idealens_logo.png");
			#else
			logo = AssetDatabase.LoadAssetAtPath<Texture2D> (GetResourcePath () + "/Resources/idealens_logo.png");
			#endif

			script = serializedObject.FindProperty ("m_Script");

			HideCameraRig = serializedObject.FindProperty ("HideCameraRig");
			VRPlatform = serializedObject.FindProperty ("VRPlatform");
			VRCameraEyeMode = serializedObject.FindProperty ("VRCameraEyeMode");
			dontDestroy = serializedObject.FindProperty ("dontDestroy");

			antiAliasingQualcomm = serializedObject.FindProperty ("antiAliasingQualcomm");
			//enableOverlayMode = serializedObject.FindProperty ("enableOverlayMode");
			enableResetPosition = serializedObject.FindProperty ("enableResetPosition");

			//eyeBufferType = serializedObject.FindProperty ("eyeBufferType");
			//cpuPerfLevel = serializedObject.FindProperty ("cpuPerfLevel");
			//gpuPerfLevel = serializedObject.FindProperty ("gpuPerfLevel");

			head = serializedObject.FindProperty ("head");
			gaze = serializedObject.FindProperty ("gaze");
			monoCamera = serializedObject.FindProperty ("monoCamera");
			leftCamera = serializedObject.FindProperty ("leftCamera");
			rightCamera = serializedObject.FindProperty ("rightCamera");
			leftOverlay = serializedObject.FindProperty ("leftOverlay");
			rightOverlay = serializedObject.FindProperty ("rightOverlay");
			monoOverlay = serializedObject.FindProperty ("monoOverlay");
			fadeOverlay = serializedObject.FindProperty ("fadeOverlay");
			//reticleOverlay = serializedObject.FindProperty ("reticleOverlay");

			hand = serializedObject.FindProperty ("hand");
		}

	
		private string GetResourcePath ()
		{
			var monos = MonoScript.FromScriptableObject (this);
			var path = AssetDatabase.GetAssetPath (monos);
			return path.Substring (0, path.LastIndexOf ("/"));
		}

		public override void OnInspectorGUI ()
		{
			serializedObject.Update ();

			if (logo) {
				// Logo need have aspect rate 2:1
				int bannerWidth, bannerHeight;
				bannerWidth = Screen.width - 35;
				bannerHeight = (int)(bannerWidth / (float)2);
				if (bannerHeight > bannerHeightMax) {
					bannerHeight = bannerHeightMax;
					bannerWidth = bannerHeight * 2;
				}
				var rect = GUILayoutUtility.GetRect (bannerWidth, bannerHeight, GUI.skin.box);
				GUI.DrawTexture (rect, logo, ScaleMode.ScaleToFit);
			}

			IVRManager myScript = target as IVRManager;

			GUILayout.BeginHorizontal ();
			GUILayout.Space (12);
			_isExpandBasicSettings = EditorGUILayout.Foldout (_isExpandBasicSettings, "Basic Settings", true);
			GUILayout.EndHorizontal ();
			if (_isExpandBasicSettings) {
				GUILayout.BeginVertical ("HelpBox");
				//EditorGUILayout.PropertyField (VRCameraEyeMode);
				//EditorGUILayout.PropertyField (enableOverlayMode,new GUIContent("Overlay Mode"));
				EditorGUILayout.PropertyField (enableResetPosition, new GUIContent ("Recenter Tracking"));
				EditorGUILayout.PropertyField (dontDestroy, new GUIContent ("Do not Destroy Me"));

				EditorGUILayout.PropertyField (antiAliasingQualcomm, new GUIContent ("AntiAliasing Quality"));
				GUILayout.EndVertical ();
			}


			GUILayout.BeginHorizontal ();
			GUILayout.Space (12);
			_isExpandCameraRig = EditorGUILayout.Foldout (_isExpandCameraRig, "CameraRig", true);
			GUILayout.EndHorizontal ();
			if (_isExpandCameraRig) {
				GUILayout.BeginVertical ("HelpBox");
				EditorGUILayout.PropertyField (head);
				EditorGUILayout.PropertyField (gaze);
				EditorGUILayout.PropertyField (monoCamera);
				EditorGUILayout.PropertyField (leftCamera);
				EditorGUILayout.PropertyField (rightCamera);
				EditorGUILayout.PropertyField (leftOverlay);
				EditorGUILayout.PropertyField (rightOverlay);
				EditorGUILayout.PropertyField (monoOverlay);
				EditorGUILayout.PropertyField (fadeOverlay);
				//EditorGUILayout.PropertyField (reticleOverlay);
				GUILayout.EndVertical ();
			}

			GUILayout.BeginHorizontal ();
			GUILayout.Space (12);
			_isExpandHandController = EditorGUILayout.Foldout (_isExpandHandController, "HandController", true);
			GUILayout.EndHorizontal ();
			if (_isExpandHandController) {
				GUILayout.BeginVertical ("HelpBox");
				EditorGUILayout.PropertyField (hand);
				GUILayout.EndVertical ();
			}

			//}



			SvrManager svrManager = myScript.GetComponent<SvrManager> ();
			if (svrManager == null) {
				myScript.gameObject.AddComponent<SvrManager> ();
			}

			check ();

			serializedObject.ApplyModifiedProperties ();

		}

		private void check ()
		{
			IVRManager myScript = target as IVRManager;
			SvrManager svrManager = myScript.GetComponent<SvrManager> ();
			if (svrManager == null) {
				myScript.gameObject.AddComponent<SvrManager> ();
			}
		}
	}
}
