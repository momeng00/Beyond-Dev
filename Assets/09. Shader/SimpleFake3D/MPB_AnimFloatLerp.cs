using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SpriteRenderer))]
public class MPB_AnimFloatLerp : MonoBehaviour
{
    public string propertyName = "Anim_"; // 머테리얼 프로퍼티 이름
    public float duration = 1f;           // Lerp 시간(초)

    private SpriteRenderer sr;
    private MaterialPropertyBlock mpb;
    private int propertyID;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        mpb = new MaterialPropertyBlock();
        propertyID = Shader.PropertyToID(propertyName);
    }

    // ===== 외부 이벤트 =====

    /// <summary>
    /// EaseIn 0 -> 1
    /// </summary>
    public void PlayAnimUp()
    {
        StopAllCoroutines();
        StartCoroutine(LerpFloatEaseIn(0f, 1f));
    }

    /// <summary>
    /// EaseIn 1 -> 0
    /// </summary>
    public void PlayAnimDown()
    {
        StopAllCoroutines();
        StartCoroutine(LerpFloatEaseIn(1f, 0f));
    }


    // ===== 공용 코루틴 =====

    private IEnumerator LerpFloatEaseIn(float startValue, float endValue)
    {
        float t = 0f;

        // 초기값 설정
        sr.GetPropertyBlock(mpb);
        mpb.SetFloat(propertyID, startValue);
        sr.SetPropertyBlock(mpb);

        while (t < 1f)
        {
            t += Time.deltaTime / duration;

            // ------------------------------
            // EaseIn 적용 (Quint, Quad 등 선택 가능)
            // 여기서는 Quad (t^2)
            float easeT = t * t;
            // ------------------------------

            float value = Mathf.Lerp(startValue, endValue, easeT);

            sr.GetPropertyBlock(mpb);
            mpb.SetFloat(propertyID, value);
            sr.SetPropertyBlock(mpb);

            yield return null;
        }

        // 최종값 보정
        sr.GetPropertyBlock(mpb);
        mpb.SetFloat(propertyID, endValue);
        sr.SetPropertyBlock(mpb);
    }


    // ===== 에디터 테스트 =====

    [ContextMenu("▶ Test Up EaseIn 0->1")]
    private void TestUp()
    {
        PlayAnimUp();
    }

    [ContextMenu("▶ Test Down EaseIn 1->0")]
    private void TestDown()
    {
        PlayAnimDown();
    }
}
