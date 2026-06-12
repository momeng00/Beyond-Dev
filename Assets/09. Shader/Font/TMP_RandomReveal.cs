using System.Text;
using TMPro;
using UnityEngine;

[ExecuteAlways]
public class TMP_RandomReveal : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private TMP_Text targetText;

    [Header("Text")]
    [TextArea]
    [SerializeField] private string originalText = "START TEXT";

    [TextArea]
    [SerializeField] private string finalText = "FINAL TEXT";

    [Header("Reveal")]
    [Range(0f, 1f)]
    [SerializeField] private float progress = 0f;

    [Header("Random Characters")]
    [SerializeField] private string randomChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789@#$%&!?";

    [Header("Options")]
    [SerializeField] private bool preserveSpaces = true;
    [SerializeField] private float randomChangeInterval = 0.04f;

    [Header("Ease")]
    [SerializeField] private bool useEaseIn = true;

    [Header("Editor")]
    [SerializeField] private bool previewInEditor = false;

    private readonly StringBuilder builder = new StringBuilder();
    private float timer;

    public float Progress
    {
        get => progress;
        set
        {
            progress = Mathf.Clamp01(value);
            UpdateText();
        }
    }

    public string OriginalText
    {
        get => originalText;
        set
        {
            originalText = value;
            UpdateText();
        }
    }

    public string FinalText
    {
        get => finalText;
        set
        {
            finalText = value;
            UpdateText();
        }
    }

    private void Reset()
    {
        targetText = GetComponent<TMP_Text>();

        if (targetText != null)
        {
            originalText = targetText.text;
            finalText = targetText.text;
        }
    }

    private void OnEnable()
    {
        if (targetText == null)
            targetText = GetComponent<TMP_Text>();

        if (Application.isPlaying || previewInEditor)
            UpdateText();
    }

    private void Update()
    {
        if (!Application.isPlaying && !previewInEditor)
            return;

        timer += Application.isPlaying ? Time.deltaTime : 0.016f;

        if (timer >= randomChangeInterval)
        {
            timer = 0f;
            UpdateText();
        }
    }

    private void OnValidate()
    {
        if (targetText == null)
            targetText = GetComponent<TMP_Text>();

        progress = Mathf.Clamp01(progress);

        if (previewInEditor)
            UpdateText();
    }

    public void Play()
    {
        Progress = 0f;
        UpdateText();
    }

    public void SetOriginalText(string text)
    {
        originalText = text;
        UpdateText();
    }

    public void SetFinalText(string text)
    {
        finalText = text;
        UpdateText();
    }

    private void UpdateText()
    {
        if (targetText == null)
            return;

        string fromText = originalText;
        string toText = finalText;

        int maxLength = Mathf.Max(fromText.Length, toText.Length);

        float revealProgress = useEaseIn ? EaseInCubic(progress) : progress;

        int totalRevealableCount = CountRevealableCharacters(toText);
        int revealedCount = Mathf.RoundToInt(totalRevealableCount * revealProgress);

        int currentRevealableIndex = 0;

        builder.Clear();

        for (int i = 0; i < maxLength; i++)
        {
            char fromChar = i < fromText.Length ? fromText[i] : ' ';
            char toChar = i < toText.Length ? toText[i] : ' ';

            // progress 0과 1은 무조건 원문/최종문을 그대로 보여준다
            if (progress <= 0f)
            {
                builder.Append(fromChar);
                continue;
            }

            if (progress >= 1f)
            {
                builder.Append(toChar);
                continue;
            }

            // 중간 상태에서는 Final Text 기준으로 공백 보존
            if (preserveSpaces && char.IsWhiteSpace(toChar))
            {
                builder.Append(toChar);
                continue;
            }

            bool revealed = currentRevealableIndex < revealedCount;

            if (revealed)
            {
                builder.Append(toChar);
            }
            else
            {
                builder.Append(GetRandomChar());
            }

            currentRevealableIndex++;
        }

        targetText.text = builder.ToString();
    }

    private int CountRevealableCharacters(string text)
    {
        int count = 0;

        for (int i = 0; i < text.Length; i++)
        {
            if (preserveSpaces && char.IsWhiteSpace(text[i]))
                continue;

            count++;
        }

        return count;
    }

    private float EaseInCubic(float x)
    {
        return x * x * x;
    }

    private char GetRandomChar()
    {
        if (string.IsNullOrEmpty(randomChars))
            return '?';

        int index = Random.Range(0, randomChars.Length);
        return randomChars[index];
    }
}