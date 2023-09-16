using RoyalFortune21.CardProperty;
using System.Collections;
using UnityEngine;

namespace RoyalFortune21
{
    public class Table : MonoBehaviour
    {
        [SerializeField] DeckCardSpawner deckCard;
        [SerializeField] bool m_AutoSpawn = true;

        private void Start()
        {
            m_AutoSpawn = GameManager.instance.AutogenerateOrNot();
        }

        public void SetDeckActive()
        {
            if (!m_AutoSpawn)
            {
                deckCard.enabled = true;
            }
            else
            {
                deckCard.enabled = false;
                deckCard.gameObject.SetActive(false);
            }
        }

        public void TableIsReady()
        {
            if (!m_AutoSpawn)
            {
                return;
            }
            StartCoroutine(WaitForSomeTime());
        }

        IEnumerator WaitForSomeTime()
        {
            yield return new WaitForSeconds(0.5f);
            FindObjectOfType<ShuffleCards>().StartShufflingCards();
        }
    }
}
    