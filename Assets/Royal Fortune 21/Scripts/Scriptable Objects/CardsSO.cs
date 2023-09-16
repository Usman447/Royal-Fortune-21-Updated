using UnityEngine;
using RoyalFortune21.CardProperty;
using System.Collections.Generic;

namespace RoyalFortune21
{
    [CreateAssetMenu]
    public class CardsSO : ScriptableObject
    {
        public CardProperties[] Cards;

        public void ResetCards(Dictionary<string, int> _sceneCardCounter)
        {
            foreach(CardProperties cardprop in Cards)
            {
                Card card = cardprop.Card.GetComponent<Card>();
                string cardName = card.Suit + " " + card.Type;

                int scene = 0;
                if (_sceneCardCounter.ContainsKey(cardName))
                {
                    scene = _sceneCardCounter[cardName];
                }
                int countResult = GetAddedNumber(scene, cardprop.CardsCount);
                cardprop.SetCardCount(countResult);
            }
        }

        int GetAddedNumber(int _sceneCount, int _backendCount)
        {
            int result = 3 - _sceneCount;
            int finalResult = result - _backendCount;
            finalResult += _backendCount;
            return finalResult;
        }
    }
}
    