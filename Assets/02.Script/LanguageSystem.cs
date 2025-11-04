using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
public enum Language
{
    Korean,
    English,
    Japanese
}

public class LanguageSystem : MonoBehaviour
{
    private static LanguageSystem _instance;
    public static LanguageSystem Instance
    {
        get
        {
            if (_instance == null)
            {
                // 1. 씬에서 기존 인스턴스를 찾아봅니다.
                _instance = FindAnyObjectByType<LanguageSystem>();

                // 2. 씬에 인스턴스가 없다면 새로 생성합니다.
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("LanguageSystem");
                    _instance = singletonObject.AddComponent<LanguageSystem>();
                    _instance.LoadLocalizationData();
                }
            }
            return _instance;
        }
    }
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
        LoadLocalizationData();
        if (Instance != this)
        {
            Destroy(gameObject);
            return;
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
#if UNITY_WEBGL && !UNITY_EDITOR
        // 자동 생성된 데이터 시트를 사용합니다.
        // LanguageDataSheet_KR.Data가 IDictionary<string, string>이므로, Dictionary로 변환해줍니다.
        allLanguageData[Language.Korean] = new Dictionary<string, string>(LanguageDataSheet_KR.Data);
        Debug.Log("WebGL: Loaded language data from generated data sheet.");

#else
#endif
        foreach (string fileName in dataFileNames)
        {
            string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

            if (File.Exists(filePath))
            {
                string csvText = File.ReadAllText(filePath);
                ParseCSV(csvText);
            }
            else
            {
                Debug.LogError($"Cannot find localization file: {fileName}");
            }
        }
    }
    // 안드로이드뿐만 아니라 WebGL에서도 사용하므로 함수 이름 변경
    private IEnumerator LoadDataWithWebRequest(string path)
    {
        // UnityWebRequest를 사용하여 서버(또는 로컬 데이터)에 파일 요청
        UnityWebRequest www = UnityWebRequest.Get(path);
        yield return www.SendWebRequest(); // 요청이 끝날 때까지 대기

        if (www.result == UnityWebRequest.Result.Success)
        {
            // 성공적으로 텍스트를 다운로드하면 파싱 실행
            ParseCSV(www.downloadHandler.text);
        }
        else
        {
            Debug.LogError($"Failed to load localization file: {path} | Error: {www.error}");
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
