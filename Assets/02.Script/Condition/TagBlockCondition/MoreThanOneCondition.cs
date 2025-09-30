using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "TagBlockCondition", menuName = "Scriptable Objects/TagBlockCondition/MoreThan")]
public class MoreThanOneCondition : TagBlockCondition
{
    public int requiredCount = 1;

    public override bool IsSatisfied(TagBlockController controller, string groupName)
    {
        Debug.Log("만족하는지 체크");
        // Controller에게 groupName의 감지된 오브젝트 리스트를 직접 요청
        var detectedList = controller.detectedObject[groupName];

        if (detectedList != null)
        {
            // 리스트의 개수가 필요한 개수 이상인지 확인하여 true/false 반환
            return detectedList.Count >= requiredCount;
        }

        return false;
    }
}