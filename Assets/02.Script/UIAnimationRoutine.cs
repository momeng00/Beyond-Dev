using System;
using System.Collections;
using UnityEngine;

public static class UIAnimationRoutine
{
    public static IEnumerator Run(float duration, Func<float, float> easing, Action<float> apply)
    {
        if (duration <= 0f)
        {
            apply(1f);
            yield break;
        }

        float elapsed = 0f;
        apply(0f);

        while (elapsed < duration)
        {
            yield return null;

            elapsed += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            apply(easing(progress));
        }

        apply(1f);
    }
}