using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>Per-camera birth-position history for the BirdEyeRain shader.</summary>
public static class BirdEyeRainCameraHistory
{
    const int SampleCount = 128;
    const double Interval = 1.0 / 30.0;
    static readonly int HistoryId = Shader.PropertyToID("_BirdEyeRainCameraHistory");
    static readonly int NowId = Shader.PropertyToID("_BirdEyeRainCameraNow");
    static readonly int InfoId = Shader.PropertyToID("_BirdEyeRainCameraHistoryInfo");
    static readonly Dictionary<Camera, History> Cameras = new Dictionary<Camera, History>();
    static readonly List<Camera> RemovedCameras = new List<Camera>();
    static int cleanupCounter;

    sealed class History
    {
        public readonly Vector4[] samples = new Vector4[SampleCount];
        public Vector3 previousPosition;
        public double previousTime, sampleTime;

        public void Reset(Vector3 position, double now)
        {
            for (int i = 0; i < SampleCount; i++)
                samples[i] = new Vector4(position.x, position.y, position.z,
                    (float)(now - (SampleCount - 1 - i) * Interval));
            previousPosition = position;
            previousTime = sampleTime = now;
        }

        public void Update(Vector3 position, double now)
        {
            if (now < previousTime || now - previousTime > (SampleCount - 1) * Interval)
            {
                Reset(position, now);
                return;
            }
            while (sampleTime + Interval <= now)
            {
                sampleTime += Interval;
                float blend = (float)((sampleTime - previousTime) / Math.Max(now - previousTime, 0.000001));
                Vector3 interpolated = Vector3.Lerp(previousPosition, position, Mathf.Clamp01(blend));
                Array.Copy(samples, 1, samples, 0, SampleCount - 1);
                samples[SampleCount - 1] = new Vector4(interpolated.x, interpolated.y, interpolated.z, (float)sampleTime);
            }
            previousPosition = position;
            previousTime = now;
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void InitializeRuntime() { Install(); }
#if UNITY_EDITOR
    [InitializeOnLoadMethod]
    static void InitializeEditor() { Install(); }
#endif
    static void Install()
    {
        Cameras.Clear();
        Shader.SetGlobalVector(InfoId, Vector4.zero);
        RenderPipelineManager.beginCameraRendering -= BeforeCamera;
        RenderPipelineManager.beginCameraRendering += BeforeCamera;
    }

    static double CurrentTime()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return EditorApplication.timeSinceStartup;
#endif
        return Time.timeAsDouble;
    }

    static void BeforeCamera(ScriptableRenderContext context, Camera camera)
    {
        if (camera == null) return;
        double now = CurrentTime();
        Vector3 position = camera.transform.position;
        if (!Cameras.TryGetValue(camera, out var history))
        {
            history = new History();
            history.Reset(position, now);
            Cameras.Add(camera, history);
        }
        else history.Update(position, now);
        Shader.SetGlobalVectorArray(HistoryId, history.samples);
        Shader.SetGlobalVector(NowId, new Vector4(position.x, position.y, position.z, (float)now));
        Shader.SetGlobalVector(InfoId, new Vector4(1, (float)Interval, 0, 0));

        // Do not retain cameras destroyed during scene transitions or editor previews.
        if (++cleanupCounter >= 300)
        {
            cleanupCounter = 0;
            RemovedCameras.Clear();
            foreach (var entry in Cameras) if (entry.Key == null) RemovedCameras.Add(entry.Key);
            foreach (var removed in RemovedCameras) Cameras.Remove(removed);
        }
    }
}
