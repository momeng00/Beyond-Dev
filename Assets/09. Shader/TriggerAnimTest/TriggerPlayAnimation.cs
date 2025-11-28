using UnityEngine;

public class TriggerPlayAnimation : MonoBehaviour
{
    [Header("Trigger Filter")]
    [Tooltip("이 레이어에 포함된 오브젝트만 반응합니다.")]
    public LayerMask triggerLayer;

    [Header("Animation Settings")]
    [Tooltip("Enter 시 재생할 Animator State 이름")]
    public string enterClipName = "";

    [Tooltip("Exit 시 재생할 Animator State 이름")]
    public string exitClipName = "";

    [Header("Play Control")]
    [Tooltip("True면 Enter/Exit 각각 한 번만 실행")]
    public bool playOnce = true;

    private bool enterPlayed = false;
    private bool exitPlayed = false;

    private void OnTriggerEnter2D(Collider2D col)
    {
        // LayerMask 검사
        if (((1 << col.gameObject.layer) & triggerLayer) == 0)
            return;

        // 한 번만 실행
        if (playOnce && enterPlayed)
            return;

        // 충돌한 오브젝트의 Animator 가져오기
        Animator anim = col.GetComponent<Animator>();
        if (anim != null && !string.IsNullOrEmpty(enterClipName))
        {
            anim.Play(enterClipName, 0, 0f);
            enterPlayed = true;
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (((1 << col.gameObject.layer) & triggerLayer) == 0)
            return;

        if (playOnce && exitPlayed)
            return;

        Animator anim = col.GetComponent<Animator>();
        if (anim != null && !string.IsNullOrEmpty(exitClipName))
        {
            anim.Play(exitClipName, 0, 0f);
            exitPlayed = true;
        }
    }
}
