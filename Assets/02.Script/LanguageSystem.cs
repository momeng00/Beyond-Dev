using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class LanguageSystem : MonoBehaviour
{

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log(LanguageSystem.Instance.GetText("BLOCK_TEMP"));
        }   
    }
    public static LanguageSystem Instance { get; private set; }

    [Header("Language Settings")]
    [Tooltip("현재 게임에서 사용할 언어를 설정합니다.")]
    public Language targetLanguage = Language.Korean;

    [Header("Data Files")]
    [Tooltip("로딩할 CSV 파일들의 이름을 입력하세요.")]
    private List<string> dataFileNames = new List<string>
    {
        "textSetting.csv"
    }; // 여러 파일 이름을 받을 리스트

    private Dictionary<string, string> currentLanguageData;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadLocalizationData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public string GetText(string key)
    {
        if (currentLanguageData != null && currentLanguageData.ContainsKey(key))
        {
            return currentLanguageData[key];
        }

        Debug.LogWarning($"Localization key not found: {key}");
        return $"#{key}";
    }

    private void LoadLocalizationData()
    {
        Debug.Log("Load text File");
        // 딕셔너리를 먼저 초기화합니다.
        currentLanguageData = new Dictionary<string, string>();

        // 지정된 모든 파일에 대해 로딩을 시도합니다.
        foreach (string fileName in dataFileNames)
        {
            string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

#if UNITY_ANDROID && !UNITY_EDITOR
            StartCoroutine(LoadDataForAndroid(filePath));
#else
            if (File.Exists(filePath))
            {
                Debug.Log("Exist text File");
                string csvText = File.ReadAllText(filePath);
                ParseCSV(csvText); // 읽어온 데이터를 기존 딕셔너리에 추가
            }
            else
            {
                Debug.LogError($"Cannot find localization file: {fileName}");
            }
#endif
        }
    }

    private IEnumerator LoadDataForAndroid(string path)
    {
        UnityWebRequest www = UnityWebRequest.Get(path);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.Success)
        {
            ParseCSV(www.downloadHandler.text);
        }
        else
        {
            Debug.LogError($"Failed to load localization file on Android: {path} | Error: {www.error}");
        }
    }

    private void ParseCSV(string csvText)
    {
        string[] lines = csvText.Split(new[] { "\r\n", "\n" }, System.StringSplitOptions.None);
        if (lines.Length <= 1) return;

        string[] header = lines[0].Split(',');
        int languageColumnIndex = -1;
        for (int i = 0; i < header.Length; i++)
        {
            if (header[i].Trim() == targetLanguage.ToString())
            {
                languageColumnIndex = i;
                break;
            }
        }

        if (languageColumnIndex == -1)
        {
            Debug.LogError($"Language column '{targetLanguage}' not found in CSV header.");
            return;
        }

        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrEmpty(lines[i])) continue;

            string[] columns = lines[i].Split(',');
            if (columns.Length > languageColumnIndex)
            {
                string key = columns[0].Trim();
                string value = columns[languageColumnIndex].Trim().Trim('"');

                // 키가 이미 존재하면 경고를 표시하고, 아니면 추가합니다.
                if (currentLanguageData.ContainsKey(key))
                {
                    Debug.LogWarning($"Duplicate key found: '{key}'. The value will be overwritten.");
                    currentLanguageData[key] = value; // 덮어쓰기
                }
                else
                {
                    currentLanguageData.Add(key, value);
                }
            }
        }
    }
}
