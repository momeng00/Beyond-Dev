using System;
using UnityEngine;

[DefaultExecutionOrder(-1000)]
public class SettingsLifecycle : MonoBehaviour
{
    private SettingData settings;
    private bool canSaveSettings;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Create()
    {
        GameObject target = new GameObject("SettingsLifecycle");
        target.AddComponent<SettingsLifecycle>();
        DontDestroyOnLoad(target);
    }

    private void Start()
    {
        bool loadSucceeded = SaveManager.Instance.TryLoadSettings(out settings);
        ApplySettings();
        canSaveSettings = loadSucceeded;
    }

    private void OnApplicationQuit()
    {
        if (settings == null || !canSaveSettings)
            return;

        CaptureSettings();
        SaveManager.Instance.SaveSettings(settings);
    }

    private void ApplySettings()
    {
        AudioManager audio = FindAnyObjectByType<AudioManager>();
        if (audio != null)
        {
            audio.MusicVolume = settings.bgmVolume;
            audio.SFXVolume = settings.sfxVolume;
        }

        LanguageSystem.Instance.ChangeLanguage(settings.selectLanguage);

        // SaveManager에서 유효한 화면 모드 이름인지 검사한 뒤 전달한다.
        FullScreenMode mode = (FullScreenMode)Enum.Parse(
            typeof(FullScreenMode), settings.screenMode);
        LetterboxScaler scaler = FindAnyObjectByType<LetterboxScaler>();
        if (scaler != null)
        {
            scaler.ApplyDisplaySettings(
                settings.resolutionWidth, settings.resolutionHeight, mode);
        }
        else
        {
            Screen.SetResolution(
                settings.resolutionWidth, settings.resolutionHeight, mode);
        }
    }

    private void CaptureSettings()
    {
        // 종료 시 Instance로 없던 설정 객체를 새로 생성하지 않는다.
        AudioManager audio = FindAnyObjectByType<AudioManager>();
        if (audio != null)
        {
            settings.bgmVolume = audio.MusicVolume;
            settings.sfxVolume = audio.SFXVolume;
        }

        LanguageSystem language = FindAnyObjectByType<LanguageSystem>();
        if (language != null)
        {
            settings.selectLanguage = language.currentLanguage;
        }

#if !UNITY_EDITOR
        // Editor Game View 크기로 제품의 화면 설정을 덮어쓰지 않는다.
        settings.resolutionWidth = Screen.width;
        settings.resolutionHeight = Screen.height;
        settings.screenMode = Screen.fullScreenMode.ToString();
#endif
    }
}
