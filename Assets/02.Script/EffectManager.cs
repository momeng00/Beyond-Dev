using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.Rendering.DebugUI;

public class EffectManager : MonoBehaviour
{
    public Volume volume;
    public Volume volume2;
    public float blurDuration = 0.3f;
    GaussianBlur1DVolumeComponent blur;
    GaussianBlur1DVolumeComponent blur2;
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
        if (volume.profile.TryGet(out blur))
        {
            
        }
        else
        {
            Debug.Log("Blur 못 찾음");
        }

        if (volume2.profile.TryGet(out blur2))
        {
            
        }
        else
        {
            Debug.Log("Blur2 못 찾음");
        }
    }
    void Start()
    {
        
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
                blur.radius.value = Mathf.Lerp(1f, 0f, t);
                blur.dimmed.value = Mathf.Lerp(1f, 0f, t);
                yield return null;
            }
            blur.radius.value = 0f;
            blur.dimmed.value = 0f;
        }
        else
        {
            // 애니메이션 루프
            while (timer < blurDuration)
            {
                timer += Time.unscaledDeltaTime;
                float t = Mathf.SmoothStep(0f, 1f, timer / blurDuration);
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