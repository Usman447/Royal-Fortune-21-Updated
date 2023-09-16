using RoyalFortune21.CardProperty;
using UnityEngine;

namespace RoyalFortune21
{
    public class DeckCardSpawner : MonoBehaviour
    {
        GameObject m_selectedObject;
        BoxCollider2D m_boxCollider;

        private void Awake()
        {
            m_boxCollider = GetComponent<BoxCollider2D>();
        }

        private void Update()
        {
            if (Input.touchCount != 1)
                return;

            Touch touch = Input.GetTouch(0);

            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
            touchPosition.z = 0;

            if (touch.phase == TouchPhase.Began)
            {
                if(FindObjectsOfType<Card>().Length == 9)
                {
                    return;
                }

                Collider2D targetObject = Physics2D.OverlapPoint(touchPosition);

                if (targetObject == m_boxCollider)
                {
                    FindObjectOfType<ShuffleCards>().SpawnCardFromDeck(transform.position);
                    m_selectedObject = targetObject.transform.gameObject;
                }
            }

            if (touch.phase == TouchPhase.Ended && m_selectedObject)
            {
                m_selectedObject = null;
            }
        }
    }
}
    