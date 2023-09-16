using EditorColor;
using UnityEngine;

namespace RoyalFortune21
{
    public class GridPoint : MonoBehaviour
    {
        [SerializeField] int m_Index = 0;

        bool m_isOccupied;
        public bool IsOccupied
        {
            get { return m_isOccupied; }
            set
            {
                m_isOccupied = value;
                m_BoxCollider.enabled = !m_isOccupied;
                m_SpriteRenderer.enabled = !m_isOccupied;
            }
        }
        BoxCollider2D m_BoxCollider;
        SpriteRenderer m_SpriteRenderer;

        private void Awake()
        {
            OnValidate();
        }

        private void OnValidate()
        {
            m_BoxCollider = GetComponent<BoxCollider2D>();
            m_SpriteRenderer = GetComponent<SpriteRenderer>();
        }

        public int Index
        {
            get => m_Index;
        }

        public void Print(string _cardName)
        {
            print((Index + " " + _cardName).ToString() % Colorize.Gold);
        }
    }
}
    