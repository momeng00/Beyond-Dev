using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class UIElement : MonoBehaviour
{
    public UnityEvent OnCustomClick;
    private Coroutine selectionAnimationCoroutine;

    protected void PlaySelectionAnimation(IEnumerator animation)
    {
        StopSelectionAnimation();
        if (!isActiveAndEnabled)
            return;

        selectionAnimationCoroutine = StartCoroutine(RunSelectionAnimation(animation));
    }

    private IEnumerator RunSelectionAnimation(IEnumerator animation)
    {
        yield return animation;
        selectionAnimationCoroutine = null;
    }

    protected IEnumerator FadeSelectionImage(Image targetImage, float targetAlpha, float duration)
    {
        if (targetImage == null)
            yield break;

        Color color = targetImage.color;
        float startAlpha = color.a;
        yield return UIAnimationRoutine.Run(duration, progress => progress, t =>
        {
            color.a = Mathf.Lerp(startAlpha, targetAlpha, t);
            targetImage.color = color;
        });
    }

    protected virtual void OnDisable()
    {
        StopSelectionAnimation();
    }

    private void StopSelectionAnimation()
    {
        if (selectionAnimationCoroutine == null)
            return;

        StopCoroutine(selectionAnimationCoroutine);
        selectionAnimationCoroutine = null;
    }

    virtual public void Selected()
    {

    }
    virtual public void UnSelected()
    {

    }
    virtual public void Action()
    {
        OnCustomClick?.Invoke();
    }
}
