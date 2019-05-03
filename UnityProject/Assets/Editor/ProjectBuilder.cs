using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.Analytics;

public class Builds
{
    static string[] gameLevels = new[]
    {
        "Assets/Scenes/Login.unity",
        "Assets/Scenes/Game.unity"
    };
    
    static string[] mlLevels = new[]
    {
        "Assets/Scenes/machine_learning.unity"
    };

    
    [MenuItem("Builds/Game/Windows")]
    public static void BuildWindows()
    {
        PlayerSettings.runInBackground = false;
        var message = BuildPipeline.BuildPlayer(
            gameLevels,
            $"../Build/Game/Windows/Evol.exe",
            BuildTarget.StandaloneWindows64,
            BuildOptions.ShowBuiltPlayer);

        if (message)
            Debug.Log($"Windows build complete");
        else
            Debug.LogError($"Error building Windows { message }");
    }
    
    [MenuItem("Builds/Game/Linux")]
    public static void BuildLinux()
    {
        PlayerSettings.runInBackground = false;
        var message = BuildPipeline.BuildPlayer(
            gameLevels,
            $"../Build/Game/Linux/Evol.x86_64",
            BuildTarget.StandaloneLinux64,
            BuildOptions.ShowBuiltPlayer);

        if (message)
            Debug.Log($"Linux build complete");
        else
            Debug.LogError($"Error building Linux { message }");
    }
    
    [MenuItem("Builds/Game/Web")]
    public static void BuildWeb()
    {
        PlayerSettings.runInBackground = false;
        var message = BuildPipeline.BuildPlayer(
            gameLevels,
            $"../Build/Game/Web/",
            BuildTarget.WebGL,
            BuildOptions.ShowBuiltPlayer);

        if (message)
            Debug.Log($"WebGL build complete");
        else
            Debug.LogError($"Error building WebGL { message }");
    }
    
    // Seems to be runnable from bash
    // "C:\Program Files\Unity\Editor\Unity.exe -quit -batchmode -executeMethod BuildWin64NoDRMWorldwide"
    [MenuItem("Builds/Game/PC All Platforms")]
    public static void BuildAllPc() {
        BuildWindows();
        BuildLinux();
    }
    
    [MenuItem("Builds/ML/LinuxHeadless")]
    public static void BuildMachineLearningLinuxHeadless()
    {
        PlayerSettings.runInBackground = false;
        var message = BuildPipeline.BuildPlayer(
            mlLevels,
            $"../Build/Linux/ML/Headless/Evol.x86_64",
            BuildTarget.StandaloneLinux64,
        BuildOptions.ShowBuiltPlayer | BuildOptions.EnableHeadlessMode);

        if (message)
            Debug.Log($"Linux headless build complete");
        else
            Debug.LogError($"Error building Linux { message }");
    }
}
