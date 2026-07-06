using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager 
{
    private const string SaveFileName = "save_data.json";
    private Dictionary<string, Stage> stageList = new Dictionary<string, Stage>();
    public SaveData CurrentData { get; private set; }
    private string SavePath => Path.Combine(Application.persistentDataPath, SaveFileName);
    private static SaveManager _instance;
    public static SaveManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SaveManager();
                _instance.Load();
            }
            return _instance;
        }
    }
    public void RegisterStage(Stage stage)
    {
        if (!stageList.ContainsKey(stage.stageName))
        {
            stageList.Add(stage.stageName, stage);
        }
    }
    public void Save()
    {
        // 현재 저장 데이터가 없으면 기본 데이터로 생성한다.
        if (CurrentData == null)
        {
            CurrentData = CreateDefaultSaveData();
        }

        // SaveData를 JSON 문자열로 변환한 뒤 파일로 저장한다.
        string json = JsonUtility.ToJson(CurrentData, true);
        File.WriteAllText(SavePath, json);

        Debug.Log($"Save completed: {SavePath}");
    }

    public void Load()
    {
        // 저장 파일이 없으면 기본 데이터를 만들고 새로 저장한다.
        if (!File.Exists(SavePath))
        {
            CurrentData = CreateDefaultSaveData();
            Save();
            return;
        }

        // 저장 파일의 JSON을 읽어서 SaveData로 변환한다.
        string json = File.ReadAllText(SavePath);
        CurrentData = JsonUtility.FromJson<SaveData>(json);

        // JSON 변환에 실패했을 경우 기본 데이터로 복구한다.
        if (CurrentData == null)
        {
            CurrentData = CreateDefaultSaveData();
            Save();
            return;
        }

        EnsureData();
    }

    public bool HasSaveData()
    {
        // 저장 파일이 존재하는지 확인한다.
        return File.Exists(SavePath);
    }

    public void DeleteSaveData()
    {
        // 저장 파일이 있으면 삭제한다.
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
        }

        // 메모리의 현재 데이터는 기본 데이터로 초기화한다.
        CurrentData = CreateDefaultSaveData();
    }

    private SaveData CreateDefaultSaveData()
    {
        // 새로운 기본 저장 데이터를 생성한다.
        return new SaveData();
    }

    private void EnsureData()
    {
        // 로드된 데이터에서 settings가 비어 있으면 기본값을 넣는다.
        if (CurrentData.settings == null)
        {
            CurrentData.settings = new SettingData();
        }

        // 로드된 데이터에서 progress가 비어 있으면 기본값을 넣는다.
        if (CurrentData.progress == null)
        {
            CurrentData.progress = new ProgressData();
        }

        // 저장 데이터 버전이 없거나 잘못되어 있으면 기본 버전으로 설정한다.
        if (CurrentData.version <= 0)
        {
            CurrentData.version = 1;
        }
    }
}