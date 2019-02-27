using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class SvrConfigOptions
{
    public static SvrConfigOptions Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new SvrConfigOptions();
            }

            return instance;
        }
    }
    private static SvrConfigOptions instance;
    private static string optionsFileName = "config.txt";

    public bool FreezeAnimations { get; private set; }
    public float FreezeAnimationsAtTimeInSecs { get; private set; }
    public bool UseFixedViewport { get; private set; }
    public Vector3 FixedViewportPosition { get; private set; }
    public Vector3 FixedViewportEulerAnglesRotation { get; private set; }
    public int OverrideRenderTextureMSAA { get; private set; }
    public bool DisableAudio { get; private set; }
    public bool DisableParticles { get; private set; }
    public bool FoveationEnabled { get; private set; }
    public float FoveationArea { get; private set; }
    public Vector2 FoveationGain { get; private set; }
    public float FoveationMinimum { get; private set; }
    public bool FocusEnabled { get; private set; }
    public float FocusSpeed { get; private set; }
    public Vector2 FocusAmplitude { get; private set; }
    public Vector2 FocusFrequency { get; private set; }
    public bool? TrackEyesEnabled { get; private set; }
    public bool? TrackPositionEnabled { get; private set; }
    public bool? GazeReticleEnabled { get; private set; }
    public bool ThermalEnabled { get; private set; }
    public bool ThermalTouchTest { get; private set; }
    public List<string> ThermalStatesDefault { get; private set; }
    public List<string> CpuState0 { get; private set; }
    public List<string> CpuState1 { get; private set; }
    public List<string> CpuState2 { get; private set; }
    public List<string> CpuState3 { get; private set; }
    public List<string> CpuState4 { get; private set; }
    public List<string> GpuState0 { get; private set; }
    public List<string> GpuState1 { get; private set; }
    public List<string> GpuState2 { get; private set; }
    public List<string> GpuState3 { get; private set; }
    public List<string> GpuState4 { get; private set; }
    public List<string> SkinState0 { get; private set; }
    public List<string> SkinState1 { get; private set; }
    public List<string> SkinState2 { get; private set; }
    public List<string> SkinState3 { get; private set; }
    public List<string> SkinState4 { get; private set; }

    private string ConfigFilePath
    {
        get
        {
            string optionsFileFullPath = "";

            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                optionsFileFullPath = Application.dataPath + "/../etc/config";
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                optionsFileFullPath = Application.persistentDataPath;
            }

            optionsFileFullPath += "/" + optionsFileName;

            return optionsFileFullPath;
        }
    }

    private SvrConfigOptions()
    {
		Debug.Log("Looking for config file at: " + ConfigFilePath);
        ParseConfigFile(ConfigFilePath);
    }

    void ParseConfigFile(string path)
    {
        try
        {
            StreamReader sr = File.OpenText(path);
            Debug.Log("Config file found at: " + path);

            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                if (line.Length == 0)
                {
                    continue;
                }

                line = line.Replace(" ", "");
                int commentStartIndex = line.LastIndexOf("//");
                line = commentStartIndex > -1 ? line.Replace(line.Substring(commentStartIndex), "") : line;

                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                string[] tokens = line.Split('=', ',');
                if (tokens.Length < 2)
                {
                    Debug.LogError("line: " + line + "is invalid!");
                }

                switch (tokens[0])
                {
                    case "FreezeAnimations":
                        FreezeAnimations = bool.Parse(tokens[1]);
                        break;
                    case "FreezeAnimationsAtTimeInSecs":
                        FreezeAnimationsAtTimeInSecs = float.Parse(tokens[1]);
                        break;
                    case "DisableParticles":
                        DisableParticles = bool.Parse(tokens[1]);
                        break;
                    case "UseFixedViewport":
                        UseFixedViewport = bool.Parse(tokens[1]);
                        break;
                    case "FixedViewportPosition":
                        FixedViewportPosition = new Vector3( float.Parse(tokens[1]), float.Parse(tokens[2]), float.Parse(tokens[3]) );
                        break;
                    case "FixedViewportEulerAnglesRotation":
                        FixedViewportEulerAnglesRotation = new Vector3(float.Parse(tokens[1]), float.Parse(tokens[2]), float.Parse(tokens[3]));
                        break;
                    case "OverrideRenderTextureMSAA":
                        OverrideRenderTextureMSAA = int.Parse(tokens[1]);
                        break;
                    case "DisableAudio":
                        DisableAudio = bool.Parse(tokens[1]);
                        break;
                    case "FoveationEnabled":
                        FoveationEnabled = bool.Parse(tokens[1]);
                        break;
                    case "FoveationArea":
                        FoveationArea = float.Parse(tokens[1]);
                        break;
                    case "FoveationGain":
                        FoveationGain = new Vector2(float.Parse(tokens[1]), float.Parse(tokens[2]));
                        break;
                    case "FoveationMinimum":
                        FoveationMinimum = float.Parse(tokens[1]);
                        break;
                    case "FocusEnabled":
                        FocusEnabled = bool.Parse(tokens[1]);
                        break;
                    case "FocusSpeed":
                        FocusSpeed = float.Parse(tokens[1]);
                        break;
                    case "FocusAmplitude":
                        FocusAmplitude = new Vector2(float.Parse(tokens[1]), float.Parse(tokens[2]));
                        break;
                    case "FocusFrequency":
                        FocusFrequency = new Vector2(float.Parse(tokens[1]), float.Parse(tokens[2]));
                        break;

                    case "TrackEyesEnabled":
                        TrackEyesEnabled = bool.Parse(tokens[1]);
                        break;
                    case "TrackPositionEnabled":
                        TrackPositionEnabled = bool.Parse(tokens[1]);
                        break;
                    case "GazeReticleEnabled":
                        GazeReticleEnabled = bool.Parse(tokens[1]);
                        break;

                    case "ThermalEnabled":
                        ThermalEnabled = bool.Parse(tokens[1]);
                        break;
                    case "ThermalTouchTest":
                        ThermalTouchTest = bool.Parse(tokens[1]);
                        break;
                    case "ThermalStatesDefault":
                        ThermalStatesDefault = new List<string>();
                        for (int i = 1; i < tokens.Length; i++) ThermalStatesDefault.Add(tokens[i]);
                        break;
                    case "CpuState0":
                        CpuState0 = new List<string>();
                        for (int i = 1; i < tokens.Length; i++) CpuState0.Add(tokens[i]);
                        break;
                    case "CpuState1":
                        CpuState1 = new List<string>();
                        for (int i = 1; i < tokens.Length; i++) CpuState1.Add(tokens[i]);
                        break;
                    case "CpuState2":
                        CpuState2 = new List<string>();
                        for (int i = 1; i < tokens.Length; i++) CpuState2.Add(tokens[i]);
                        break;
                    case "CpuState3":
                        CpuState3 = new List<string>();
                        for (int i = 1; i < tokens.Length; i++) CpuState3.Add(tokens[i]);
                        break;
                    case "CpuState4":
                        CpuState4 = new List<string>();
                        for (int i = 1; i < tokens.Length; i++) CpuState4.Add(tokens[i]);
                        break;

                    case "GpuState0":
                        GpuState0 = new List<string>();
                        for (int i = 1; i < tokens.Length; i++) GpuState0.Add(tokens[i]);
                        break;
                    case "GpuState1":
                        GpuState1 = new List<string>();
                        for (int i = 1; i < tokens.Length; i++) GpuState1.Add(tokens[i]);
                        break;
                    case "GpuState2":
                        GpuState2 = new List<string>();
                        for (int i = 1; i < tokens.Length; i++) GpuState2.Add(tokens[i]);
                        break;
                    case "GpuState3":
                        GpuState3 = new List<string>();
                        for (int i = 1; i < tokens.Length; i++) GpuState3.Add(tokens[i]);
                        break;
                    case "GpuState4":
                        GpuState4 = new List<string>();
                        for (int i = 1; i < tokens.Length; i++) GpuState4.Add(tokens[i]);
                        break;

                    case "SkinState0":
                        SkinState0 = new List<string>();
                        for (int i = 1; i < tokens.Length; i++) SkinState0.Add(tokens[i]);
                        break;
                    case "SkinState1":
                        SkinState1 = new List<string>();
                        for (int i = 1; i < tokens.Length; i++) SkinState1.Add(tokens[i]);
                        break;
                    case "SkinState2":
                        SkinState2 = new List<string>();
                        for (int i = 1; i < tokens.Length; i++) SkinState2.Add(tokens[i]);
                        break;
                    case "SkinState3":
                        SkinState3 = new List<string>();
                        for (int i = 1; i < tokens.Length; i++) SkinState3.Add(tokens[i]);
                        break;
                    case "SkinState4":
                        SkinState4 = new List<string>();
                        for (int i = 1; i < tokens.Length; i++) SkinState4.Add(tokens[i]);
                        break;

                    default:
                        Debug.LogError("Option: " + tokens[0] + " not supported!");
                        break;
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message + "Using default values");
        }
    }

}

