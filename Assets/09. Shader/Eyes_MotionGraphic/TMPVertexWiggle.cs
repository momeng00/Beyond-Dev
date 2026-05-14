using UnityEngine;
using TMPro;

[ExecuteAlways]
[RequireComponent(typeof(TMP_Text))]
public class TMPVertexWiggle : MonoBehaviour
{
    [Header("Timing")]
    [Tooltip("1초에 몇 번 랜덤 모양을 갱신할지")]
    [SerializeField] private float updatesPerSecond = 4f;

    [Header("Vertex Wiggle")]
    [Tooltip("각 버텍스가 움직이는 최대 거리")]
    [SerializeField] private float vertexMoveAmount = 1.5f;

    [Tooltip("글자 전체가 같이 움직이는 양. 0이면 글자 형태만 찌그러짐")]
    [SerializeField] private float characterMoveAmount = 0f;

    [Header("Smoothing")]
    [Tooltip("랜덤값이 딱딱 끊기지 않고 보간됨")]
    [SerializeField] private bool smoothTransition = true;

    [Tooltip("랜덤값 사이를 따라가는 속도")]
    [SerializeField] private float smoothSpeed = 16f;

    [Header("Time")]
    [Tooltip("플레이 모드에서 Time.timeScale의 영향을 받지 않음")]
    [SerializeField] private bool useUnscaledTime = true;

    [Header("Editor")]
    [Tooltip("에디터 모드에서도 효과 작동")]
    [SerializeField] private bool animateInEditor = true;

    private TMP_Text tmp;
    private TMP_TextInfo textInfo;

    private Vector3[][] originalVertices;
    private Vector3[][] currentOffsets;
    private Vector3[][] targetOffsets;

    private float timer;
    private double lastEditorTime;

    private int cachedCharacterCount = -1;
    private int cachedMeshCount = -1;

    private void Awake()
    {
        Initialize();
    }

    private void OnEnable()
    {
        Initialize();
        Rebuild();

#if UNITY_EDITOR
        lastEditorTime = UnityEditor.EditorApplication.timeSinceStartup;
        UnityEditor.EditorApplication.update -= EditorUpdate;
        UnityEditor.EditorApplication.update += EditorUpdate;
#endif
    }

    private void OnDisable()
    {
        RestoreOriginalVertices();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.update -= EditorUpdate;
#endif
    }

    private void OnValidate()
    {
        updatesPerSecond = Mathf.Max(0.01f, updatesPerSecond);
        vertexMoveAmount = Mathf.Max(0f, vertexMoveAmount);
        characterMoveAmount = Mathf.Max(0f, characterMoveAmount);
        smoothSpeed = Mathf.Max(0.01f, smoothSpeed);

        Initialize();
        Rebuild();
    }

#if UNITY_EDITOR
    private void EditorUpdate()
    {
        if (Application.isPlaying)
            return;

        if (!animateInEditor)
            return;

        if (!isActiveAndEnabled)
            return;

        double now = UnityEditor.EditorApplication.timeSinceStartup;
        float deltaTime = Mathf.Clamp((float)(now - lastEditorTime), 0f, 0.1f);
        lastEditorTime = now;

        Tick(deltaTime);

        UnityEditor.SceneView.RepaintAll();
    }
#endif

    private void LateUpdate()
    {
        if (!Application.isPlaying)
            return;

        float deltaTime = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        Tick(deltaTime);
    }

    private void Tick(float deltaTime)
    {
        if (tmp == null)
            Initialize();

        if (tmp == null)
            return;

        tmp.ForceMeshUpdate();

        textInfo = tmp.textInfo;

        if (NeedRebuild())
            Rebuild();

        timer += deltaTime;

        float interval = 1f / Mathf.Max(0.01f, updatesPerSecond);

        if (timer >= interval)
        {
            timer %= interval;
            GenerateTargetOffsets();
        }

        ApplyWiggle(deltaTime);
    }

    private void Initialize()
    {
        if (tmp == null)
            tmp = GetComponent<TMP_Text>();
    }

    private bool NeedRebuild()
    {
        if (tmp == null)
            return false;

        if (textInfo == null)
            return true;

        if (originalVertices == null)
            return true;

        if (cachedCharacterCount != textInfo.characterCount)
            return true;

        if (cachedMeshCount != textInfo.meshInfo.Length)
            return true;

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            if (originalVertices[i] == null)
                return true;

            if (originalVertices[i].Length != textInfo.meshInfo[i].vertices.Length)
                return true;
        }

