using UnityEngine;

namespace RoyalFortune21.CardProperty
{
    [System.Serializable]
    public class CardProperties
    {
        CardProperties()
        {
            SetCardCount(3);
        }

        public bool isEmpty
        {
            get
            {
                if (CardsCount > 0)
                    return false;
                return true;
            }
        }

        public bool IsCardAvailable
        {
            get
            {
                if (CardsCount <= 0)
                    return false;
                CardsCount--;
                return true;
            }
        }

        public void SetCardCount(int _count)
        {
            CardsCount = _count;
        }

        public void Print()
        {
            Debug.Log(Card.name + " " + CardsCount);
        }

        public GameObject Card;
        public int m_CardsCount;

        public int CardsCount
        {
            get { return m_CardsCount; }
            set { m_CardsCount = value; }
        }
    }
}
    