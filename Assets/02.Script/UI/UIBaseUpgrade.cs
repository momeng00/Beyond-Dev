using System;
using System.Collections;
using UnityEngine;
public enum direction
{
    up,
    down,
    left,
    right,
}

[RequireComponent(typeof(CanvasGroup), typeof(RectTransform))]
public class UIBaseUpgrade : MonoBehaviour
{
    [Range(0f, 100f)]
    public float ratio; //원본과 얼마나 떨어져서 이동할 것인지 최대 100
    public float delay = 0.72f;
    public direction direction = direction.right;
    public event Action onOpen;

    protected CanvasGroup canvasGroup;
    protected RectTransform rectTransform;
    protected Vector2 originalPosition;
    protected Vector3 originalScale;
    protected Coroutine currentCoroutine;
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();

        originalPosition = rectTransform.anchoredPosition;
        originalScale = rectTransform.localScale;

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
    public void Open()
    {
        onOpen?.Invoke();
        canvasGroup.alpha = 0f;
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(OpenCoroutine());
    }
    public void Close()
    {

    }
    IEnumerator OpenCoroutine()
    {
        Vector2 startPos = originalPosition;
        yield return null;
        switch (direction)
        {
            case direction.right: startPos = new Vector2(originalPosition.x + rectTransform.rect.x *(ratio/100), originalPosition.y); break;
            case direction.left: startPos = new Vector2(originalPosition.x - rectTransform.rect.x *(ratio/100), originalPosition.y); break;
            case direction.up: startPos = new Vector2(originalPosition.x, originalPosition.y + rectTransform.rect.y * (ratio/100)); break;
            case direction.down: startPos = new Vector2(originalPosition.x, originalPosition.y - rectTransform.rect.y * (ratio/100) ); break;
        }

        rectTransform.anchoredPosition = startPos;
        float timer = 0f;
        while (timer < delay)
        {
            timer += Time.unscaledDeltaTime;
            float t = Mathf.Sin((timer / delay) * Mathf.PI * 0.5f);

            rectTransform.anchoredPosition = Vector2.Lerp(startPos, originalPosition, t);
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);

            yield return null;
        }

        // 끝난 후 확실하게 값 고정
        rectTransform.anchoredPosition = originalPosition;
        rectTransform.localScale = originalScale;
        canvasGroup.alpha = 1f;
    }
}