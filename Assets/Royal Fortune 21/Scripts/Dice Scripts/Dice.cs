using EditorColor;
using RoyalFortune21.BonusGame;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace RoyalFortune21
{
    public class Dice : MonoBehaviour
    {
        [SerializeField] Sprite[] chessDiceImages;
        [SerializeField] Sprite[] pinoDiceImages;

        [SerializeField] GameObject ImageSpawnerPos;
        [SerializeField] GameObject DiceImagePrefab;
        [SerializeField][Range(-1, 5)] int ForceDiceNum = -1;


        public string Type { get; set; }
        public Button button { get; private set; }
        public bool isSelected {  get; set; }

        Sprite[] diceImages;
        Image diceImage;
        Animator animator;
        bool isDiceClicked = true;
        bool isDiceSelected = true;

        private void Awake()
        {
            Initialize();

            button.onClick.AddListener(() =>
            {
                if (GameManager.instance != null)
                {
                    OnClickDice();
                }
                else
                {
                    if (BonusGameManager.instance.isJackpot)
                        OnClickDiceJackpot();
                    else
                        OnClickDice();
                }
            });
        }

        private void Start()
        {
            if (GameManager.instance != null)
            {
                diceImages = GameManager.instance.isChessCards ? chessDiceImages : pinoDiceImages;
            }
            else if(BonusGameManager.instance != null)
            {
                diceImages = BonusGameManager.instance.isChessCards ? chessDiceImages : pinoDiceImages;
            }

            SetImage(diceImages[Random.Range(0, diceImages.Length)]);
        }

        void Initialize()
        {
            diceImage = GetComponent<Image>();
            button = GetComponent<Button>();
            animator = GetComponent<Animator>();
            isSelected = false;
            isDiceClicked = true;
            isDiceSelected = true;
            Type = null;
        }

        public void OnClickDiceJackpot()
        {
            if (Type == null) return;

            if (isDiceSelected)
            {
                BonusGameManager.instance.SelectDice(this);
            }
            else
            {
                BonusGameManager.instance.DeSelectDice();
            }
            isDiceSelected = !isDiceSelected;
        }

        public void OnClickDiceJackpotFinalCheck()
        {
            Vector3 pos = Vector3.zero;
            BonusGameManager.instance.CheckFinalTime(this);

            if (pos != Vector3.zero)
            {
                SpawnDiceOnCard(pos);
            }
        }

        public void SetImage(Sprite _sprite)
        {
            diceImage.sprite = _sprite;
        }

        public void OnClickDice()
        {
            if (Type == null ||
                Type != "Ace") 
                return;

            if (GameManager.instance != null)
            {
                if (isDiceClicked)
                    GameManager.instance.SelectDice(this);
                else
                    GameManager.instance.DeselectDice();
                isDiceClicked = !isDiceClicked;
            }
            else if(BonusGameManager.instance != null)
            {
                if (isDiceClicked)
                    BonusGameManager.instance.SelectDice(this);
                else
                    BonusGameManager.instance.DeSelectDice();
                isDiceClicked = !isDiceClicked;
            }
        }

        public void CompareDice(bool isFinalCheck = false)
        {
            Vector3 pos = GameManager.instance.CompareDiceWithCards(this);

            print("Dice Checking with card Done" % Colorize.Gold);
            if (pos != Vector3.zero)
            {
                SpawnDiceOnCard(pos);
            }
            else if (isFinalCheck)
            {
                pos = GameManager.instance.CompareAceDiceWithCards(this);

                if (pos != Vector3.zero)
                {
                    SpawnDiceOnCard(pos);
                }
            }
        }

        public void CompareDiceBonus(bool isFinalCheck = false)
        {
            Vector3 pos = BonusGameManager.instance.CompareDiceWithCards(this);

            if (pos != Vector3.zero)
            {
                SpawnDiceOnCard(pos);
            }
            else if (isFinalCheck)
            {
                pos = BonusGameManager.instance.CompareAceDiceWithCards(this);

                if (pos != Vector3.zero)
                {
                    SpawnDiceOnCard(pos);
                }
            }
        }

        public void SpawnDiceOnCard(Vector3 pos)
        {
            StartCoroutine(SpawnDice(pos));
        }

        public void RollDice()
        {
            if (isSelected)
                return;

            int rolledImageNum;
            if (ForceDiceNum == -1)
                rolledImageNum = Random.Range(0, diceImages.Length);
            else
                rolledImageNum = ForceDiceNum;

            SetImage(rolledImageNum);

            transform.GetChild(0).gameObject.SetActive(true);
            Invoke(nameof(RollAnimOff), 0.75f);
        }

        void SetImage(int rolledType)
        {
            diceImage.sprite = diceImages[rolledType];
            Type = diceImage.sprite.name;
        }

        void RollAnimOff()
        {
            transform.GetChild(0).gameObject.SetActive(false);

            if (GameManager.instance != null)
                CompareDice();
            else
                CompareDiceBonus();
        }

        IEnumerator SpawnDice(Vector3 _pos)
        {
            animator.enabled = true;
            yield return new WaitForSeconds(0.5f);
            animator.enabled = false;

            SpawnDiceImage(_pos);
        }

        void SpawnDiceImage(Vector3 _pos)
        {
            GameObject obj = Instantiate(DiceImagePrefab, ImageSpawnerPos.transform.position, DiceImagePrefab.transform.rotation);
            obj.GetComponent<SpriteRenderer>().sprite = diceImage.sprite;

            DiceImageMover moveTowards = obj.GetComponent<DiceImageMover>();

            if (moveTowards != null)
            {
                moveTowards.towardsPosition = _pos;
            }
        }
    }
}
    