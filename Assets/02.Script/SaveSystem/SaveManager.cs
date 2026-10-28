using System;
using System.IO;
using UnityEngine;

public class SaveManager
{
    private const int SettingsVersion = 1;
    private const string SettingsFileName = "settings.json";
    private static SaveManager _instance;
    private bool preserveBackup;

    private string SettingsPath => Path.Combine(Application.persistentDataPath, SettingsFileName);
    private string BackupPath => SettingsPath + ".bak";

    public static SaveManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = new SaveManager();

            return _instance;
        }
    }

    public bool TryLoadSettings(out SettingData settings)
    {
        settings = new SettingData();
        preserveBackup = false;

        try
        {
            if (TryReadSettings(SettingsPath, out SettingData loadedSettings, out bool mainMissing))
            {
                settings = loadedSettings;
                return true;
            }

            if (TryReadSettings(BackupPath, out loadedSettings, out bool backupMissing))
            {
                settings = loadedSettings;
                preserveBackup = true;
                Debug.LogWarning("설정 파일을 읽을 수 없어 백업 설정을 사용합니다.");
                return true;
            }

            // 두 파일이 모두 없는 첫 실행에만 기본값 저장을 허용한다.
            if (mainMissing && backupMissing)
                return true;

            Debug.LogError("설정을 복구하지 못했습니다. 기본값을 사용하며 이번 실행에서는 저장 파일을 덮어쓰지 않습니다.");
        }
        catch (Exception exception)
        {
            // 접근 실패나 다른 버전의 파일은 백업으로 대체해 덮어쓰지 않는다.
            Debug.LogError($"설정 불러오기 실패: {exception.Message}");
        }

        return false;
    }

    public bool SaveSettings(SettingData settings)
    {
        if (!IsValidSettings(settings))
        {
            Debug.LogError("저장할 설정 데이터가 유효하지 않습니다.");
            return false;
        }

        try
        {
            Directory.CreateDirectory(Application.persistentDataPath);
            string temporaryPath = SettingsPath + ".tmp";
            string json = JsonUtility.ToJson(settings, true);
            File.WriteAllText(temporaryPath, json);

            if (File.Exists(SettingsPath))
            {
                // 백업에서 복구했다면 손상된 본 파일로 정상 백업을 덮지 않는다.
                if (preserveBackup)
                    File.Replace(temporaryPath, SettingsPath, null);
                else
                    File.Replace(temporaryPath, SettingsPath, BackupPath);
            }
            else
            {
                File.Move(temporaryPath, SettingsPath);
            }

            preserveBackup = false;
            return true;
        }
        catch (Exception exception)
        {
            Debug.LogError($"설정 저장 실패: {exception.Message}");
            return false;
        }
    }

    private bool TryReadSettings(string path, out SettingData settings, out bool fileMissing)
    {
        settings = null;
        fileMissing = false;
        string json;
        try
        {
            // File.Exists로 권한 오류를 파일 없음으로 오인하지 않도록 직접 읽는다.
            json = File.ReadAllText(path);
        }
        catch (FileNotFoundException)
        {
            fileMissing = true;
            return false;
        }
        catch (DirectoryNotFoundException)
        {
            fileMissing = true;
            return false;
        }

        // 누락된 필드를 기본값으로 채워 정상 파일로 오인하지 않도록 검사값을 사용한다.
        SettingData candidate = new SettingData
        {
            version = 0,
            bgmVolume = float.NaN,
            sfxVolume = float.NaN,
            resolutionWidth = 0,
            resolutionHeight = 0,
            screenMode = null,
            selectLanguage = (Language)(-1)
        };

        try
        {
            if (string.IsNullOrWhiteSpace(json))
                return false;

            JsonUtility.FromJsonOverwrite(json, candidate);
        }
        catch (Exception)
        {
            return false;
        }

        if (candidate.version > SettingsVersion)
            throw new NotSupportedException($"지원하지 않는 설정 버전입니다 ({path}): {candidate.version}");

        if (!IsValidSettings(candidate))
            return false;

        settings = candidate;
        return true;
    }

    private bool IsValidSettings(SettingData settings)
    {
        if (settings == null || settings.version != SettingsVersion)
            return false;

        if (!IsValidVolume(settings.bgmVolume) || !IsValidVolume(settings.sfxVolume))
            return false;

        if (settings.resolutionWidth <= 0 || settings.resolutionHeight <= 0)
            return false;

        if (!Enum.IsDefined(typeof(Language), settings.selectLanguage))
            return false;

        if (!Enum.TryParse(settings.screenMode, out FullScreenMode mode))
            return false;

        if (!Enum.IsDefined(typeof(FullScreenMode), mode))
            return false;

        // 숫자 문자열이나 별칭 대신 Unity의 정식 모드 이름만 저장한다.
        return settings.screenMode == mode.ToString();
    }

    private bool IsValidVolume(float volume)
    {
        return !float.IsNaN(volume) && !float.IsInfinity(volume) && volume >= 0f && volume <= 1f;
    }
}
