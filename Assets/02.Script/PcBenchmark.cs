using System;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>Opt-in player benchmark. Launch with -benchmark; no scene setup required.</summary>
public sealed class PcBenchmark : MonoBehaviour
{
    private const int Capacity = 300000;
    private readonly double[] frames = new double[Capacity];
    private readonly CultureInfo culture = CultureInfo.InvariantCulture;
    private int count, oldVsync, oldLimit, width, height, quality;
    private int captureVsync, captureLimit;
    private FullScreenMode captureScreenMode;
    private float captureTimeScale;
    private double previous, warmUntil, elapsed, nextMemorySample;
    private long peakWorkingSet;
    private bool running, uncapped, changed, lostFocus;
    private string scenario = "normal-play";
    private string status = "Ready. F8: start/stop. F9: show/hide.";
    private string startScene, startSettings, runScenario, outputDirectory;
    private bool visible = true;
    private System.Diagnostics.Process process;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Initialize()
    {
        if (Array.IndexOf(Environment.GetCommandLineArgs(), "-benchmark") < 0)
            return;
        var host = new GameObject("PC Benchmark");
        DontDestroyOnLoad(host);
        host.AddComponent<PcBenchmark>();
    }

    private void Awake()
    {
        outputDirectory = Path.Combine(Application.persistentDataPath, "Benchmarks");
        try { process = System.Diagnostics.Process.GetCurrentProcess(); }
        catch (Exception) { process = null; }
    }

    private void Update()
    {
        bool toggle = false, hide = false;
#if ENABLE_INPUT_SYSTEM
        var keyboard = UnityEngine.InputSystem.Keyboard.current;
        if (keyboard != null)
        {
            toggle = keyboard.f8Key.wasPressedThisFrame;
            hide = keyboard.f9Key.wasPressedThisFrame;
        }
#elif ENABLE_LEGACY_INPUT_MANAGER
        toggle = Input.GetKeyDown(KeyCode.F8);
        hide = Input.GetKeyDown(KeyCode.F9);
#endif
        if (hide) visible = !visible;
        if (toggle)
        {
            if (running) Finish("manual");
            else Begin();
            return;
        }
        if (!running) return;

        double now = Time.realtimeSinceStartupAsDouble;
        if (now < warmUntil) { previous = now; return; }
        // Discard the frame that crosses the warm-up boundary.
        if (previous < warmUntil) { previous = now; return; }
        double ms = (now - previous) * 1000.0;
        previous = now;
        if (ms <= 0) return;
        frames[count++] = ms;
        elapsed += ms / 1000.0;
        changed |= width != Screen.width || height != Screen.height ||
                   quality != QualitySettings.GetQualityLevel() ||
                   captureVsync != QualitySettings.vSyncCount ||
                   captureLimit != Application.targetFrameRate ||
                   captureScreenMode != Screen.fullScreenMode || captureTimeScale != Time.timeScale;
        if (now >= nextMemorySample)
        {
            nextMemorySample = now + 1;
            try
            {
                process?.Refresh();
                if (process != null) peakWorkingSet = Math.Max(peakWorkingSet, process.WorkingSet64);
            }
            catch (Exception) { /* Unavailable memory stays blank in the report. */ }
        }
        if (elapsed >= 120 || count == Capacity)
            Finish(count == Capacity ? "buffer-full" : "completed");
    }

    private string Settings()
    {
        return Screen.width + "x" + Screen.height + "; " + Screen.fullScreenMode +
               "; quality=" + QualitySettings.names[QualitySettings.GetQualityLevel()] +
               "; vsync=" + QualitySettings.vSyncCount + "; fpsLimit=" + Application.targetFrameRate +
               "; timeScale=" + Time.timeScale.ToString(culture);
    }

