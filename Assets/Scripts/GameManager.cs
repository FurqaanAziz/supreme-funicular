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

        public GameObject gameCompletedPanel;
        public AudioClip cardFlippedAudio;
        public AudioClip cardMatchedAudio;
        public AudioClip cardMismatchedAudio;
        public AudioClip gameCompletedAudio;

        private AudioSource audioSource;

        public TMP_Text scoreText;
        private void Start()
        {
            UpdateScoreText();
            audioSource = GetComponent<AudioSource>();
        }
        public void CardClicked(Card clickedCard)
        {
            if (clickedCard == null || clickedCard.isFaceUp || cardClickQueue.Contains(clickedCard))
                return;
            PlayAudio(cardFlippedAudio);
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

                PlayAudio(cardMatchedAudio);
                UpdateScoreText();
                CheckForGameCompletion();

                first.Notify(first, CardEvent.Matched);
                second.Notify(second, CardEvent.Matched);
            }
            else
            {
                first.Notify(first, CardEvent.Mismatched);
                second.Notify(second, CardEvent.Mismatched);
                PlayAudio(cardMismatchedAudio);
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
                ShowGameCompletedPanel();
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

        private void PlayAudio(AudioClip clip)
        {
            if (audioSource != null && clip != null)
            {
                audioSource.PlayOneShot(clip);
            }
        }

        private void UpdateScoreText()
        {
            if (scoreText != null)
            {
                scoreText.text = $"Match Score: {matchedPairs}";
            }
        }
       
        private void ShowGameCompletedPanel()
        {
            if (gameCompletedPanel != null)
            {
                gameCompletedPanel.SetActive(true);
                PlayAudio(gameCompletedAudio);
            }
        }

        public void ResetGame()
        {
            matchedPairs = 0;
            matchedCardIds.Clear();
            cardClickQueue.Clear();
            isComparing = false;
            isGameCompleted = false;

            UpdateScoreText();

            if (gameCompletedPanel != null)
                gameCompletedPanel.SetActive(false);

            GridManager gridManager = FindObjectOfType<GridManager>();
            if (gridManager != null)
            {
                gridManager.RestartGame();
            }
        }
    }
}
