using RoyalFortune21.BonusGame;
using UnityEngine;

namespace RoyalFortune21.CardProperty
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Card : MonoBehaviour
    {
        public string Suit;
        public string Type;
        public GridPoint Grid { get; set; } = null;

        public bool isMatched { get; set; }
        public bool isInteractable { get; set; }

        public BoxCollider2D boxCollider { get; private set; }
        public CardMovement cardMovement { get; private set; }

        bool isSelected = false;

        private void Awake()
        {
            OnValidate();
            this.Grid = null;
            isMatched = false;
            isInteractable = true;
            enabled = false;
        }

        private void OnValidate()
        {
            boxCollider = GetComponent<BoxCollider2D>();
            cardMovement = GetComponent<CardMovement>();
        }

        public void PlayDiceAnim()
        {
            this.transform.GetChild(0).gameObject.SetActive(true);
        }

        private void Update()
        {
            if (!Input.GetMouseButtonDown(0))
                return;

            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider == boxCollider)
            {
                if (isInteractable)
                {
                    if (GameManager.instance != null)
                    {
                        if (!isSelected)
                            GameManager.instance.SelectCard(this);
                        else
                            GameManager.instance.DeselectCard(this);
                        isSelected = !isSelected;
                    }
                }
                else if (!isMatched)
                {
                    if (BonusGameManager.instance != null)
                    {
                        if (BonusGameManager.instance.isJackpot)
                            BonusGameManager.instance.CheckSelectedDiceWithCards(this);
                        else
                            BonusGameManager.instance.CompareCardWithSelectedDice(this);
                    }
                    else
                    {
                        GameManager.instance.CompareCardWithDice(this);
                    }
                }
            }
        }
    }
}
    