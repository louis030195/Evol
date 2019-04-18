using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.Analytics;

public class Builds
{
    static string[] levels = new[]
    {
        "Assets/Scenes/Login.unity",
        "Assets/Scenes/Game.unity"
    };

    
    [MenuItem("Builds/Windows")]
    public static void BuildWindows()
    {
        PlayerSettings.runInBackground = false;
        var message = BuildPipeline.BuildPlayer(
            levels,
            $"../Build/Windows/Evol.exe",
            BuildTarget.StandaloneWindows64,
            BuildOptions.ShowBuiltPlayer);

        if (message)
            Debug.Log($"Windows build complete");
        else
            Debug.LogError($"Error building Windows { message }");
    }
    
    [MenuItem("Builds/Linux")]
    public static void BuildLinux()
    {
        PlayerSettings.runInBackground = false;
        var message = BuildPipeline.BuildPlayer(
            levels,
            $"../Build/Linux/Evol.x86_64",
            BuildTarget.StandaloneLinux64,
            BuildOptions.ShowBuiltPlayer);

        if (message)
            Debug.Log($"Linux build complete");
        else
            Debug.LogError($"Error building Linux { message }");
    }
    
    [MenuItem("Builds/Web")]
    public static void BuildWeb()
    {
        PlayerSettings.runInBackground = false;
        var message = BuildPipeline.BuildPlayer(
            levels,
            $"../Build/Web/",
            BuildTarget.WebGL,
            BuildOptions.ShowBuiltPlayer);

        if (message)
            Debug.Log($"WebGL build complete");
        else
            Debug.LogError($"Error building WebGL { message }");
    }
    
    // Seems to be runnable from bash
    // "C:\Program Files\Unity\Editor\Unity.exe -quit -batchmode -executeMethod BuildWin64NoDRMWorldwide"
    [MenuItem("Builds/PC All Platforms")]
    public static void BuildAllPc() {
        BuildWindows();
        BuildLinux();
    }
}
