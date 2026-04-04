using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SignFlag : MonoBehaviour
{

    public SpriteRenderer sign;
    private Coroutine activeCoroutine;
    private void Start()
    {

    }
    public void ShowSign()
    {
        sign.enabled = true;
        activeCoroutine = StartCoroutine(BlinkSign());
    }

    public void HideSign()
    {
        sign.enabled = false;
        StopCoroutine(activeCoroutine);
    }

    IEnumerator BlinkSign()
    {
        while (true)
        {
            sign.enabled = !sign.enabled;
            yield return new WaitForSeconds(0.6f);
        }
        
    }
}
