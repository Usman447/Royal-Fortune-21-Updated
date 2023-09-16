using System.Collections.Generic;
using System.Linq;
using RoyalFortune21.CardProperty;
using UnityEngine;

namespace RoyalFortune21.BonusGame
{
    public class BonusShuffleCard : MonoBehaviour
    {
        [SerializeField] Transform CardGrid;
        [SerializeField] CardsSO[] cardsSOArray;
        [SerializeField] Transform InitialSpawningPosition;

        List<Vector3> pos;
        int i = 0;
        int sortingOrderLayer = 0;
        int totalCards = 0;
        CardsSO cardsSO;

        private void Start()
        {
            cardsSO = BonusGameManager.instance.isChessCards ? cardsSOArray[0] : cardsSOArray[1];
        }

        public void StartGame()
        {
            pos = new List<Vector3>();
            foreach (Transform trans in CardGrid)
            {
                pos.Add(trans.position);
            }

            totalCards = CountCards();
            PassACard();
        }

        public void PassACard()
        {
            if (i < pos.Count)
            {
                SpawnCardBonus(pos[i]);
                i++;
            }
        }

        void SpawnCardBonus(Vector3 toPos)
        {
            if (totalCards <= 12)
                cardsSO.ResetCards(CountSceneCards(FindObjectsOfType<Card>().ToList()));

            bool isCardSpawn = false;
            do
            {
                int cardType = Random.Range(0, cardsSO.Cards.Length);
                if (cardsSO.Cards[cardType].IsCardAvailable)
                {
                    GameObject card = Instantiate(cardsSO.Cards[cardType].Card, InitialSpawningPosition.position, Quaternion.identity);
                    card.transform.SetParent(transform);
                    card.GetComponent<SpriteRenderer>().sortingOrder = sortingOrderLayer;
                    sortingOrderLayer++;

                    Card _card = card.GetComponent<Card>();
                    _card.enabled = true;

                    _card.cardMovement.MoveTowardsFixed(toPos);
                    _card.cardMovement.OnReached += () => BonusGameManager.instance.AddCardInList(_card);
                    isCardSpawn = true;
                }
            } while (!isCardSpawn);
            totalCards--;
        }

        Dictionary<string, int> CountSceneCards(List<Card> _sceneCards)
        {
            Dictionary<string, int> _counter = new Dictionary<string, int>();
            foreach (Card card in _sceneCards)
                if (_counter.ContainsKey(card.name))
                    _counter[card.name]++;
                else
                    _counter.Add(card.name, 1);
            return _counter;
        }

        int CountCards()
        {
            totalCards = 0;
            foreach (var card in cardsSO.Cards)
                totalCards += card.CardsCount;
            return totalCards;
        }
    }
}
    