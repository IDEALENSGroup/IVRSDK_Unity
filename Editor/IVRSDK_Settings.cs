using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace IDEALENS.IVR
{
	/// <summary>
	/// SDK全局设置
	/// tansir
	/// </summary>
	public class IVRSDK_Settings : EditorWindow {


		const string GLOBAL_LOG_OPEN = "Global_Log_Open";
		bool isOpenLog = false;

		[MenuItem("IDEALENS/SDK/Settings")]
		static void Init()
		{

			IVRSDK_Settings window = (IVRSDK_Settings)EditorWindow.GetWindowWithRect<IVRSDK_Settings>(new Rect{ width = 350,height = 300},true,"IDEALENS Settings");
		}
		/*private string GetResourcePath()
		{
			var monos = MonoScript.FromScriptableObject(this);
			var path = AssetDatabase.GetAssetPath(monos);
			return path.Substring(0, path.LastIndexOf("/"));
		}*/

		void OnEnable()
		{
			isOpenLog = EditorPrefs.GetBool (GLOBAL_LOG_OPEN, true);
		}
		void OnGUI()
		{
			//EditorGUILayout.BeginHorizontal();
			/*if (GUILayout.Button("Documentation", GUILayout.Height(30)))
			{
				Application.OpenURL("http://www.idealens.com/cn/developer/k2developerDoc/brief");
			}
			if (GUILayout.Button("Support", GUILayout.Height(30)))
			{
				Application.OpenURL("http://www.idealens.com/cn/developer/k2useHelp/onlineUpdate");
			}*/


			isOpenLog = EditorGUILayout.Toggle ("Open Log", isOpenLog);
			IVRSettings.IsDebugMode = isOpenLog;

			EditorGUILayout.Space ();

			if (GUILayout.Button("Save", GUILayout.Height(30)))
			{
				EditorPrefs.SetBool (GLOBAL_LOG_OPEN, isOpenLog);
			}

		}
	}
}

