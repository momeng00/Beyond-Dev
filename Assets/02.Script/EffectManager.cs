using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

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
    public FocusMaskController focusMaskController; //죽을때 호출할 대상
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
    public float focusFadeDuration = 0.36f;
    private bool isFocusWorking = false;
    public void FocusOnPosition(CharacterControl control)
    {
        if (!isFocusWorking)
        {
            isFocusWorking = true;
            focusMaskController.ShowAtWorld(control.gameObject.transform.position, 5.0f);
            StartCoroutine(FocusOnPositionCoroutine(control));
        }
    }
    public void TurnOffFocus()
    {
        
    }

    public IEnumerator FocusOnPositionCoroutine(CharacterControl control)
    {
        control.PauseCharacter();
        //Scale을 처음크기에서 0.5까지 이동시키기
        float startScale = focusMaskController.ReturnScale();
        yield return null;
        yield return UIAnimationRoutine.Run(
            0.72f,
            progress => Mathf.Sin(progress * Mathf.PI * 0.5f),
            t =>
            {
                float offsetScale = Mathf.Lerp(startScale, focusMaskController.DefaultScale, t);
                focusMaskController.SetScale(offsetScale);
            });

        yield return new WaitForSeconds(0.18f);

        float closeStartScale = focusMaskController.ReturnScale();
        yield return UIAnimationRoutine.Run(
            0.36f,
            progress => Mathf.Sin(progress * Mathf.PI * 0.5f),
            t => focusMaskController.SetScale(
                Mathf.Lerp(closeStartScale, 0.0f, t)));

        focusMaskController.SetScale(0.0f);
        yield return null;

        float startAlpha = focusMaskController.ReturnAlpha();
        yield return UIAnimationRoutine.Run(
            focusFadeDuration,
            progress => progress,
            t => focusMaskController.SetAlpha(Mathf.Lerp(startAlpha, 0.0f, t)));

        focusMaskController.InitFocusMaskMaterial();
        control.UnPauseCharacter();
        GameManager.Instance.ResetGame();
        isFocusWorking = false;
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

        float startValue = value ? 1f : 0f;
        float targetValue = value ? 0f : 1f;

        yield return UIAnimationRoutine.Run(
            blurDuration,
            progress => Mathf.SmoothStep(0f, 1f, progress),
            t =>
            {
                blur.radius.value = Mathf.Lerp(startValue, targetValue, t);
                blur.dimmed.value = Mathf.Lerp(startValue, targetValue, t);
            });

        isWorking = false;
    }
}