using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
                    _instance = new GameObject("CardManager").AddComponent<CardManager>();
            }
            return _instance;
        }
    }
    private List<Card> cardList = new List<Card>(); //기본적으로 AddLast로 들어감. Count를 통해서 뒤로 정렬을 해야함.
    public int limit = 1; //카드를 가질 수 있는 최대치.
    public float animDuration = 0.52f;
    [Range(0f, 100f)] public float ratio; //옆으로 움직일 비율 및 크기가 작아질 비율 카드 끼리 간격에 해당
    private Coroutine coroutine;
    private Card currentCard;
    private readonly List<Card> outgoingCards = new List<Card>();
    private readonly HashSet<Card> enteringCards = new HashSet<Card>();
    private readonly Dictionary<Card, Vector2> layoutTargets = new Dictionary<Card, Vector2>();

    private void OnCardClosing(Card card)
    {
        if (card != currentCard) return;
        StopLayout();
        foreach (Card other in DisplayCards())
            if (other != card) other.HideAsBackground(card.animDuration);
    }

    private void OnCardClosed(Card card)
    {
        enteringCards.Remove(card);
        if (outgoingCards.Remove(card))
        {
            card.Closing -= OnCardClosing;
            card.Closed -= OnCardClosed;
            layoutTargets.Remove(card);
        }
        // Closing the display does not remove retained cards.
        if (card == currentCard)
        {
            currentCard = null;
            StopLayout();
            foreach (Card other in DisplayCards()) other.HideAsBackground(0f);
            return;
        }
        if (!isActiveAndEnabled || currentCard == null || currentCard.IsClosing) return;
        if (coroutine != null && layoutTargets.Count == 0) return;
        if (LayoutTargetsChanged()) RefreshCardList();
    }

    private Card[] DisplayCards()
    {
        return outgoingCards.Concat(cardList)
            .Where(card => card != null && card.IsDisplayed).ToArray();
    }

    private void StopLayout()
    {
        if (coroutine != null) StopCoroutine(coroutine);
        coroutine = null;
    }

    private Vector2 GetTargetPosition(Card card, int index, int count)
    {
        float offsetX = -card.Rect.rect.width * (ratio / 100f) * (count - 1 - index);
        return card.OriginalPosition + new Vector2(offsetX, 0f);
    }

    private bool LayoutTargetsChanged()
    {
        Card[] cards = DisplayCards();
        for (int i = 0; i < cards.Length; i++)
        {
            if (!layoutTargets.TryGetValue(cards[i], out Vector2 target) ||
                target != GetTargetPosition(cards[i], i, cards.Length))
                return true;
        }
        return false;
    }

    private void OnDisable()
    {
        StopLayout();
        currentCard = null;
        foreach (Card card in DisplayCards()) card.HideAsBackground(0f);
    }

    private void OnDestroy()
    {
        foreach (Card card in cardList.Concat(outgoingCards))
        {
            if (card == null) continue;
            card.Closing -= OnCardClosing;
            card.Closed -= OnCardClosed;
        }
        if (_instance == this) _instance = null;
    }

    #region 카드 추가
    public void AddCard(Card card)
    {
        if (!isActiveAndEnabled || card == null || limit <= 0 || cardList.Contains(card) || outgoingCards.Contains(card))
            return;

        card.Closing += OnCardClosing;
        card.Closed += OnCardClosed;
        if (!card.BeginEntrance(animDuration))
        {
            card.Closing -= OnCardClosing;
            card.Closed -= OnCardClosed;
            Debug.LogWarning("Cannot show a card under an inactive parent or with a disabled Card component.", card);
            return;
        }
        currentCard = card;
        cardList.RemoveAll(stored => stored == null);
        foreach (Card stored in cardList)
        {
            enteringCards.Remove(stored);
            stored.ShowAsBackground();
        }
        float entryOffset = card.Rect.rect.width * (ratio / 100f);
        card.Rect.anchoredPosition = card.OriginalPosition + new Vector2(entryOffset, 0f);
        enteringCards.Add(card);
        card.Rect.SetAsLastSibling();
        cardList.Add(card);
        RefreshCardList();
    }

    public void RefreshCardList()
    {
        StopLayout();
        layoutTargets.Clear();
        cardList.RemoveAll(card => card == null);
        outgoingCards.RemoveAll(card => card == null);
        enteringCards.RemoveWhere(card => card == null || !cardList.Contains(card));
        int maxCards = Mathf.Max(0, limit);
        while (cardList.Count > maxCards)
        {
            Card oldest = cardList[0];
            cardList.RemoveAt(0);
            enteringCards.Remove(oldest);
            if (oldest.IsDisplayed)
            {
                outgoingCards.Add(oldest);
                oldest.RemoveCard();
            }
            else
            {
                oldest.Closing -= OnCardClosing;
                oldest.Closed -= OnCardClosed;
            }
        }
        if (currentCard != null && !currentCard.IsClosing && DisplayCards().Length > 0)
            coroutine = StartCoroutine(RefreshCardCoroutine());
    }
    #endregion

    #region 카드 삭제
    public void HideCardList()
    {
        if (currentCard != null && currentCard.IsDisplayed)
            currentCard.RemoveCard();
        else
        {
            StopLayout();
            foreach (Card card in DisplayCards()) card.HideAsBackground(animDuration);
        }
    }

    // Legacy entry point: hide the presentation, keep retained history.
    public void RemoveCard() => HideCardList();
    private Card GetTopStoredCard()
    {
        for (int i = cardList.Count - 1; i >= 0; i--)
        {
            if (cardList[i] != null)
                return cardList[i];
        }

        return null;
    }

    public void CheckClearCard()
    {
        if (!isActiveAndEnabled)
            return;

        Card topCard = GetTopStoredCard();
        if (topCard == null)
            return;

        if (!topCard.CanStartClearCheck)
            return;

        topCard.ShowForLayout();
        if (!topCard.isActiveAndEnabled || !topCard.IsDisplayed)
            return;

        currentCard = topCard;
        topCard.PlayAnimation("In_Clear");
    }

    public void ShowTopCard()
    {
        CheckClearCard();
    }
    public void HideTopCard(Card card)
    {
        if (card == currentCard) HideCardList();
    }
    #endregion
    //Card에 있는 IEnumerator을 가져와서 여기에 종속시키기
    //맨 앞에 있는 카드만 가져와서 ClearCheck하는 기능을 추가
    //맨 앞에 있는 카드의 TodoList를 PauseMenu에 동기화 시키는 방식
    #region 코루틴 애니메이션
    private IEnumerator RefreshCardCoroutine()
    {
        // Let the UI layout settle before reading card widths.
        yield return null;

        Card[] cards = DisplayCards();
        Vector2[] startPositions = new Vector2[cards.Length];
        Vector2[] targetPositions = new Vector2[cards.Length];
        bool[] entering = new bool[cards.Length];
        for (int i = 0; i < cards.Length; i++)
        {
            Card card = cards[i];
            if (card == null)
                continue;

            startPositions[i] = card.Rect.anchoredPosition;
            entering[i] = enteringCards.Contains(card);
            targetPositions[i] = GetTargetPosition(card, i, cards.Length);
            layoutTargets[card] = targetPositions[i];
        }

        yield return UIAnimationRoutine.Run(
            animDuration,
            progress => progress * progress,
            t =>
            {
                for (int i = 0; i < cards.Length; i++)
                {
                    if (cards[i] != null && cards[i].IsDisplayed)
                    {
                        cards[i].Rect.anchoredPosition = Vector2.Lerp(
                            startPositions[i], targetPositions[i], t);
                    }
                }
            });

        coroutine = null;
        for (int i = 0; i < cards.Length; i++)
        {
            Card card = cards[i];
            if (card != null && !card.IsClosing && entering[i] && enteringCards.Remove(card))
                card.OnEntranceCompleted();
        }
    }

    #endregion 

}