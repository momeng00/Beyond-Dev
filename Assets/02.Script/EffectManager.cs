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
       
    }
    void Start()
    {
        
    }
    public void VolumeInit()
    {
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
            }
            else
            {
                Debug.LogWarning(
                    $"{data.volumeName}의 Profile에 GaussianBlur1DVolumeComponent가 없습니다.");
            }
        }
    }
    public void BlurAnimation(bool value)
    {
        isWorking = true;
        StartCoroutine(BlurSet(value));
    }

    //0이 잘보이는거 1이 안보이는거
    //false가 1이 됨(안보임)/ture가 0이 됨 (보임)
    public IEnumerator BlurSet(bool value)
    {
        float timer = 0f;
        if (value)
        {
            // 애니메이션 루프
            while (timer < blurDuration)
            {
                timer += Time.unscaledDeltaTime;
                float t = Mathf.SmoothStep(0f, 1f, timer / blurDuration);
                //blur.radius.value = Mathf.Lerp(1f, 0f, t);
                //blur.dimmed.value = Mathf.Lerp(1f, 0f, t);
                yield return null;
            }
            //blur.radius.value = 0f;
            //blur.dimmed.value = 0f;
        }
        else
        {
            // 애니메이션 루프
            while (timer < blurDuration)
            {
                timer += Time.unscaledDeltaTime;
                float t = Mathf.SmoothStep(0f, 1f, timer / blurDuration);
                //blur.radius.value = Mathf.Lerp(0f, 1f, t);
                //blur.dimmed.value = Mathf.Lerp(0f, 1f, t);
                yield return null;
            }
            //blur.radius.value = 1f;
            //blur.dimmed.value = 1f;
        }
        isWorking = false;
    }
}