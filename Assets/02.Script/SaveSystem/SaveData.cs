using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public int version;
    public SettingData settings = new SettingData();
    public ProgressData progress = new ProgressData();
}

[Serializable]
public class SettingData //유저가 세팅하는 값을 저장할 곳.
{
    public float bgmVolume = 0.5f;
    public float sfxVolume = 0.5f;

    public int resolutionWidth = 1920;
    public int resolutionHeight = 1080;
    public string screenMode = "FullScreen";

    public Language selectLanguage = Language.Korean;
    public KeyState gameState = KeyState.Play_Key;
}


[System.Serializable]
public class ProgressData //게임 내부적으로 필요한 세팅을 저장할 곳
{
    public string currentStageId = "stage_01";
    public bool flag = false;
    
}