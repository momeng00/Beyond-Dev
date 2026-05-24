using System.Collections.Generic;
using UnityEngine;


public class CarouselUIManager
{
    private static CarouselUIManager _instance;
    public static CarouselUIManager Instance
    {
        get 
        {
            if (_instance==null)
            {
                _instance = new CarouselUIManager();
            }
            return _instance; 
        }
    }
    public int currentIndex;
    float width = 500f;
    float height = 880f;
    private float animDuration = 0.52f;
    private List<CarouselElement> uis = new List<CarouselElement>();
    public List<CarouselElement> testUi = new List<CarouselElement>();
    public void AddCarousel(CarouselElement carousel)
    {
        if (uis.Contains(carousel))
        {
            uis.Remove(carousel);
        }
        uis.Add(carousel); //Add가 이미 AddLast임.
        Debug.Log("count = " + uis.Count);
        //RefreshCarousel();
    }
    public void RemoveCarousel(CarouselElement carousel)
    {
        uis.Remove(carousel);
    }
    public void RefreshCarousel()
    {
        for (int i = 0; i < uis.Count; i++)
        {
            uis[i].transform.SetSiblingIndex(i);
            var (pos, rot, size) = GetValue(uis.Count - i);
            uis[i].MoveTo(pos,size,rot,animDuration);
            //float targetX = -offsetPerIndex * i;
            //uis[i].MoveTo(targetX, animDuration);
        }
    }
    public void TestAction(CarouselElement carousel)
    {
        AddCarousel(uis[0]);
        RefreshCarousel();
        uis[^1].AddMoveTo();
    }
    (Vector2 pos, float rot, float size) GetValue(int index)
    {
        int n = index;
        float t = n - 1f;
        // 이동 (너가 만든 값)
        float x = -210f * Mathf.Log(n);
        x = Mathf.Max(-260f, x);
        Debug.Log(n);
        // 회전
        float rot = -20f * t * Mathf.Exp(-0.8f * t);
        rot = Mathf.Min(0f, rot);
        // 스케일
        float scaleFactor = 1f - 0.15f * Mathf.Log(n);
        scaleFactor = Mathf.Clamp(scaleFactor, 0.7f, 1f);

        return (new Vector2(x, 10f), rot, scaleFactor);
    }
    //맨앞에 있는 요소가 아닌 이상 크기를 줄이면서 카메라 요소를 활용해서 Fake2D 기능을 사용해야함
    //좌우에 있는 요소들을 볼 수 있게 현재에 있는 요소 좌우로 크기를 줄여야하기 때문에 좌우 요소를 판별할 수 있어야함.
    //위치의 순서를 바꾸는 작업을 일어나지 않지만 잴 앞에 있는 요소가 변경되는 작업이 진행되어야함.
    //순서 자체가 변경되지는 않으니, 잴 앞에 있는 요소만 판별하기만 하면 됨.
    //그러면 요소들의 순서 자체는 상관이 없지만 좌우에 대해서는 있으면 되는데 LinkedList를 활용해서 작업하면 되려나?
    //LinkedList를 진행하고 좌우 끝까지 할 수 있는 방식을 가져오면 될 것 같은데. LinkedList에 처음 들어간 요소.
    //마지막으로 들어가는 오브젝트에 대해서 Last를 체크해줘서 작업하는 방식으로 하면 될듯.
    //요소 자체에 Head, Tail이라는 요소를 두고 들어갈때 Tail을 변경하는 방식으로 하고 좌우 끝에 있는 요소들을 foreach?는 아닌데?
    //시작 요소로 부터 좌우로 가면서 재귀함수를 사용해야하나? 이걸 재귀함수라고 하지는 않을것같은데?
    //정렬함수중에서 이게 있는데... 뭐였드라...
    //Mono넣을 필요는 없을듯
    //image.transform.SetSiblingIndex(index); 이거를 이용해서 하면 될듯
    //좌우로 할 필요가 없어짐. Count를 통해서 정렬하면 될듯 List자체만으로도 충분히 가능하고
    //오른쪽에 있는 요소는 치워버리는 기능이 필요함.
    //들어가면서 Last에 집어 넣는 것만 하면 될듯.
    //중간에 있던 요소가 뒤로 다시 갈때가 있을까?
    //중간에 있는 요소를 List에 넣을때 체크하면 되네.
    //기능을 정리하면?
    //삭제, 추가(Last에 넣어야함.) List에 존재함.
    //넣을때 중복체크를 하고 넣어야함.
    //현재 선택된 요소 오른쪽에 있는건 좀 더 멀리 둬야함.
    //넣을때 Count를 체크해서 요소를 맨 밑에 둬야함.
    //추가되면서 이동하거나 해야함. 우로 이동하거나 새로이 이동되는 방식이 필요함

    //Add가 호출되면 Refresh가 작동이 되고 Element에 
    //Add에 대한 애니메이션?과 이동에 관한 애니메이션이 있어야함.
    //Add에 대한 애니메이션은 페이드 In(Color의 Alpha값을 변경해야함)이 되면서 중앙으로 오도록 해야함
    //Refresh가 언제 되는지를 정해야함.
    //Hide가 반대로 작동되는거 페이드 Out되면서 사라지는거.
    //Hide뒤에 Delete될지 작용시키는 방법
    //추가는 Clear액션 자체에서 하면 될듯
    //얼마나 움직일지 계산 수식. log값 이용하면 될듯.
}