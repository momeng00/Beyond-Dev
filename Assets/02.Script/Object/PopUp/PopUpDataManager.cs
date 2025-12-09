using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class PopUpDataManager : MonoBehaviour
{
    private static PopUpDataManager _instance;
    public static PopUpDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<PopUpDataManager>();
                if (_instance == null)
                {
                    _instance = new GameObject("PopUpDataManager").AddComponent<PopUpDataManager>();
                }

            }
            return _instance;
        }
    }
    private Dictionary<string, PopUpData> dataDictionary = new Dictionary<string, PopUpData>();


    private void Awake()
    {
        LoadJsonData();
    }

    private void LoadJsonData()
    {
        string fileName = "PopupData.json"; // 확장자 포함
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
        if (File.Exists(filePath))
        {
            string jsonText = File.ReadAllText(filePath);
            ParseJson(jsonText);
        }
        else
        {
            Debug.LogError($" JSON 파일을 찾을 수 없습니다: {filePath}");
        }
    }

    // JSON 문자열을 실제 데이터로 변환하는 함수 (공통 사용)
    private void ParseJson(string json)
    {
        // 기존 로직을 여기로 가져옵니다.
        PopupDataTable table = JsonUtility.FromJson<PopupDataTable>(json);
        if (table != null)
        {
            dataDictionary.Clear();
            foreach (var item in table.items)
            {

                if (!dataDictionary.ContainsKey(item.key))
                {
                    dataDictionary.Add(item.key, item);
                }
            }
            Debug.Log($"StreamingAssets 데이터 로드 완료: {dataDictionary.Count}개");
        }
    }


    // 4. 외부에서 데이터를 꺼내 쓰는 함수
    public PopUpData GetData(string key)
    {
        if (dataDictionary.ContainsKey(key))
            return dataDictionary[key];

        Debug.LogWarning($"키를 찾을 수 없습니다: {key}");
        return null;
    }
    //public PopUpData GetData(PopupKey keyEnum) //overload를 통해서 enum을 통해서도 가능하게.
    //{
    //    // Enum을 string으로 변환 ("St1_Tuto_Jump")
    //    string keyString = keyEnum.ToString();

    //    return GetData(keyString);
    //}
}