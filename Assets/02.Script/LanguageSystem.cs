using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class LanguageSystem : MonoBehaviour
{

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            LanguageSystem.Instance.ChangeLanguage(Language.English);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            LanguageSystem.Instance.ChangeLanguage(Language.Korean);
        }
    }
    public static LanguageSystem Instance { get; private set; }

    [Header("Language Settings")]
    [Tooltip("현재 게임에서 사용할 언어를 설정합니다.")]
    public Language currentLanguage = Language.Korean;
    public static event Action OnLanguageChanged;

    [Header("Data Files")]
    [Tooltip("로딩할 CSV 파일들의 이름을 입력하세요.")]
    private List<string> dataFileNames = new List<string>
    {
        "textSetting.csv"
    }; // 여러 파일 이름을 받을 리스트

    private Dictionary<Language, Dictionary<string, string>> allLanguageData;

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

    public void ChangeLanguage(Language newLanguage)
    {
        currentLanguage = newLanguage;
        // 언어가 변경되었음을 모든 구독자에게 알림
        OnLanguageChanged?.Invoke();
        Debug.Log($"Language changed to: {newLanguage}");
    }
    public string GetText(string key)
    {
        // 현재 언어의 딕셔너리가 존재하고, 그 안에 키가 있는지 확인
        if (allLanguageData != null &&
            allLanguageData.ContainsKey(currentLanguage) &&
            allLanguageData[currentLanguage].ContainsKey(key))
        {
            return allLanguageData[currentLanguage][key];
        }

        Debug.LogWarning($"Localization key not found for language '{currentLanguage}': {key}");
        return $"#{key}";
    }

    private void LoadLocalizationData()
    {
        // 딕셔너리를 먼저 초기화합니다.
        allLanguageData = new Dictionary<Language, Dictionary<string, string>>();

        foreach (string fileName in dataFileNames)
        {
            string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

#if UNITY_ANDROID && !UNITY_EDITOR
            StartCoroutine(LoadDataForAndroid(filePath));
#else
            if (File.Exists(filePath))
            {
                string csvText = File.ReadAllText(filePath);
                ParseCSV(csvText);
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
        string[] lines = csvText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        if (lines.Length <= 1) return;

        // --- 헤더 분석 ---
        string[] header = lines[0].Split(',');
        // <열 인덱스, 해당 언어 Enum> 맵 생성
        Dictionary<int, Language> columnIndexToLanguage = new Dictionary<int, Language>();
        for (int i = 1; i < header.Length; i++) // 1부터 시작 (0은 Key 열)
        {
            // 헤더의 문자열을 Language Enum으로 변환 시도
            if (Enum.TryParse(header[i].Trim(), out Language lang))
            {
                columnIndexToLanguage[i] = lang;
                // 만약 allLanguageData에 해당 언어 딕셔너리가 없다면 새로 생성
                if (!allLanguageData.ContainsKey(lang))
                {
                    allLanguageData[lang] = new Dictionary<string, string>();
                }
            }
        }

        // --- 데이터 분석 ---
        for (int i = 1; i < lines.Length; i++)
        {
            if (string.IsNullOrEmpty(lines[i])) continue;

            string[] columns = lines[i].Split(',');
            string key = columns[0].Trim();

            foreach (var pair in columnIndexToLanguage)
            {
                int colIndex = pair.Key;
                Language lang = pair.Value;
                if (columns.Length > colIndex)
                {
                    string value = columns[colIndex].Trim().Trim('"');
                    allLanguageData[lang][key] = value; // 키가 있으면 덮어쓰고, 없으면 추가
                }
            }
        }
    }
}