    private void Begin()
    {
        oldVsync = QualitySettings.vSyncCount;
        oldLimit = Application.targetFrameRate;
        if (uncapped)
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = -1;
        }
        count = 0;
        elapsed = 0;
        peakWorkingSet = 0;
        changed = false;
        lostFocus = !Application.isFocused;
        width = Screen.width;
        height = Screen.height;
        quality = QualitySettings.GetQualityLevel();
        captureVsync = QualitySettings.vSyncCount;
        captureLimit = Application.targetFrameRate;
        captureScreenMode = Screen.fullScreenMode;
        captureTimeScale = Time.timeScale;
        startSettings = Settings();
        startScene = SceneManager.GetActiveScene().name;
        runScenario = scenario;
        previous = Time.realtimeSinceStartupAsDouble;
        warmUntil = previous + 5;
        nextMemorySample = warmUntil;
        running = true;
        status = "5s warm-up, then 120s recording. Repeat the same route.";
    }

    private void OnApplicationFocus(bool focused)
    {
        if (running && !focused) lostFocus = true;
    }

    private static string Csv(string value) => "\"" + (value ?? "").Replace("\"", "\"\"") + "\"";
    private string Number(double value) => value.ToString("0.###", culture);

    private void Finish(string reason)
    {
        if (!running) return;
        running = false;
        string endScene = SceneManager.GetActiveScene().name;
        if (uncapped)
        {
            QualitySettings.vSyncCount = oldVsync;
            Application.targetFrameRate = oldLimit;
        }
        if (count == 0) { status = "No measured frames. Start again."; return; }
        try
        {
            Directory.CreateDirectory(outputDirectory);
            string id = DateTime.Now.ToString("yyyyMMdd-HHmmss-fff", culture);
            string rawPath = Path.Combine(outputDirectory, id + "-frames.csv");
            using (var writer = new StreamWriter(rawPath, false, new UTF8Encoding(true)))
            {
                writer.WriteLine("frame,frame_time_ms");
                for (int i = 0; i < count; i++)
                    writer.WriteLine(i.ToString(culture) + "," + frames[i].ToString("F6", culture));
            }
            // Sort only after recording; no sorting or disk writes in measured frames.
            Array.Sort(frames, 0, count);
            int slowCount = Math.Max(1, (int)Math.Ceiling(count * 0.01));
            double slowSum = 0;
            int over60 = 0, over30 = 0;
            for (int i = 0; i < count; i++)
            {
                if (i >= count - slowCount) slowSum += frames[i];
                if (frames[i] > 1000.0 / 60) over60++;
                if (frames[i] > 1000.0 / 30) over30++;
            }
            double average = count / elapsed;
            double low = 1000.0 / (slowSum / slowCount);
            double p99 = frames[Math.Max(0, (int)Math.Ceiling(count * 0.99) - 1)];
            string summaryPath = Path.Combine(outputDirectory, "results.csv");
            bool header = !File.Exists(summaryPath);
            using (var writer = new StreamWriter(summaryPath, true, new UTF8Encoding(true)))
            {
                if (header)
                    writer.WriteLine("run,scenario,start_scene,end_scene,cpu,logical_cores,gpu,ram_mb,vram_capacity_mb,os,unity,app_version,development,editor,graphics_api,settings,seconds,frames,avg_fps,one_percent_low_fps,p99_ms,max_ms,over_16_667ms_percent,over_33_333ms_percent,peak_process_working_set_mb,settings_changed,focus_lost,result");
                writer.WriteLine(string.Join(",", new[] {
                    Csv(id), Csv(runScenario), Csv(startScene), Csv(endScene),
                    Csv(SystemInfo.processorType), SystemInfo.processorCount.ToString(culture),
                    Csv(SystemInfo.graphicsDeviceName), SystemInfo.systemMemorySize.ToString(culture),
                    SystemInfo.graphicsMemorySize.ToString(culture), Csv(SystemInfo.operatingSystem),
                    Csv(Application.unityVersion), Csv(Application.version), Debug.isDebugBuild.ToString(),
                    Application.isEditor.ToString(), Csv(SystemInfo.graphicsDeviceType.ToString()),
                    Csv(startSettings), Number(elapsed), count.ToString(culture), Number(average),
                    Number(low), Number(p99), Number(frames[count - 1]),
                    Number(100.0 * over60 / count), Number(100.0 * over30 / count),
                    peakWorkingSet > 0 ? Number(peakWorkingSet / 1048576.0) : "",
                    changed.ToString(), lostFocus.ToString(), Csv(reason)
                }));
            }
            status = "Saved: avg " + Number(average) + " FPS, 1% low " + Number(low) +
                     ", P99 " + Number(p99) + " ms" +
                     (changed || lostFocus ? "\nWARNING: settings changed or focus lost. Repeat this run." : "");
#if UNITY_EDITOR
            Debug.Log("Benchmark saved: " + summaryPath);
#endif
        }
        catch (Exception error)
        {
            status = "Save failed: " + error.Message;
            Debug.LogException(error);
        }
    }

    private void OnGUI()
    {
        if (!visible) return;
        GUILayout.BeginArea(new Rect(10, 10, 530, 290), GUI.skin.box);
        GUILayout.Label("PC BENCHMARK | F8 start/stop | F9 hide");
        GUILayout.Label("Current resolution: " + Screen.width + " x " + Screen.height +
                        " | " + Screen.fullScreenMode);
        GUI.enabled = !running;
        if (GUILayout.Button("1920 x 1080 (Fullscreen)"))
        {
            Screen.SetResolution(1920, 1080, FullScreenMode.ExclusiveFullScreen);
            status = "Resolution requested. Check Current resolution before starting.";
        }
        scenario = GUILayout.TextField(scenario, 80);
        uncapped = GUILayout.Toggle(uncapped, "Temporarily disable VSync / FPS limit");
        GUI.enabled = true;
        if (GUILayout.Button(running ? "Stop and save" : "Start: 5s warm-up + 120s capture"))
        {
            if (running) Finish("manual");
            else Begin();
        }
        GUILayout.Label(running ? "Recording: " + elapsed.ToString("F0", culture) + " / 120s" : status);
        GUILayout.Label(outputDirectory);
        if (!running && GUILayout.Button("Open results folder"))
        {
            Directory.CreateDirectory(outputDirectory);
            Application.OpenURL(new Uri(outputDirectory + Path.DirectorySeparatorChar).AbsoluteUri);
        }
        GUILayout.EndArea();
    }

    private void OnApplicationQuit() => Finish("quit");
    private void OnDestroy()
    {
        Finish("destroyed");
        process?.Dispose();
    }
}
