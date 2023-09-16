using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace RoyalFortune21.BonusGame
{
    public class D3Dice : MonoBehaviour
    {
        [SerializeField] Color DisableColor;

        public int Type;
        public string ColorType;

        [SerializeField] Sprite[] diceImages;
        [SerializeField] GameObject ImageSpawnerPos;
        [SerializeField] GameObject DiceImagePrefab;

        [SerializeField] int Index;

        public bool isSelected { get; set; }
        public Button button { get; private set; }
        Image diceImage;
        Animator animator;

        private void Awake()
        {
            OnValidate();

            button.onClick.AddListener(delegate
            {
                if (BonusGameManager.instance.isJackpot)
                    OnD3DiceClickJackpot();
                else
                    OnD3DiceClick();
            });

            isSelected = false;
            Type = -1;
        }

        public void OnD3DiceClick(bool isFinlaChecking = false)
        {
            if (BonusGameManager.instance.D3DiceSelection)
            {
                BonusGameManager.instance.OnClickEnableD3Dices(Index);
            }
            else
            {
                Vector3 pos = BonusGameManager.instance.GetSpawnPosition(Type);

                if (pos != Vector3.zero)
                {
                    button.interactable = false;
                    isSelected = true;
                    SpawnDiceOnCard(pos);

                    if (!isFinlaChecking)
                    {
                        if (BonusGameManager.instance.isJackpot)
                            BonusGameManager.instance.CheckMatchedDiceCardsJackpot(0.25f);
                        else
                            BonusGameManager.instance.CheckMatchedDiceCards(0.25f);
                    }
                }
            }
        }

        public void OnD3DiceClickJackpot()
        {
            Vector3 pos = BonusGameManager.instance.GetD3DiceJackpotSpawnPostion(this);
            if (pos != Vector3.zero)
            {
                button.interactable = false;
                isSelected = true;
                SpawnDiceOnCard(pos);
            }
        }
        
        public void SpawnDiceOnCard(Vector3 pos)
        {
            StartCoroutine(SelectDice(pos));
        }

        IEnumerator SelectDice(Vector3 _pos)
        {
            animator.enabled = true;
            yield return new WaitForSeconds(1);
            animator.enabled = false;

            SpawnDiceImage(_pos);
        }

        void SpawnDiceImage(Vector3 _pos)
        {
            GameObject obj = Instantiate(DiceImagePrefab, ImageSpawnerPos.transform.position, DiceImagePrefab.transform.rotation);
            obj.transform.SetParent(ImageSpawnerPos.transform);
            obj.transform.localScale = new Vector3(0.42f, 0.42f, 0.42f);
            obj.GetComponent<SpriteRenderer>().sprite = diceImage.sprite;

            DiceImageMover moveTowards = obj.GetComponent<DiceImageMover>();

            if (moveTowards != null)
            {
                moveTowards.towardsPosition = _pos;
            }
        }

        public void SetButtonActive(bool _status)
        {
            ColorBlock color;
            if (_status)
            {
                color = button.colors;
                color.normalColor = Color.white;
                color.selectedColor = Color.white;
            }
            else
            {
                color = button.colors;
                color.normalColor = DisableColor;
                color.selectedColor = DisableColor;
            }
            button.colors = color;
        }

        void OnValidate()
        {
            button = GetComponent<Button>();
            diceImage = GetComponent<Image>();
            animator = GetComponent<Animator>();
        }

        public void RollDice()
        {
            if(isSelected)
            {
                return;
            }

            int rolledImage = ReturnRandomChar();
            SetImage(rolledImage);
            transform.GetChild(0).gameObject.SetActive(true);
            StartCoroutine(RollAnimOff());
        }

        public void Temp_SetRandomImage()
        {
            int rolledImage = ReturnRandomChar();
            SetImage(rolledImage);
        }

        void SetImage(int rolledType)
        {
            diceImage.sprite = diceImages[rolledType];

            string[] splitName = diceImages[rolledType].name.Split(' ');

            ColorType = splitName[0];
            Type = int.Parse(splitName[1]);
        }

        int ReturnRandomChar()
        {
            return Random.Range(0, diceImages.Length);
        }

        IEnumerator RollAnimOff()
        {
            yield return new WaitForSeconds(1f);
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}



/*
        Doubles 2x				2		(two of the same number)
        Two pair 4x				4		(two doubles of differing numbers)
        3 flush 5x				3		(all the same color)
        3 straight 10x			3		(1,2,3 in any color)
        Trips 15x				3		(3 of the same)
        4 flush 20x 			4		(all the same color)
        Fullhouse 25x 			5		(3 of a kind and 2 of a kind)
        3 straight flush 30x 	3		(1,2,3 in the same color)
        Royal set 35x 			3		(3 of the same number in the same suit)
        Quads 40x 				4		(4 of the same number)
        5 flush 45x 			5		(all the same color)
        Yahtzee 50x 			5		(5 of a kind)
        Royal house 75x			7		(4 + 3 of a kind)
        Double yahtzee 100x 	10		(5 of a kind in 2 differing numbers)
        10 of a kind 150x 		10		(10 of the same number)
*/