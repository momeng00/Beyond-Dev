using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
public struct VolumeData
{
    public string volumeName;
    public Volume volume;
    [HideInInspector]public GaussianBlur1DVolumeComponent blur;
}
public class EffectManager : MonoBehaviour
{
    public List<VolumeData> volumeList;
    private Dictionary<string,VolumeData> _volumeList = new Dictionary<string,VolumeData>();
    public float blurDuration = 0.3f;
    private bool isWorking = false;

    //싱글톤 사용
    private static EffectManager _instance;
    public static EffectManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<EffectManager>();
                if (_instance == null)
                    _instance = new GameObject("EffectManager").AddComponent<EffectManager>();
            }
            return _instance;
        }
    }
    private void Awake()
    {
       VolumeInit();    
    }
    void Start()
    {
        
    }
    public void VolumeInit()
    {
        _volumeList.Clear();

        foreach (var volume in volumeList)
        {
            VolumeData data = volume;

            if (data.volume == null)
            {
                Debug.LogWarning($"{data.volumeName}의 Volume이 없습니다.");
                continue;
            }

            if (data.volume.profile == null)
            {
                Debug.LogWarning($"{data.volumeName}의 Profile이 없습니다.");
                continue;
            }

            if (data.volume.profile.TryGet(
                out GaussianBlur1DVolumeComponent blur))
            {
                data.blur = blur;

                // 초기화된 데이터를 Dictionary에 저장
                _volumeList.Add(data.volumeName, data);
                Debug.Log(_volumeList.Count);
            }
            else
            {
                Debug.LogWarning(
                    $"{data.volumeName}의 Profile에 GaussianBlur1DVolumeComponent가 없습니다.");
            }
        }
    }
    public void BlurOnAnimation(string name)
    {
        isWorking = true;
        StartCoroutine(BlurSet(true,name));
    }
    public void BlurOffAnimation(string name)
    {
        isWorking = true;
        StartCoroutine(BlurSet(false, name));
    }

    //0이 잘보이는거 1이 안보이는거
    //false가 1이 됨(안보임)/ture가 0이 됨 (보임)
    public IEnumerator BlurSet(bool value, string name)
    {
        if (!_volumeList.TryGetValue(name, out VolumeData data))
        {
            Debug.LogWarning($"VolumeData '{name}'을 찾을 수 없습니다.");
            yield break;
        }

        if (data.blur == null)
        {
            Debug.LogWarning($"VolumeData '{name}'의 Blur가 없습니다.");
            yield break;
        }

        GaussianBlur1DVolumeComponent blur = data.blur;

        float timer = 0f;

        if (value)
        {
            while (timer < blurDuration)
            {
                timer += Time.unscaledDeltaTime;

                float t = Mathf.SmoothStep(
                    0f,
                    1f,
                    timer / blurDuration
                );

                blur.radius.value = Mathf.Lerp(1f, 0f, t);
                blur.dimmed.value = Mathf.Lerp(1f, 0f, t);

                yield return null;
            }

            blur.radius.value = 0f;
            blur.dimmed.value = 0f;
        }
        else
        {
            while (timer < blurDuration)
            {
                timer += Time.unscaledDeltaTime;

                float t = Mathf.SmoothStep(
                    0f,
                    1f,
                    timer / blurDuration
                );

                blur.radius.value = Mathf.Lerp(0f, 1f, t);
                blur.dimmed.value = Mathf.Lerp(0f, 1f, t);

                yield return null;
            }

            blur.radius.value = 1f;
            blur.dimmed.value = 1f;
        }

        isWorking = false;
    }
}