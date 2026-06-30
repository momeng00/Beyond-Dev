using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    
    private List<Card> cardList = new List<Card>(); //기본적으로 AddLast로 들어감. Count를 통해서 뒤로 정렬을 해야함.
    private Card frontCard;
    private Coroutine coroutine;

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
    private void Start()
    {
        cardList.Clear();
    }
    [Range(0f, 100f)] public float ratio; //옆으로 움직일 비율 및 크기가 작아질 비율
    public float animDuration = 0.52f;
    public int limit; //카드를 가질 수 있는 최대치.
    public void AddCard(Card card)
    {
        //card.GetComponent<UIBaseUpgrade>().Open();
        cardList.Add(card);
        RefreshCardList();
    }

    public void RemoveCard(Card card) 
    {
        
    }

    public void RefreshCardList()
    {

        while(limit < cardList.Count)
        {
            Card lastCard = cardList[0];
            lastCard.GetComponent<UIBaseUpgrade>().Close();
            cardList.Remove(lastCard);
        }
        
        for (int index=0; index< cardList.Count; index++) // 0 1 2 / 3 1 2 3
        {
            cardList[index].MoveCard(cardList.Count - index, ratio, animDuration);
        }
    }
}