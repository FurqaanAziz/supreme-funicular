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

        public TMP_Text comboText;
        public GameObject comboObject;

        private int matchesFound = 0;

        private int comboStreak = 0;
        private int comboMultiplier = 1;

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
                comboStreak++;
                comboMultiplier = comboStreak >= 2 ? comboStreak : 1;

                matchesFound += comboMultiplier;

                matchedPairs++;
                matchedCardIds.Add(first.id);

                PlayAudio(cardMatchedAudio);
                UpdateScoreText();
                CheckForGameCompletion();
                ShowComboText(comboMultiplier > 1 ? $"Combo x{comboMultiplier}!" : "");
                first.Notify(first, CardEvent.Matched);
                second.Notify(second, CardEvent.Matched);
            }
            else
            {
                comboStreak = 0;
                comboMultiplier = 1;
                ShowComboText("");
                PlayAudio(cardMismatchedAudio);

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
                scoreText.text = comboMultiplier > 1
                    ? $"Match Score: {matchesFound} (x{comboMultiplier} Combo!)"
                    : $"Match Score: {matchesFound}";
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
            matchesFound = 0;
            matchedPairs = 0;
            comboStreak = 0;
            comboMultiplier = 1;
            matchedCardIds.Clear();
            cardClickQueue.Clear();
            isComparing = false;
            isGameCompleted = false;

            UpdateScoreText();
            ShowComboText("");
            if (gameCompletedPanel != null)
                gameCompletedPanel.SetActive(false);

            GridManager gridManager = FindObjectOfType<GridManager>();
            if (gridManager != null)
            {
                gridManager.RestartGame();
            }
        }

        public int GetScore() => matchedPairs;

        public List<int> GetMatchedCardIds() => matchedCardIds;

        public void RestoreGameState(SaveData data)
        {
            matchedPairs = data.score;
            isGameCompleted = data.isGameCompleted;

            var uniqueMatchedIds = new HashSet<int>(GetMatchedCardIds());
            matchedPairs = uniqueMatchedIds.Count;

            UpdateScoreText();

            if (matchedPairs == FindObjectsOfType<Card>().Length / 2)
            {
                isGameCompleted = true;
                ShowGameCompletedPanel();
            }
        }

        private void ShowComboText(string message)
        {
            if (comboText != null)
            {
                if (!string.IsNullOrEmpty(message))
                {
                    comboObject.SetActive(true);
                    comboText.text = message;
                    CancelInvoke(nameof(ClearComboText));
                    Invoke(nameof(ClearComboText), 1.5f);
                }
                else
                {
                    comboObject.SetActive(false);
                }
            }
        }

        private void ClearComboText()
        {
            if (comboText != null)
            {
                comboText.text = "";
                comboObject.SetActive(false);
            }
        }
    }
}
