using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class PcBenchmarkBuild
{
    [MenuItem("Tools/PC Benchmark/Build Windows Test Player")]
    public static void Build()
    {
        string[] scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled)
            .Select(scene => scene.path).ToArray();
        if (scenes.Length == 0)
            throw new InvalidOperationException("Enable the desired scenes in the Build Profiles Scene List first.");
        string folder = Path.GetFullPath("Builds/PCBenchmark");
        Directory.CreateDirectory(folder);
        var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = Path.Combine(folder, "BeyondBenchmark.exe"),
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        });
        if (report.summary.result != BuildResult.Succeeded)
            throw new InvalidOperationException("Benchmark build failed: " + report.summary.result);
        File.WriteAllText(Path.Combine(folder, "StartBenchmark.cmd"),
            "@echo off\r\ncd /d \"%~dp0\"\r\nstart \"\" \"BeyondBenchmark.exe\" -benchmark\r\n");
        Debug.Log("Benchmark ready: " + folder + "/StartBenchmark.cmd");
        if (!Application.isBatchMode) EditorUtility.RevealInFinder(Path.Combine(folder, "StartBenchmark.cmd"));
    }
}
