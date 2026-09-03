using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.GPUSort;

public class CardManager : MonoBehaviour
{
    //Card에서 만들어진 기능을 원하는 타이밍에 맞게 실행을 시킬 수 있어야 함.
    //맨 앞에 있는 요소를 체크하는 방식 및 카드에서 사용되는 애니메이션이 끝나면 카드매니저에게 다시 부르는 식으로
    //맨앞에 추가되는 애니메이션, 맨 앞에 있는거 확인하는거
    //맨 앞에 있는 Todo랑 동기화 (4버튼 시 뜨는 화면)
    //맨 앞에 있는것만 보이게 TodoCheck 게임 클리어 시.
    private static CardManager _instance;
    public static CardManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<CardManager>();
                if (_instance == null)
                    _instance = new CardManager();
            }
            return _instance;
        }
    }
    private List<Card> cardList = new List<Card>(); //기본적으로 AddLast로 들어감. Count를 통해서 뒤로 정렬을 해야함.
    public int limit; //카드를 가질 수 있는 최대치.
    public float animDuration = 0.52f;
    [Range(0f, 100f)] public float ratio; //옆으로 움직일 비율 및 크기가 작아질 비율 카드 끼리 간격에 해당
    private Coroutine coroutine;

    
    private void Start()
    {
        cardList.Clear();
    }

    public void Update()
    {
        
    }

    public void AddCard(Card card)
    {
        //card.GetComponent<UIBaseUpgrade>().Open();
        if (cardList.Contains(card))
            return;

        cardList.Add(card);
        if (cardList.Count > limit)
        {
            RemoveCard();
        }
        RefreshCardList();
    }

    public void RemoveCard()
    {
        if (cardList.Count == 0 || cardList == null)
        {
            return;
        }
        Card card = cardList[0];
        cardList.RemoveAt(0);
        card.RemoveCard();
    }
    public void HideCardList()
    {
        foreach (Card card in cardList) {
            card.RemoveCard();
        }
    }
    public void RefreshCardList()
    {
        while (limit < cardList.Count)
        {
            Card lastCard = cardList[0]; // 처음 들어온 카드 제거
            lastCard.RemoveCard(); //삭제 애니메이션은 필요없을듯 그냥 안보이게만 하는 용도
            cardList.Remove(lastCard); 
        }
        for (int i = 0; i < cardList.Count; i++)
        {
            StartCoroutine(
                MoveCardCoroutine(
                    cardList[i],
                    i,
                    ratio,
                    animDuration
                )
            );
        }
        
        
        //for (int index=0; index< cardList.Count; index++) // 0 1 2 / 3 1 2 3 위에 기능으로 이전 됨
        //{
        //    cardList[index].MoveCard(cardList.Count - index, ratio, animDuration);
        //}
    }

    private IEnumerator MoveCardCoroutine(Card card, int index, float ratio, float animDuration)
    {
        RectTransform rect = card.Rect;
        CanvasGroup canvas = card.CanvasGroup;

        canvas.alpha = 1f; //보이게 하고

        yield return null;

        float targetOffsetX =
         -rect.rect.width *
         (ratio / 100f) *
         (cardList.Count-index);

        Vector2 startPos = card.OriginalPosition; //현재 위치

        Vector2 targetPos = card.OriginalPosition + new Vector2(targetOffsetX, 0f); // 이동될 위치

        float elapsed = 0f; 

        while (elapsed < animDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Sin(
                elapsed / animDuration * Mathf.PI * 0.5f
            );

            rect.anchoredPosition =
                Vector2.Lerp(startPos, targetPos, t);

            yield return null;
        }

        rect.anchoredPosition = targetPos;
    }
    public void ShowTopCard()
    {
        if(cardList[^1]!=null)
            cardList[^1].CheckClearCard();
    }
    public void HideTopCard(Card card)
    {
        
    }
    //Card에 있는 IEnumerator을 가져와서 여기에 종속시키기
    //맨 앞에 있는 카드만 가져와서 ClearCheck하는 기능을 추가
    //맨 앞에 있는 카드의 TodoList를 PauseMenu에 동기화 시키는 방식
}