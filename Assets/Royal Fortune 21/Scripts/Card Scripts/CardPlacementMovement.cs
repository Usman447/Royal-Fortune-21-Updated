using UnityEngine;
using System.Collections;
using System;
using UnityEditor;
using EditorColor;

namespace RoyalFortune21.CardProperty
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class CardPlacementMovement : MonoBehaviour
    {
        public LayerMask gridPositionMask;
        [SerializeField] float CardMoveSmoothness = 5f;

        public GameObject selectedObject { get; set; }

        BoxCollider2D boxCollider;
        Card card;
        Vector3 dampSmoothRef;
        bool isCardDrag = false;
        bool isSelected = false;

        private void Awake()
        {
            OnValidate();
        }

        private void OnValidate()
        {
            boxCollider = GetComponent<BoxCollider2D>();
            card = GetComponent<Card>();
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
                Collider2D targetObject = Physics2D.OverlapPoint(touchPosition);

                if (targetObject == boxCollider)
                {
                    GameManager.instance.OnClickCard();
                    selectedObject = targetObject.transform.gameObject;
                }
                isCardDrag = false;
            }

            if (touch.phase == TouchPhase.Moved)
            {
                MoveCard(touchPosition);
            }

            if (touch.phase == TouchPhase.Ended && selectedObject)
            {
                selectedObject = null;

                if (card.Grid == null)
                {
                    GridPoint grid = GameManager.instance.GetAvailableGridPoint();
                    try
                    {
                        CardGridSetup(grid);
                    }
                    catch (ArgumentNullException e)
                    {
                        Debug.LogError(e.Message);
                    }
                }

                if(GameManager.instance.nikel || GameManager.instance.quarter
                    || GameManager.instance.dollar)
                    SelectAndDeselectCard();

                GameManager.instance.OnLeaveCard();
                StartCoroutine(WaitForEndOfFrame());
            }
        }

        IEnumerator WaitForEndOfFrame()
        {
            yield return new WaitForEndOfFrame();
            if (Vector3.Distance(card.transform.position, card.cardMovement.currentPosition) != 0)
            {
                card.cardMovement.MoveTowards(card.cardMovement.currentPosition);
            }
        }

        void SelectAndDeselectCard()
        {
            if (!isCardDrag)
            {
                if (!isSelected)
                    GameManager.instance.SelectCardDiscarded(card);
                else
                    GameManager.instance.DeselectCardDiscarded(card);
                isSelected = !isSelected;
            }
        }

        void CardGridSetup(GridPoint _grid)
        {
            if (_grid == null)
                throw new ArgumentNullException();

            _grid.IsOccupied = true;
            card.Grid = _grid;
            card.cardMovement.currentPosition = _grid.transform.position;
        }

        void MoveCard(Vector3 _touchPosition)
        {
            if(selectedObject && !isSelected &&
                (GameManager.instance.halfDollar || GameManager.instance.dollar ||
                GameManager.instance.dime || GameManager.instance.quarter))
            {
                transform.position = Vector3.SmoothDamp(transform.position, 
                    _touchPosition, ref dampSmoothRef, CardMoveSmoothness * Time.deltaTime);
                isCardDrag = true;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (selectedObject)
            {
                if (collision.tag == "Card" && 
                    (GameManager.instance.halfDollar || GameManager.instance.dollar))
                {
                    Card otherCard = collision.GetComponent<Card>();
                    Swap(otherCard);

                    otherCard.cardMovement.MoveTowards(otherCard.cardMovement.currentPosition);
                }
                else if (collision.tag == "Grid Point")
                {
                    GridPoint grid = collision.GetComponent<GridPoint>();
                    try
                    {
                        if(card.Grid != null)
                        {
                            card.Grid.IsOccupied = false;
                            card.Grid = null;
                        }

                        if(card.Grid == null)
                            CardGridSetup(grid);
                    }
                    catch (ArgumentNullException e)
                    {
                        Debug.LogError(e.Message);
                    }
                }
            }
        }

        void Swap(Card _otherCard)
        {
            Vector3 otherCardCurrentPosition = _otherCard.cardMovement.currentPosition;
            GridPoint otherCardGridIndex = _otherCard.Grid;

            _otherCard.Grid = card.Grid;
            _otherCard.cardMovement.currentPosition = card.cardMovement.currentPosition;

            card.Grid = otherCardGridIndex;
            card.cardMovement.currentPosition = otherCardCurrentPosition;
        }
    }
}
    