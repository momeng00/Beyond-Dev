using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TagBlock : Block
{
    public TextMeshPro text;
    private Animator ani;
    [SerializeField]private List<string> detectedLists = new List<string>();
    public Action OnSatisfied;
    public TagContent content;
    public LayerMask detectedLayer;
    private float duration = 0.3f;
    private Coroutine runningShakeCoroutine;
    public float magnitude;
    public GameObject render;
    Vector3 originalPosition;
    private void Awake()
    {
        ani = GetComponent<Animator>();
        content.gameObject.SetActive(false);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    override public void Start()
    {
        base.Start();
        originalPosition = render.transform.position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsInLayerMask(collision.gameObject, detectedLayer))
        {
            detectedLists.Add(collision.gameObject.name);
        }
        OnShow();
        OnBlockAction();
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (detectedLists.Contains(collision.gameObject.name))
        {
            detectedLists.Remove(collision.gameObject.name);
        }
        OnHide();
    }

    public override void OnBlockAction()
    {
        base.OnBlockAction();
        runningShakeCoroutine = StartCoroutine("OnAnimation");
    }

    public void OnShow()
    {

        if(detectedLists.Count > 0)
        {
            content.gameObject.SetActive(true);
        }
    }

    public void OnHide()
    {
        StopShaking();
        if (detectedLists.Count <= 0)
        {
            if (content.gameObject.activeSelf)
            {
                content.gameObject.SetActive(false);
            }
        }

    }

    public override void InitializeReset()
    {
        base.InitializeReset();
    }

    public override void ResetAction()
    {
        base.ResetAction();
    }

    protected bool IsInLayerMask(GameObject obj, LayerMask mask)
    {
        return ((1 << obj.layer) & mask) != 0;
    }
    public void StopShaking()
    {
        if (runningShakeCoroutine != null)
        {
            StopCoroutine("OnAnimation");
        }
    }
    private IEnumerator OnAnimation()
    {
        Debug.Log(originalPosition);
        float elapsed = 0.0f;

        // try...finally 구문을 사용하여 코루틴이 어떻게 종료되든 finally는 항상 실행되도록 보장
        try
        {
            // --- 1. 흔들리는 로직 ---
            while (elapsed < duration)
            {
                // 2D 게임이라면 Random.insideUnitCircle 사용
                Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * magnitude;

                render.transform.position = originalPosition + new Vector3(randomOffset.x, randomOffset.y, 0);

                elapsed += Time.deltaTime;
                yield return null; // 다음 프레임까지 대기
            }
        }
        finally
        {
            runningShakeCoroutine = null;

            // 오브젝트가 비활성화되거나 파괴되었을 경우를 대비한 안전장치
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine(ReturnToOriginalPosition(originalPosition));
            }
        }
        render.transform.position = originalPosition;
        yield return null;
    }

    private IEnumerator ReturnToOriginalPosition(Vector3 originalPosition)
    {
        float returnDuration = 0.2f;
        float timer = 0f;
        Vector3 currentPosition = render.transform.position;

        while (timer < returnDuration)
        {
            timer += Time.deltaTime;
            render.transform.position = Vector3.Lerp(currentPosition, originalPosition, timer / returnDuration);
            yield return null;
        }

        render.transform.position = originalPosition;
    }
}
