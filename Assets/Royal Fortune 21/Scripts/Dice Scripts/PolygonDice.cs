using MPUIKIT;
using RoyalFortune21.BonusGame;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace RoyalFortune21
{
    public class PolygonDice : MonoBehaviour
    {
        public int Type { get; set; }
       
        public int MaxNum { get; set; }

        [SerializeField] Sprite[] diceImages;
        [SerializeField] Image diceImage;
        [SerializeField] GameObject AnimPanel;

        public bool isSelected { get; set; }
        public Button button;
        public Animator animator;

        private void Awake()
        {
            button.onClick.AddListener(delegate
            {
                OnClickDice();
            });

            isSelected = false;
            Type = -1;

            onNumber = Random.Range(0, 3);
        }

        int onNumber = 0;
        int rollingNumber = 0;

        public void OnClickDice()
        {
            RollDice();
        }

        public bool isForceValue = false;

        public void RollDice()
        {
            int rolledImage = ReturnRandomChar();
            if (isForceValue)
            {
                if (rollingNumber == onNumber)
                    SetImage(BonusGameManager.instance.cards.Count - 1);
                else
                    SetImage(rolledImage);
                rollingNumber++;
            }
            else
            {
                SetImage(rolledImage);
            }

            AnimPanel.gameObject.SetActive(true);
            animator.enabled = true;
            StartCoroutine(RollAnimOff());
        }

        void SetImage(int rolledType)
        {
            diceImage.sprite = diceImages[rolledType];
            Type = rolledType + 1;
        }

        int ReturnRandomChar()
        {
            return Random.Range(0, diceImages.Length);
        }

        IEnumerator RollAnimOff()
        {
            yield return new WaitForSeconds(1f);
            AnimPanel.gameObject.SetActive(false);
            animator.enabled = false;
            BonusGameManager.instance.OnRollPolygonDice(Type, MaxNum);
        }
    }
}
        