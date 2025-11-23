using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMoveTest : MonoBehaviour
{
    public Transform target;
    public float followSpeed = 5f;
    public float interval = 1f;
    Queue<(Vector3 pos, float time)> history = new();

    private Vector3 nextPosition;
    bool flag;
    private void Start()
    {
        flag = true;
        StartCoroutine(FollowRoutine());
    }

    IEnumerator FollowRoutine()
    {
        while (true)
        {
            nextPosition = target.position; // 1초마다 목표 위치 기록
            yield return new WaitForSeconds(interval);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            flag = !flag;
        }
        if (flag)
        {
            // 기록된 위치로 부드럽게 이동
            transform.position = Vector3.Lerp(transform.position, nextPosition, Time.deltaTime * followSpeed);
        }
        else 
        {

            history.Enqueue((target.position, Time.time));
            // 2) 오래된 기록 제거 (1초 이상 지난 것만)
            while (history.Count > 0 && Time.time - history.Peek().time > interval)
            {
                var old = history.Dequeue();
                transform.position = old.pos;
            }
        }
        
    }

}