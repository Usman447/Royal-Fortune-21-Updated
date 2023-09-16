using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using EditorColor;

namespace RoyalFortune21.CardProperty
{
    public class ShuffleCards : MonoBehaviour
    {
        [SerializeField] Transform CardGrid;
        [SerializeField] CardsSO[] cardsSOArray;

        [SerializeField] Transform InitialSpawningPosition;
        [SerializeField] float TimeDelayBetweenCards = .5f;
        [SerializeField] float CardMovementSpeed = 5f;
        
        
        CardsSO cardsSO;
        int sortingOrderLayer = 0;
        int totalCards = 0;

        private void Start()
        {
            cardsSO = GameManager.instance.isChessCards ? cardsSOArray[0] : cardsSOArray[1];

            foreach (CardProperties card in cardsSO.Cards)
            {
                card.CardsCount = 3;
            }

            totalCards = CountCards();
        }

        public void StartShufflingCards()
        {
            sortingOrderLayer = 0;
            cardsSO.ResetCards(CountSceneCards(FindObjectsOfType<Card>().ToList()));

            foreach (var card in GetComponentsInChildren<Card>())
            {
                Destroy(card.gameObject);
            }

            StartCoroutine(ShuffleCardsOnGrid());
        }

        IEnumerator ShuffleCardsOnGrid()
        {
            foreach (Transform toPos in CardGrid)
            {
                SpawnCardOnGrid(toPos.position, toPos.GetComponent<GridPoint>());
                yield return new WaitForSeconds(TimeDelayBetweenCards);
            }
        }

        public void SpawnCardOnGrid(Vector3 _toPos, GridPoint _gridPoint)
        {
            Card card = SpawnCard(InitialSpawningPosition.position);

            _gridPoint.IsOccupied = true;
            card.Grid = _gridPoint;

            card.cardMovement.MoveTowards(_toPos, CardMovementSpeed);
            card.cardMovement.currentPosition = _toPos;

            GameManager.instance.CardPropertyOnGameSelected(card);
            GameManager.instance.CardsOnGrid++;
        }


        public void SpawnCardFromDeck(Vector3 _spawnPosition)
        {
            Card card = SpawnCard(_spawnPosition);

            CardPlacementMovement cardPlacement = card.GetComponent<CardPlacementMovement>();
            cardPlacement.selectedObject = card.gameObject;
            cardPlacement.enabled = true;

            GameManager.instance.CardPropertyOnGameSelected(card);
            GameManager.instance.CardsOnGrid++;
        }

        Card SpawnCard(Vector3 _spawnPosition)
        {
            if (totalCards <= 12)
            {
                cardsSO.ResetCards(CountSceneCards(FindObjectsOfType<Card>().ToList()));
                totalCards = CountCards();
            }

            int spawnCardCounter = 0;
            do
            {
                int cardType;
                if (spawnCardCounter >= 5)
                    cardType = ForceSpawnCard();
                else
                    cardType = Random.Range(0, cardsSO.Cards.Length);

                if (cardsSO.Cards[cardType].IsCardAvailable)
                {
                    GameObject card = Instantiate(cardsSO.Cards[cardType].Card, _spawnPosition, Quaternion.identity);
                    card.transform.SetParent(transform);

                    card.GetComponent<SpriteRenderer>().sortingOrder = sortingOrderLayer;
                    totalCards--;
                    Card cardScript = card.GetComponent<Card>();
                    card.name = cardScript.Suit + " " + cardScript.Type;

                    return cardScript;
                }
                spawnCardCounter++;
            } while (true);
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

        int lastIndex = 0;
        int ForceSpawnCard()
        {
            for (int i = lastIndex; i < cardsSO.Cards.Length; i++, lastIndex++)
            {
                if (!cardsSO.Cards[i].isEmpty)
                {
                    return i;
                }
            }
            return -1;
        }

        int CountCards()
        {
            int count = 0;
            foreach (var card in cardsSO.Cards)
                count += card.CardsCount;
            return count;
        }
    }
}
    