using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

namespace CardGame
{
    public class GameManager : MonoBehaviour, IObserver
    {
        private Queue<Card> cardClickQueue = new Queue<Card>();
        private bool isComparing = false;

        private int matchedPairs = 0;
        private bool isGameCompleted = false;

        private List<int> matchedCardIds = new List<int>();

        public void CardClicked(Card clickedCard)
        {
            if (clickedCard == null || clickedCard.isFaceUp || cardClickQueue.Contains(clickedCard))
                return;

            cardClickQueue.Enqueue(clickedCard);
            TryProcessQueue();
        }

        private void TryProcessQueue()
        {
            if (isComparing || cardClickQueue.Count < 2)
                return;

            Card first = cardClickQueue.Dequeue();
            Card second = cardClickQueue.Dequeue();

            StartCoroutine(CheckCards(first, second));
        }

        private IEnumerator CheckCards(Card first, Card second)
        {
            isComparing = true;

            if (!first.isFaceUp) first.Flip();
            if (!second.isFaceUp) second.Flip();

            yield return new WaitForSeconds(0.5f);

            if (first.id == second.id)
            {
                matchedPairs++;
                matchedCardIds.Add(first.id);

                CheckForGameCompletion();

                first.Notify(first, CardEvent.Matched);
                second.Notify(second, CardEvent.Matched);
            }
            else
            {
                first.Notify(first, CardEvent.Mismatched);
                second.Notify(second, CardEvent.Mismatched);

                yield return new WaitForSeconds(0.5f);

                first.Flip();
                second.Flip();
            }

            isComparing = false;
            TryProcessQueue();
        }


        private void CheckForGameCompletion()
        {
            int totalPairs = FindObjectsOfType<Card>().Length / 2;
            if (matchedPairs == totalPairs)
            {
                isGameCompleted = true;
                Debug.Log("Game Completed");
            }
        }

        public void OnNotify(Card card, CardEvent cardEvent)
        {
            switch (cardEvent)
            {
                case CardEvent.Matched:
                    break;
                case CardEvent.Mismatched:
                    break;
            }
        }

        public bool IsGameCompleted() => isGameCompleted;

    }
}