        return false;
    }

    private void Rebuild()
    {
        if (tmp == null)
            return;

        tmp.ForceMeshUpdate();

        textInfo = tmp.textInfo;

        if (textInfo == null)
            return;

        cachedCharacterCount = textInfo.characterCount;
        cachedMeshCount = textInfo.meshInfo.Length;

        originalVertices = new Vector3[cachedMeshCount][];
        currentOffsets = new Vector3[cachedMeshCount][];
        targetOffsets = new Vector3[cachedMeshCount][];

        for (int meshIndex = 0; meshIndex < cachedMeshCount; meshIndex++)
        {
            int vertexCount = textInfo.meshInfo[meshIndex].vertices.Length;

            originalVertices[meshIndex] = new Vector3[vertexCount];
            currentOffsets[meshIndex] = new Vector3[vertexCount];
            targetOffsets[meshIndex] = new Vector3[vertexCount];

            textInfo.meshInfo[meshIndex].vertices.CopyTo(originalVertices[meshIndex], 0);
        }

        GenerateTargetOffsets();

        for (int meshIndex = 0; meshIndex < cachedMeshCount; meshIndex++)
        {
            for (int vertexIndex = 0; vertexIndex < currentOffsets[meshIndex].Length; vertexIndex++)
            {
                currentOffsets[meshIndex][vertexIndex] = targetOffsets[meshIndex][vertexIndex];
            }
        }
    }

    private void GenerateTargetOffsets()
    {
        if (textInfo == null || targetOffsets == null)
            return;

        ClearTargetOffsets();

        for (int charIndex = 0; charIndex < textInfo.characterCount; charIndex++)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[charIndex];

            if (!charInfo.isVisible)
                continue;

            int materialIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            if (materialIndex < 0 || materialIndex >= targetOffsets.Length)
                continue;

            Vector3 characterOffset = RandomVector2(characterMoveAmount);

            for (int i = 0; i < 4; i++)
            {
                int index = vertexIndex + i;

                if (index < 0 || index >= targetOffsets[materialIndex].Length)
                    continue;

                Vector3 vertexOffset = RandomVector2(vertexMoveAmount);
                targetOffsets[materialIndex][index] = characterOffset + vertexOffset;
            }
        }
    }

    private void ClearTargetOffsets()
    {
        for (int meshIndex = 0; meshIndex < targetOffsets.Length; meshIndex++)
        {
            for (int vertexIndex = 0; vertexIndex < targetOffsets[meshIndex].Length; vertexIndex++)
            {
                targetOffsets[meshIndex][vertexIndex] = Vector3.zero;
            }
        }
    }

    private void ApplyWiggle(float deltaTime)
    {
        if (textInfo == null)
            return;

        if (originalVertices == null || currentOffsets == null || targetOffsets == null)
            return;

        for (int meshIndex = 0; meshIndex < textInfo.meshInfo.Length; meshIndex++)
        {
            Vector3[] vertices = textInfo.meshInfo[meshIndex].vertices;

            if (meshIndex >= originalVertices.Length)
                continue;

            for (int vertexIndex = 0; vertexIndex < vertices.Length; vertexIndex++)
            {
                if (vertexIndex >= originalVertices[meshIndex].Length)
                    continue;

                if (smoothTransition)
                {
                    currentOffsets[meshIndex][vertexIndex] = Vector3.Lerp(
                        currentOffsets[meshIndex][vertexIndex],
                        targetOffsets[meshIndex][vertexIndex],
                        1f - Mathf.Exp(-smoothSpeed * deltaTime)
                    );
                }
                else
                {
                    currentOffsets[meshIndex][vertexIndex] = targetOffsets[meshIndex][vertexIndex];
                }

                vertices[vertexIndex] =
                    originalVertices[meshIndex][vertexIndex] +
                    currentOffsets[meshIndex][vertexIndex];
            }
        }

        tmp.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
    }

    private Vector3 RandomVector2(float amount)
    {
        if (amount <= 0f)
            return Vector3.zero;

        return new Vector3(
            Random.Range(-amount, amount),
            Random.Range(-amount, amount),
            0f
        );
    }

    private void RestoreOriginalVertices()
    {
        if (tmp == null || originalVertices == null)
            return;

        tmp.ForceMeshUpdate();
        textInfo = tmp.textInfo;

        if (textInfo == null)
            return;

        for (int meshIndex = 0; meshIndex < textInfo.meshInfo.Length; meshIndex++)
        {
            if (meshIndex >= originalVertices.Length)
                continue;

            Vector3[] vertices = textInfo.meshInfo[meshIndex].vertices;

            int count = Mathf.Min(vertices.Length, originalVertices[meshIndex].Length);

            for (int vertexIndex = 0; vertexIndex < count; vertexIndex++)
            {
                vertices[vertexIndex] = originalVertices[meshIndex][vertexIndex];
            }
        }

        tmp.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
    }
}