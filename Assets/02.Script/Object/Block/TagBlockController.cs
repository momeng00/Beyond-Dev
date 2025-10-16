using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TagBlockController : MonoBehaviour
{
    public TagBlockCondition condition;
    public Dictionary<string, List<Block>> blockPairs = new Dictionary<string, List<Block>>();
    public Dictionary<string, List<GameObject>> detectedObject = new Dictionary<string, List<GameObject>>();

    // <그룹 이름, 현재 활성 상태>
    public Dictionary<string, bool> isGroupActive = new Dictionary<string, bool>();
    void Awake()
    {

    }
    public void RegisterBlock(string groupName, Block block)
    {
        // 만약 처음 등록되는 그룹 이름이라면,
        if (!blockPairs.ContainsKey(groupName))
        {
            // 딕셔너리에 새로운 리스트를 생성합니다.
            blockPairs[groupName] = new List<Block>();
            detectedObject[groupName] = new List<GameObject>();
            isGroupActive[groupName] = false;
        }

        // 해당 그룹의 리스트에 블록을 추가합니다.
        blockPairs[groupName].Add(block);
    }

    public void OnObjectEntered(string groupName, GameObject obj)
    {
        if (detectedObject.ContainsKey(groupName))
        {
            List<GameObject> detectedList = detectedObject[groupName];
            if (!detectedList.Contains(obj))
            {
                detectedList.Add(obj);
                CheckCondition(groupName);
            }
        }
    }


    public void OnObjectExited(string groupName, GameObject obj)
    {
        if (detectedObject.ContainsKey(groupName))
        {
            List<GameObject> detectedList = detectedObject[groupName];
            detectedList.Remove(obj);
            CheckCondition(groupName);
        }
    }

    private void CheckCondition(string groupName)
    {
        if (condition == null) return;

        // 단일 Condition 에셋에게 조건이 만족되었는지 확인
        bool shouldBeActive = condition.IsSatisfied(this,groupName);

        if (shouldBeActive != isGroupActive[groupName])
        {
            isGroupActive[groupName] = !isGroupActive[groupName]; // 컨트롤러는 상태만 변경
            Debug.Log($"Group '{groupName}' state changing to: {isGroupActive[groupName]}");

            if (blockPairs.ContainsKey(groupName))
            {
                foreach (var block in blockPairs[groupName])
                {
                    block.OnBlockAction();
                }
            }
        }
    }
}