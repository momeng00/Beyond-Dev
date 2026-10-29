using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(RotateAnimation))]
public sealed class RotateAnimationEditor : Editor
{
    RotateAnimation component;
    AnimationClip clip;
    Quaternion initialLocal, initialWorld;
    bool previewing, playing;
    float elapsed;
    double lastTime;

    void OnEnable()
    {
        component = (RotateAnimation)target;
        EditorApplication.update += Tick;
        EditorApplication.playModeStateChanged += OnPlayMode;
        AssemblyReloadEvents.beforeAssemblyReload += StopPreview;
        EditorSceneManager.sceneSaving += OnSceneSaving;
        Undo.undoRedoPerformed += StopPreview;
    }
    void OnDisable()
    {
        StopPreview();
        EditorApplication.update -= Tick;
        EditorApplication.playModeStateChanged -= OnPlayMode;
        AssemblyReloadEvents.beforeAssemblyReload -= StopPreview;
        EditorSceneManager.sceneSaving -= OnSceneSaving;
        Undo.undoRedoPerformed -= StopPreview;
    }
    void OnPlayMode(PlayModeStateChange state) => StopPreview();
    void OnSceneSaving(Scene scene, string path) => StopPreview();

    bool BeginPreview()
    {
        if (previewing) return true;
        // Do not take ownership of another inspector's animation preview.
        if (AnimationMode.InAnimationMode()) return false;
        initialLocal = component.transform.localRotation;
        initialWorld = component.transform.rotation;
        clip = new AnimationClip { hideFlags = HideFlags.HideAndDontSave };
        AnimationMode.StartAnimationMode();
        previewing = true;
        lastTime = EditorApplication.timeSinceStartup;
        return true;
    }
    void StopPreview()
    {
        if (previewing) AnimationMode.StopAnimationMode();
        previewing = playing = false;
        elapsed = 0;
        if (clip != null) DestroyImmediate(clip);
        SceneView.RepaintAll();
        Repaint();
    }
    void Sample()
    {
        if (!BeginPreview()) return;
        Quaternion q = component.EvaluateLocalRotation(initialLocal, initialWorld, elapsed);
        clip.SetCurve("", typeof(Transform), "m_LocalRotation.x", AnimationCurve.Constant(0, 1, q.x));
        clip.SetCurve("", typeof(Transform), "m_LocalRotation.y", AnimationCurve.Constant(0, 1, q.y));
        clip.SetCurve("", typeof(Transform), "m_LocalRotation.z", AnimationCurve.Constant(0, 1, q.z));
        clip.SetCurve("", typeof(Transform), "m_LocalRotation.w", AnimationCurve.Constant(0, 1, q.w));
        AnimationMode.BeginSampling();
        try { AnimationMode.SampleAnimationClip(component.gameObject, clip, 0); }
        finally { AnimationMode.EndSampling(); }
        SceneView.RepaintAll();
    }
    void Tick()
    {
        double now = EditorApplication.timeSinceStartup;
        float delta = (float)(now - lastTime);
        lastTime = now;
        if (!previewing) return;
        if (component == null || !component.isActiveAndEnabled ||
            EditorApplication.isPlayingOrWillChangePlaymode || !AnimationMode.InAnimationMode())
        { StopPreview(); return; }
        if (!playing) return;
        elapsed += delta;
        if (component.Repeat == RotateAnimation.RepeatMode.Once && elapsed >= component.Duration)
        { elapsed = component.Duration; playing = false; }
        Sample();
        Repaint();
    }
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        DrawDefaultInspector();
        if (EditorGUI.EndChangeCheck() && previewing) Sample();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Animation Preview", EditorStyles.boldLabel);
        bool blocked = !previewing && AnimationMode.InAnimationMode();
        if (blocked) EditorGUILayout.HelpBox("Close the other animation preview first.", MessageType.Info);
        using (new EditorGUI.DisabledScope(EditorApplication.isPlayingOrWillChangePlaymode ||
            !component.isActiveAndEnabled || blocked))
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button(playing ? "Pause" : "Play"))
                {
                    if (playing) playing = false;
                    else if (BeginPreview())
                    {
                        if (component.Repeat == RotateAnimation.RepeatMode.Once && elapsed >= component.Duration) elapsed = 0;
                        lastTime = EditorApplication.timeSinceStartup;
                        playing = true;
                        Sample();
                    }
                }
                if (GUILayout.Button("Reset")) StopPreview();
            }
            EditorGUI.BeginChangeCheck();
            float progress = EditorGUILayout.Slider("Normalized Progress", component.ProgressAt(elapsed), 0, 1);
            if (EditorGUI.EndChangeCheck())
            {
                playing = false;
                // For looping clips 1 is the start of the next cycle.
                elapsed = progress * component.Duration;
                Sample();
            }
            EditorGUI.BeginChangeCheck();
            float seconds = EditorGUILayout.FloatField("Elapsed Time", elapsed);
            if (EditorGUI.EndChangeCheck())
            {
                playing = false;
                elapsed = Mathf.Max(0, seconds);
                if (component.Repeat == RotateAnimation.RepeatMode.Once) elapsed = Mathf.Min(elapsed, component.Duration);
                Sample();
            }
        }
    }
}
