using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

/////////////////////////////
/// Description : SDK定义
/// Author 		: TanSir
/// Date 		: 2019
/// Copyright   : IDEALENS
/////////////////////////////

namespace IDEALENS.IVR
{
    public static class IVRSDK_Defines
    {
        /// <summary>
        /// The scripting define symbol that is used for the current version of IVRSDK.
        /// </summary>
        public static string CurrentExactVersionScriptingDefineSymbol { get; private set; }

        public const string VersionScriptingDefineSymbolPrefix = "VRTK_VERSION_";
        public const string VersionScriptingDefineSymbolSuffix = "_OR_NEWER";

		static IVRSDK_Defines()
        {
			CurrentExactVersionScriptingDefineSymbol = ExactVersionSymbol(IVRContext.CurrentVersion);
        }

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
        private static void EnsureVersionSymbolIsSet()
        {
			IEnumerable<string> atLeastVersionSymbols = new[] { IVRContext.CurrentVersion }
				.Concat(IVRContext.PreviousVersions)
                .Select(AtLeastVersionSymbol);
            string[] versionSymbols = new[] { CurrentExactVersionScriptingDefineSymbol }
                .Concat(atLeastVersionSymbols)
                .ToArray();

            foreach (BuildTargetGroup targetGroup in GetValidBuildTargetGroups())
            {
                string[] currentSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup)
                                                        .Split(';')
                                                        .Distinct()
                                                        .OrderBy(symbol => symbol, StringComparer.Ordinal)
                                                        .ToArray();
                string[] newSymbols = currentSymbols.Where(symbol => !symbol.StartsWith(VersionScriptingDefineSymbolPrefix, StringComparison.Ordinal))
                                                    .Concat(versionSymbols)
                                                    .ToArray();

                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, string.Join(";", newSymbols));
            }
        }
#endif

        private static string ExactVersionSymbol(Version version)
        {
            return string.Format("{0}{1}", VersionScriptingDefineSymbolPrefix, version.ToString().Replace(".", "_"));
        }

        private static string AtLeastVersionSymbol(Version version)
        {
            return string.Format("{0}{1}", ExactVersionSymbol(version), VersionScriptingDefineSymbolSuffix);
        }

		#if UNITY_EDITOR
		public static BuildTargetGroup[] GetValidBuildTargetGroups()
		{
			return Enum.GetValues(typeof(BuildTargetGroup)).Cast<BuildTargetGroup>().Where(group =>
				{
					if (group == BuildTargetGroup.Unknown)
					{
						return false;
					}

					string targetGroupName = Enum.GetName(typeof(BuildTargetGroup), group);
					FieldInfo targetGroupFieldInfo = typeof(BuildTargetGroup).GetField(targetGroupName, BindingFlags.Public | BindingFlags.Static);

					return targetGroupFieldInfo != null && targetGroupFieldInfo.GetCustomAttributes(typeof(ObsoleteAttribute), false).Length == 0;
				}).ToArray();
		}
		#endif
    }
}
