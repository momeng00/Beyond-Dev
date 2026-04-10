using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownloadStationSwtich : Switch, IReset
{
    public List<ISwitchable> target = new List<ISwitchable>();
    private bool isEnter;
    [HideInInspector] public bool isUpload;
    [HideInInspector] public Animator anime;
    public Switch partnerSwitch;
    float originalAngle;
    public Coroutine activiteCoroutine;
    public bool IsSatisfied
    {
        get
        {
            OnSwitchAction?.Invoke(isEnter && isUpload);
            return isEnter && isUpload;
        }
    }

    private Collider2D col;

    public override void Awake()
    {
        base.Awake();
        anime = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        col.isTrigger = true;
        originalAngle = transform.eulerAngles.z;
    }
    private void Start()
    {
        GameManager.Instance.OnReset += ResetAction;
        InputSystem.Instance.RegisterAction(KeyState.Play_Key, KeyCode.E, Interact);
    }

    public override void SetSwitch(ISwitchable node)
    {
        base.SetSwitch(node);
        target.Add(node);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsInLayerMask(collision.gameObject, layerMask))
        {
            isEnter = true;
            materialInstance.SetFloat("_IsHovered", 1.0f);
        }
    }

    public override void Interact()
    {
        base.Interact();
        if (IsSatisfied)
        {
            AudioManager.Instance.PlayOneShotSFXAudio(AudioName.CameraSwitch);
            foreach (var block in target)
            {
                block.SwitchOn(true);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (IsInLayerMask(collision.gameObject, layerMask))
        {
            isEnter = false;
            materialInstance.SetFloat("_IsHovered", 0.0f);
        }
    }

    public void ResetAction()
    {
        isEnter = false;
    }

    public void InitializeReset()
    {

    }
    public IEnumerator FaceEachOther()
    {
        yield return new WaitForSeconds(1.25f);
        float duration = 0.5f;
        float time = 0f;

        // 현재 각도
        float startAngle = transform.eulerAngles.z;

        // 타겟 방향 계산
        Vector2 dir = (partnerSwitch.gameObject.transform.position - transform.position).normalized;

        // "위(Vector2.up)" 기준으로 각도 계산
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;

        // 방향 결정
        bool clockwise = partnerSwitch.gameObject.transform.position.x > transform.position.x;

        // 각도 차이 계산
        float delta = Mathf.DeltaAngle(startAngle, targetAngle);

        // 방향 강제 적용
        if (clockwise && delta > 0)
            delta -= 360f;
        else if (!clockwise && delta < 0)
            delta += 360f;

        float endAngle = startAngle + delta;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            t = Mathf.SmoothStep(0f, 1f, t);
            float angle = Mathf.Lerp(startAngle, endAngle, t);
            transform.rotation = Quaternion.Euler(0, 0, angle);

            yield return null;
        }

        // 마지막 보정
        transform.rotation = Quaternion.Euler(0, 0, endAngle);
        yield return new WaitForSeconds(0.75f);
        StopCoroutine(activiteCoroutine);
        activiteCoroutine = StartCoroutine(RotateBackRoutine());
    }

    public IEnumerator RotateBackRoutine()
    {
        float duration = 0.5f;
        float time = 0f;

        float startAngle = transform.eulerAngles.z;
        float endAngle = originalAngle;

        // 방향 다시 결정 (원하면 동일 규칙 사용)
        bool clockwise = partnerSwitch.gameObject.transform.position.x > transform.position.x;

        float delta = Mathf.DeltaAngle(startAngle, endAngle);

        if (clockwise && delta < 0)
            delta -= 360f;
        else if (!clockwise && delta > 0)
            delta += 360f;

        endAngle = startAngle + delta;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            t = Mathf.SmoothStep(0f, 1f, t);
            float angle = Mathf.Lerp(startAngle, endAngle, t);
            transform.rotation = Quaternion.Euler(0, 0, angle);

            yield return null;
        }

        transform.rotation = Quaternion.Euler(0, 0, endAngle);
    }
}