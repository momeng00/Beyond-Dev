using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class MPB_AnimFloatLerp : MonoBehaviour
{
    public string propertyName = "Anim_"; // 변경할 머테리얼 Float 이름
    public float duration = 1f;           // Lerp 시간

    private SpriteRenderer sr;
    private Material mat;
    private int propertyID;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();

        // material 로 접근 → 자동으로 인스턴스 생성됨
        mat = sr.material;

        propertyID = Shader.PropertyToID(propertyName);
    }

    // ==================================
    //            PUBLIC API
    // ==================================

    /// <summary>
    /// EaseIn 0 → 1
    /// </summary>
    public void PlayAnimUp()
    {
        StopAllCoroutines();
        StartCoroutine(LerpFloatEaseIn(0f, 1f));
    }

    /// <summary>
    /// EaseIn 1 → 0
    /// </summary>
    public void PlayAnimDown()
    {
        StopAllCoroutines();
        StartCoroutine(LerpFloatEaseIn(1f, 0f));
    }

    // ==================================
    //             CORE LERP
    // ==================================

    private IEnumerator LerpFloatEaseIn(float startValue, float endValue)
    {
        float t = 0f;

        mat.SetFloat(propertyID, startValue);

        while (t < 1f)
        {
            t += Time.deltaTime / duration;

            // EaseIn (t^2)
            float easeT = t * t;

            float value = Mathf.Lerp(startValue, endValue, easeT);
            mat.SetFloat(propertyID, value);

            yield return null;
        }

        // 마지막 값 보정
        mat.SetFloat(propertyID, endValue);
    }


}
