using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace IDEALENS.IVR.Editor
{
    class IVRSDKWindow : EditorWindow
    {
        private static Texture2D log;
		[MenuItem("IDEALENS/SDK/Help")]
        static void Init()
        {
            
			IVRSDKWindow window = (IVRSDKWindow)EditorWindow.GetWindowWithRect<IVRSDKWindow>(new Rect{ width = 300,height = 500},true,"IDEALENS");
        }
        private string GetResourcePath()
        {
            var monos = MonoScript.FromScriptableObject(this);
            var path = AssetDatabase.GetAssetPath(monos);
            return path.Substring(0, path.LastIndexOf("/"));
        }
        void OnGUI()
        {
			GUILayout.Space (30);
			log = (Texture2D)EditorGUIUtility.Load(GetResourcePath()+"/Resources/idealens_logo.png");
            GUILayout.Label(log,GUILayout.Height(50));
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            //EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Documentation", GUILayout.Height(30)))
            {
                Application.OpenURL("http://www.idealens.com/cn/developer/k2developerDoc/brief");
            }
            if (GUILayout.Button("Support", GUILayout.Height(30)))
            {
                Application.OpenURL("http://www.idealens.com/cn/developer/k2useHelp/onlineUpdate");
            }
            //EditorGUILayout.EndHorizontal();

            //EditorGUILayout.BeginHorizontal();
            //if (GUILayout.Button("Changelog", GUILayout.Height(30)))
           // {
                //Application.OpenURL("http://simlens.com");
            //}
            if (GUILayout.Button("Check Updates", GUILayout.Height(30)))
            {
				IVRSDK_UpdatePrompt.CheckManually ();
                //Application.OpenURL("http://simlens.com");
            }
            
            //EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("About Idealens", GUILayout.Height(30)))
            {
                Application.OpenURL("http://www.idealens.com");
            }

			GUILayout.Label("IVRSDK v"+IDEALENS.IVR.IVRContext.getVersion(),EditorStyles.boldLabel);
           // EditorGUILayout.EndVertical();
            
        }
    }
}
