using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SvrDebugOverrideSettings : MonoBehaviour {

	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) { SvrOverrideSettings.EyeAntiAliasing = SvrOverrideSettings.eAntiAliasing.k1; }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { SvrOverrideSettings.EyeAntiAliasing = SvrOverrideSettings.eAntiAliasing.k2; }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { SvrOverrideSettings.EyeAntiAliasing = SvrOverrideSettings.eAntiAliasing.k4; }

        if (Input.GetKeyDown(KeyCode.Q)) { SvrOverrideSettings.EyeResolutionScaleFactor = 0.25f; }
        if (Input.GetKeyDown(KeyCode.W)) { SvrOverrideSettings.EyeResolutionScaleFactor = 0.5f; }
        if (Input.GetKeyDown(KeyCode.E)) { SvrOverrideSettings.EyeResolutionScaleFactor = 0.75f; }
        if (Input.GetKeyDown(KeyCode.R)) { SvrOverrideSettings.EyeResolutionScaleFactor = 1.0f; }
        if (Input.GetKeyDown(KeyCode.T)) { SvrOverrideSettings.EyeResolutionScaleFactor = 1.25f; }
        if (Input.GetKeyDown(KeyCode.Y)) { SvrOverrideSettings.EyeResolutionScaleFactor = 1.5f; }

        if (Input.GetKeyDown(KeyCode.A)) { SvrOverrideSettings.MasterTextureLimit = SvrOverrideSettings.eMasterTextureLimit.k0; }
        if (Input.GetKeyDown(KeyCode.S)) { SvrOverrideSettings.MasterTextureLimit = SvrOverrideSettings.eMasterTextureLimit.k1; }
        if (Input.GetKeyDown(KeyCode.D)) { SvrOverrideSettings.MasterTextureLimit = SvrOverrideSettings.eMasterTextureLimit.k2; }
        if (Input.GetKeyDown(KeyCode.F)) { SvrOverrideSettings.MasterTextureLimit = SvrOverrideSettings.eMasterTextureLimit.k3; }
        if (Input.GetKeyDown(KeyCode.G)) { SvrOverrideSettings.MasterTextureLimit = SvrOverrideSettings.eMasterTextureLimit.k4; }

        if (Input.GetKeyDown(KeyCode.U)) { SvrOverrideSettings.CpuPerfLevel = SvrOverrideSettings.ePerfLevel.Minimum; }
        if (Input.GetKeyDown(KeyCode.I)) { SvrOverrideSettings.CpuPerfLevel = SvrOverrideSettings.ePerfLevel.Medium; }
        if (Input.GetKeyDown(KeyCode.O)) { SvrOverrideSettings.CpuPerfLevel = SvrOverrideSettings.ePerfLevel.Maximum; }
        if (Input.GetKeyDown(KeyCode.P)) { SvrOverrideSettings.CpuPerfLevel = SvrOverrideSettings.ePerfLevel.System; }

        if (Input.GetKeyDown(KeyCode.H)) { SvrOverrideSettings.GpuPerfLevel = SvrOverrideSettings.ePerfLevel.Minimum; }
        if (Input.GetKeyDown(KeyCode.J)) { SvrOverrideSettings.GpuPerfLevel = SvrOverrideSettings.ePerfLevel.Medium; }
        if (Input.GetKeyDown(KeyCode.K)) { SvrOverrideSettings.GpuPerfLevel = SvrOverrideSettings.ePerfLevel.Maximum; }
        if (Input.GetKeyDown(KeyCode.L)) { SvrOverrideSettings.GpuPerfLevel = SvrOverrideSettings.ePerfLevel.System; }

        if (Input.GetKeyDown(KeyCode.Z)) { SvrOverrideSettings.ChromaticAberrationCorrection = SvrOverrideSettings.eChromaticAberrationCorrection.kDisable; }
        if (Input.GetKeyDown(KeyCode.X)) { SvrOverrideSettings.ChromaticAberrationCorrection = SvrOverrideSettings.eChromaticAberrationCorrection.kEnable; }

        if (Input.GetKeyDown(KeyCode.C)) { SvrOverrideSettings.VSyncCount = SvrOverrideSettings.eVSyncCount.k1; }
        if (Input.GetKeyDown(KeyCode.V)) { SvrOverrideSettings.VSyncCount = SvrOverrideSettings.eVSyncCount.k2; }

        if (Input.GetKeyDown(KeyCode.M)) { SvrOverrideSettings.FoveateArea = Mathf.Clamp(SvrOverrideSettings.FoveateArea + 0.1f, 0f, 1f); Debug.LogFormat("FoveateArea: {0}", SvrOverrideSettings.FoveateArea); }
        if (Input.GetKeyDown(KeyCode.N)) { SvrOverrideSettings.FoveateArea = Mathf.Clamp(SvrOverrideSettings.FoveateArea - 0.1f, 0f, 1f); Debug.LogFormat("FoveateArea: {0}", SvrOverrideSettings.FoveateArea); }
        if (Input.GetKeyDown(KeyCode.Period)) { SvrOverrideSettings.FoveateGain = SvrOverrideSettings.FoveateGain + Vector2.one; Debug.LogFormat("FoveateGain: {0}", SvrOverrideSettings.FoveateGain); }
        if (Input.GetKeyDown(KeyCode.Comma)) { SvrOverrideSettings.FoveateGain = SvrOverrideSettings.FoveateGain - Vector2.one; Debug.LogFormat("FoveateGain: {0}", SvrOverrideSettings.FoveateGain); }
    }
}
