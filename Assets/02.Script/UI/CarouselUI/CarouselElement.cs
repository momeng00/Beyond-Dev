using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CarouselElement : MonoBehaviour
{

    private Image image;
    private RectTransform rect;
    private CanvasGroup canvasGroup;
    private Coroutine _moveCoroutine;
    public bool flag=false;
    
    
    private void Start()
    {
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
        image = GetComponent<Image>();
        rect = GetComponent<RectTransform>();
        CarouselUIManager.Instance.AddCarousel(this);   
    }
    private void Update()
    {
        if (flag)
        {
            if (Input.anyKeyDown)
            {
                CarouselUIManager.Instance.TestAction(this);
            }
        }
    }

    //기본이 될 그려질 순서를 정하기
    public void SetSortingOrder(int index)
    {
        image.transform.SetSiblingIndex(index);
        init();
    }

    //오른쪽에 있는 요소를 치우기 위한 기능
    public void OverList()
    {

    }

    //요소들의 위치를 초기화하기 위한 기능
    public void init()
    {

    }
    public void AddMoveTo()
    {
        StartCoroutine(AddAnimation());
    }
    public void MoveTo(Vector2 targetX,Vector2 size,float rot, float duration)
    {
        if (_moveCoroutine != null) StopCoroutine(_moveCoroutine);
        rect.localRotation = Quaternion.Euler(0f, rot, 0f);
        rect.sizeDelta = size;
        _moveCoroutine = StartCoroutine(MoveCoroutine(targetX, duration));
    }

    private IEnumerator MoveCoroutine(Vector2 targetX, float duration)
    {
        float startX = rect.anchoredPosition.x;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / duration);  // 부드러운 이동
            rect.anchoredPosition = new Vector2(Mathf.Lerp(startX, targetX.x, t),
                                                  rect.anchoredPosition.y);
            yield return null;
        }

        rect.anchoredPosition = new Vector2(targetX.x, rect.anchoredPosition.y);
    }
    IEnumerator AddAnimation(float duration = 0.56f)
    {
        float time = 0f;

        Vector2 startPos = new Vector2(260f, rect.anchoredPosition.y);
        Vector2 endPos = new Vector2(0f, rect.anchoredPosition.y);

        rect.anchoredPosition = startPos;
        canvasGroup.alpha = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            // 부드러운 ease (중요)
            t = Mathf.SmoothStep(0f, 1f, t);

            rect.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t);

            yield return null;
        }

        // 마지막 보정
        rect.anchoredPosition = endPos;
        canvasGroup.alpha = 1f;
    }

    //선택된 요소가 중앙에 있으면서 새로 들어오는 요소가 추가되는 작동방식이 필요함.
    //init로 가능한가? 위치를 Manager가 있는 곳이 기준점이 되고 좌로 이동시키면?
    //위치를 지정하는 방식이 뭐가 있을까?
    //0,10을 기준점으로 하면 될것같은데 선택된 요소를 기준점으로 두고 x의 값을 변경시키는 방식으로 진행하고
    //이동 자체를 어떤식으로 해야할까? 코루틴?
    //Update문으로도 가능하기는 한테 입력을 받는것도 아니고 다른 방식으로 구현하고 싶은데?
    //
}