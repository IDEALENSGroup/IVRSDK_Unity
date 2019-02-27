using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace IDEALENS.IVR.Editor
{
	public class IVRSDK_Build : MonoBehaviour
	{
		static void BuildScene(string[] scenes, string apkDir, string apkName)
		{
			Directory.CreateDirectory(apkDir);

			if(System.IO.File.Exists(apkDir + apkName))
			{
				System.IO.File.SetAttributes(apkDir + apkName, System.IO.File.GetAttributes(apkDir + apkName) & ~FileAttributes.ReadOnly);
			}

			BuildPipeline.BuildPlayer(scenes, apkDir + apkName, BuildTarget.Android, BuildOptions.None);
		}
		//
		//[MenuItem("SVR/Build Project/Landscape Left")]
		static void BuildProjectLL()
		{
			BuildProjectTarget(UIOrientation.LandscapeLeft, "-ll.apk");
		}

		//[MenuItem("SVR/Build Project/Landscape Right")]
		static void BuildProjectLR()
		{
			BuildProjectTarget(UIOrientation.LandscapeRight, "-lr.apk");
		}

		//[MenuItem("SVR/Build Project/Landscape All")]
		static void BuildProject()
		{
			BuildProjectTarget(UIOrientation.LandscapeLeft, "-ll.apk");
			BuildProjectTarget(UIOrientation.LandscapeRight, "-lr.apk");
		}

		static void BuildProjectTarget(UIOrientation landscapeOrientation, string landscapeSuffix)
		{
			try
			{

				#if UNITY_2018
				PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
				PlayerSettings.Android.targetArchitectures = AndroidArchitecture.All;   // ARM64
				PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
				#elif UNITY_2017
				PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel25;
				#else // UNITY_5
				# endif
				Debug.Log("Bulding Project!");
				#if UNITY_5
				EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.Android);
				#else
				EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
				#endif

				{
					string apkDir = "./Build/Android/";
					string apkName = PlayerSettings.productName.Replace(" ", "_");   

					List<string> scenes = new List<string>();
					if( EditorBuildSettings.scenes != null )
					{
						for(int i=0;i<EditorBuildSettings.scenes.Length;i++)
						{
							if( EditorBuildSettings.scenes[i].enabled )
							{
								scenes.Add(EditorBuildSettings.scenes[i].path);
							}
						}
					}

					//Save off current orientation
					UIOrientation currentOrientation = PlayerSettings.defaultInterfaceOrientation;

					//Build target
					PlayerSettings.defaultInterfaceOrientation = landscapeOrientation;
					BuildScene(scenes.ToArray(), apkDir, apkName+landscapeSuffix);

					//revert back
					PlayerSettings.defaultInterfaceOrientation = currentOrientation;
				}
			}
			catch (IOException e)
			{
				Debug.LogError( e.Message );
			}
		}
	}
}

