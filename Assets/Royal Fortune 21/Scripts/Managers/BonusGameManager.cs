using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using EditorColor;
using UnityEngine.UI;
using TMPro;
using RoyalFortune21.CardProperty;
using Random = UnityEngine.Random;
using RoyalFortune21.Audio;

namespace RoyalFortune21.BonusGame
{
    public class BonusGameManager : MonoBehaviour
    {
        public bool isChessCards = false;

        [SerializeField] Button DiceRollButton;
        [SerializeField] TextMeshProUGUI RollCountText;
        [SerializeField] GameObject CoinSelectionPanel;
        [SerializeField] GameObject ChessDiceAnim;
        [SerializeField] GameObject D3DiceAnim;
        [SerializeField] Transform ChessDiceTrans;
        [SerializeField] Transform D3DiceTrans;
        [SerializeField] Transform PolygonDiceTrans;
        [SerializeField] TextMeshProUGUI BalanceText;

        int m_rollsAllowed;
        public int RollsAllowed
        {
            get => m_rollsAllowed;
            set
            {
                m_rollsAllowed = value;
                RollCountText.text = ((m_rollsAllowed > 1) ? "Rolls" : "Roll") + " Available: " + m_rollsAllowed;
            }
        }

        public bool D3DiceSelection { get; set; }

        public bool isJackpot { get; private set; }

        public static BonusGameManager instance;

        public List<Card> cards { get; private set; }
        List<Card> selectedCards { get; set; }
        List<Dice> jackpotDice { get; set; }
        List<D3Dice> jackpotD3Dice { get; set; }

        List<PolygonDice> polygonDices { get; set; }
        List<D3Dice> selectedD3Dices { get; set; }
        BonusShuffleCard shuffleCards { get; set; }
        List<Dice> chessDices { get; set; }
        List<D3Dice> d3Dices { get; set; }

        int diceCount { get; set; } = 0;
        int d3DiceCount { get; set; } = 0;
        int polygonDiceCount { get; set; } = 0;
        int polygonDiceRollingCount { get; set; } = 0;
        int D3Score { get; set; } = 0;
        string coinType { get; set; }

        bool isCombinationMake = false;
        bool isFirstClick = true;

        string CashBalance
        {
            get
            {
                return PlayerPrefs.GetString("Cash Balance", "500.00");
            }
            set
            {
                string val = value;
                if (!val.Equals(""))
                {
                    PlayerPrefs.SetString("Cash Balance", val);
                }
                BalanceText.text = "$" + PlayerPrefs.GetString("Cash Balance", "500.00");
            }
        }

        void Awake()
        {
            if (PlayerPrefs.GetString("SelectedCardType", "Non").Equals("ChessCards"))
                isChessCards = true;
            else if (PlayerPrefs.GetString("SelectedCardType", "Non").Equals("TraditionalCards"))
                isChessCards = false;
            else
            {
                Debug.LogWarning("Nothing is selected");
                isChessCards = false;
            }

            Application.targetFrameRate = 60;
            instance = this;
            Initialization();
        }

        void Initialization()
        {
            cards = new List<Card>();
            selectedCards = new List<Card>();
            polygonDices = new List<PolygonDice>();
            chessDices = new List<Dice>();
            d3Dices = new List<D3Dice>();
            
            selectedD3Dices = new List<D3Dice>();
            D3DiceSelection = true;

            shuffleCards = FindObjectOfType<BonusShuffleCard>();

            isWon = false;
            jackpotCount = 0;

            isJackpot = false;

            jackpotDice = new List<Dice>();
        }

        private void Start()
        {
            foreach (Transform chessDiceTrans in ChessDiceTrans)
            {
                chessDices.Add(chessDiceTrans.GetComponentInChildren<Dice>());
            }

            foreach (Transform d3DiceTrans in D3DiceTrans)
            {
                d3Dices.Add(d3DiceTrans.GetComponentInChildren<D3Dice>());
            }

            foreach (Transform poly in PolygonDiceTrans)
            {
                PolygonDice _polygonDice = poly.GetComponent<PolygonDice>();
                _polygonDice.MaxNum = Convert.ToInt32(_polygonDice.name);
                _polygonDice.button.interactable = false;
                polygonDices.Add(_polygonDice);
            }

            CashBalance = "";
        }

        bool isFirstDiceSpawn = true;
        int D3DiceIndex = 0;
        int multiplerVal = 1;

        public Vector3 GetSpawnPosition(int _score)
        {
            if (CheckScore(_score))
            {
                Vector3 targetPos = new Vector3(0, -4.4f, 0) + (D3DiceIndex * (multiplerVal * 2)) * new Vector3(1, 0, 0);
                if (isFirstDiceSpawn)
                {
                    multiplerVal = 1;
                    D3DiceIndex++;
                }
                else
                {
                    multiplerVal = -1;
                }
                isFirstDiceSpawn = !isFirstDiceSpawn;

                return targetPos;
            }
            return Vector3.zero;
        }

        public Vector3 GetD3DiceJackpotSpawnPostion(D3Dice _d3Dice)
        {
            if (_d3Dice.Type == -1)
                return Vector3.zero;

            jackpotD3Dice.Add(_d3Dice);

            Vector3 targetPos = new Vector3(0, -4.4f, 0) + (D3DiceIndex * (multiplerVal * 2)) * new Vector3(1, 0, 0);
            if (isFirstDiceSpawn)
            {
                multiplerVal = 1;
                D3DiceIndex++;
            }
            else
            {
                multiplerVal = -1;
            }
            isFirstDiceSpawn = !isFirstDiceSpawn;

            return targetPos;
        }

        bool CheckScore(int _score)
        {
            int _counter = 0;
            for (int i = 0; i < selectedD3Dices.Count; i++)
            {
                if (!selectedD3Dices[i].isSelected)
                {
                    _counter++;
                }
            }

            if (_counter == 1)
            {
                if (_score + D3Score < cards.Count)
                {
                    return false;
                }
            }

            if (_score + D3Score <= cards.Count)
            {
                D3Score += _score;
                return true;
            }
            return false;
        }

        [SerializeField] GameObject PolygonDicePanel;
        int activePolygonDice { get; set; } = 0;

        void EnablePolygonDices()
        {
            PolygonDicePanel.SetActive(true);

            for (int i = 0; i < polygonDices.Count; i++)
            {
                if (cards.Count <= polygonDices[i].MaxNum)
                {
                    polygonDices[i].button.interactable = true;
                    activePolygonDice = i;
                    break;
                }
                else
                    polygonDices[i].button.interactable = false;
            }
        }

        int polygonDiceScore = 1;

        public void OnRollPolygonDice(int _type, int _maxNumber)
        {
            if (polygonDiceCount > 0)
            {
                if (_type == cards.Count)
                {
                    if (activePolygonDice == polygonDices.Count - 1)
                    {
                        polygonDices[activePolygonDice].button.interactable = false;
                        polygonDiceScore *= _maxNumber;
                        StartCoroutine(EnableCoinTypeSelector());
                        return;
                    }

                    polygonDiceCount = polygonDiceRollingCount;
                    if (activePolygonDice < polygonDices.Count - 1)
                    {
                        polygonDiceScore *= _maxNumber;
                        polygonDices[activePolygonDice].button.interactable = false;
                        activePolygonDice++;
                        polygonDices[activePolygonDice].button.interactable = true;
                    }

                }
                else
                    polygonDiceCount--;

                if (polygonDiceCount <= 0)
                {
                    polygonDices[activePolygonDice].button.interactable = false;
                    StartCoroutine(EnableCoinTypeSelector());
                }
            }
        }

        IEnumerator EnableCoinTypeSelector()
        {
            yield return new WaitForSeconds(0.5f);
            CoinSelectionPanel.SetActive(true);
        }


        /// <summary>
        /// This function call on a button to rolls the dices and d3 dices
        /// </summary>
        public void OnClickRollButton()
        {
            if (!isJackpot && (selectedD3Dices.Count < selectedCards.Count ||
                selectedD3Dices.Count > d3DiceCount))
                return;

            if (isFirstClick)
            {
                RollsAllowed = Utilities.RollsCount(diceCount + d3DiceCount);
                
                D3DiceSelection = false;
                isFirstClick = false;

                foreach (D3Dice _d3Dice in d3Dices)
                {
                    if (!selectedD3Dices.Contains(_d3Dice))
                    {
                        _d3Dice.button.interactable = false;
                    }
                }
            }

            if (RollsAllowed > 0)
            {
                DiceRollButton.interactable = false;
                for (int i = 0; i < diceCount; i++)
                {
                    chessDices[i].RollDice();
                }

                foreach (D3Dice _d3Dice in selectedD3Dices)
                {
                    _d3Dice.RollDice();
                }

                RollsAllowed--;
                
                if (RollsAllowed == 0)
                {
                    if (isJackpot)
                    {
                        StartCoroutine(FinalCheckingJackpot(2.5f));
                    }
                    else
                    {
                        StartCoroutine(FinalChecking(2.5f));
                    }
                }
                else
                {
                    if (isJackpot)
                    {
                        CheckMatchedDiceCardsJackpot(1);
                    }
                    else
                    {
                        CheckMatchedDiceCards(1);
                    }

                    StartCoroutine(ResetRollButton(1.25f));
                }
            }
        }

        IEnumerator FinalChecking(float _time)
        {
            yield return new WaitForSeconds(_time);

            print("Final Checking");

            for (int i = 0; i < selectedCards.Count; i++)
            {
                if (!chessDices[i].isSelected)
                {
                    chessDices[i].CompareDiceBonus(true);
                }
            }

            foreach (D3Dice _d3Dice in selectedD3Dices)
            {
                if (!_d3Dice.isSelected)
                {
                    _d3Dice.OnD3DiceClick(true);
                }
            }

            CheckMatchedDiceCards(1);
        }

        IEnumerator FinalCheckingJackpot(float _time)
        {
            yield return new WaitForSeconds(_time);

            print("Final Checking Jackport");

            for (int i = 0; i < diceCount; i++)
            {
                if (!chessDices[i].isSelected &&
                    chessDices[i].Type != "Ace") // Some check
                {
                    chessDices[i].OnClickDiceJackpotFinalCheck();
                }
            }

            for (int i = 0; i < d3DiceCount; i++)
            {
                if (!d3Dices[i].isSelected)
                    d3Dices[i].OnD3DiceClickJackpot();
            }

            CheckMatchedDiceCardsJackpot(1);
        }

        IEnumerator ResetRollButton(float _waitTime)
        {
            yield return new WaitForSeconds(_waitTime);
            DiceRollButton.interactable = true;
        }

        public void OnClickEnableD3Dices(int index)
        {
            D3Dice _d3Dice = d3Dices[index];

            if (selectedD3Dices.Contains(_d3Dice))
            {
                selectedD3Dices.Remove(_d3Dice);
                _d3Dice.SetButtonActive(false);
                return;
            }

            if (selectedD3Dices.Count >= d3DiceCount)
            {
                return;
            }

            selectedD3Dices.Add(_d3Dice);
            _d3Dice.SetButtonActive(true);

            foreach (D3Dice _dice in d3Dices)
            {
                if (!selectedD3Dices.Contains(_dice))
                {
                    _dice.SetButtonActive(false);
                }
            }
        }

        bool isWon = false;

        public void CheckMatchedDiceCards(float _waitTime)
        {
            StartCoroutine(CorontineCheckMatchedDiceCards(_waitTime));
        }

        IEnumerator CorontineCheckMatchedDiceCards(float _waitTime)
        {
            yield return new WaitForSeconds(_waitTime);
            __CheckMatchedDiceCards__();
        }

        void __CheckMatchedDiceCards__()
        {
            if (isWon)
            {
                RollsAllowed = 0;
                DiceRollButton.interactable = false;
                return;
            }

            print("Simple Card Checking Call");

            bool isDiceAndCardMatched = true;

            // Match all possible hands
            for (int i = 0; i < selectedCards.Count; i++)
            {
                if (!selectedCards[i].isMatched)
                {
                    isDiceAndCardMatched = false;
                    break;
                }
            }

            if (isDiceAndCardMatched)
            {
                // Match all chess dices with the possible hands
                for (int i = 0; i < diceCount; i++)
                {
                    if (!chessDices[i].isSelected)
                    {
                        isDiceAndCardMatched = false;
                        break;
                    }
                }
            }
            bool isD3DiceCountMatched = true;

            foreach (D3Dice _d3Dice in selectedD3Dices)
            {
                if (!_d3Dice.isSelected)
                {
                    isD3DiceCountMatched = false;
                    break;
                }
            }

            if (isDiceAndCardMatched && isD3DiceCountMatched
                && D3Score == cards.Count)
            {
                RollsAllowed = 0;
                DiceRollButton.interactable = false;

                isWon = true;
                polygonDiceRollingCount = 3;
            }
            else if (isDiceAndCardMatched || (isD3DiceCountMatched && D3Score == cards.Count))
            {
                if (RollsAllowed > 0)
                    return;
                isWon = true;
                polygonDiceRollingCount = 2;
            }
            else
            {
                if (RollsAllowed > 0)
                    return;
                isWon = true;
                polygonDiceRollingCount = 1;
            }
            polygonDiceCount = polygonDiceRollingCount;
            EnablePolygonDices();
        }


        public void CheckMatchedDiceCardsJackpot(float _waitTime)
        {
            StartCoroutine(CorontineCheckMatchedDiceCardsJackpot(_waitTime));
        }

        IEnumerator CorontineCheckMatchedDiceCardsJackpot(float _waitTime)
        {
            yield return new WaitForSeconds(_waitTime);
            __CheckMatchedDiceCardsJackpot__();
        }

        void __CheckMatchedDiceCardsJackpot__()
        {
            if (isWon)
                return;

            bool isDiceAndCardMatched = true;

            // Match all possible hands
            for (int i = 0; i < selectedCards.Count; i++)
            {
                if (!selectedCards[i].isMatched)
                {
                    isDiceAndCardMatched = false;
                    break;
                }
            }

            if (isDiceAndCardMatched)
            {
                // Match all chess dices with the possible hands
                for (int i = 0; i < diceCount; i++)
                {
                    if (!chessDices[i].isSelected)
                    {
                        isDiceAndCardMatched = false;
                        break;
                    }
                }
            }
            bool isD3DiceCountMatched = true;

            // Check all D3 dices
            for (int i = 0; i < d3DiceCount; i++)
            {
                if (!d3Dices[i].isSelected)
                {
                    isD3DiceCountMatched = false;
                    break;
                }
            }

            if (isDiceAndCardMatched && isD3DiceCountMatched)
            {
                Debug.Log("Won" % Colorize.Green);

                RollsAllowed = 0;
                DiceRollButton.interactable = false;
                isWon = true;
            }
            else if (isDiceAndCardMatched)
            {
                if (RollsAllowed > 0)
                    return;
                isWon = true;
            }
            else
            {
                return;
            }

            polygonDiceRollingCount = 3;
            polygonDiceCount = polygonDiceRollingCount;
            EnablePolygonDices();
        }

        int jackpotCount = 0;
        
        public void AddCardInList(Card card)
        {
            if (isCombinationMake)
                return;

            cards.Add(card);
            List<List<int>> combination = Utilities.MakePossibleCombinations(cards);
            jackpotCount++;
            if (combination != null)
            {
                foreach (List<int> com in combination)
                {
                    List<Card> cardList = Utilities.ConvertCombinationToCardList(cards, com);

                    cardList = Utilities.SortCardList(cardList);

                    Hand possibleHand = Utilities.GetPossibleHand(cardList);
                    if (possibleHand.isHand())
                    {
                        if (jackpotCount == 3)
                        {
                            isJackpot = true;
                            jackpotD3Dice = new List<D3Dice>();
                        }

                        selectedCards = cardList;
                        EnableselectedCards(selectedCards);

                        Invoke(nameof(WaitToGetChessAndD3Dices), 0.25f);
                        isCombinationMake = true;
                        break;
                    }
                }

                if (!isCombinationMake)
                {
                    shuffleCards.PassACard();
                }
            }
            else
            {
                shuffleCards.PassACard();
            }
        }

        void WaitToGetChessAndD3Dices()
        {
            ChessDiceAnim.SetActive(true);
            D3DiceAnim.SetActive(true);

            if (isJackpot)
            {
                EnableDicesAndD3DicesJackpot();
            }
            else
            {
                EnableDicesAndD3Dices(selectedCards.Count);
            }
        }

        void EnableselectedCards(List<Card> _selectedCards)
        {
            int i = 0;
            int cardCount = cards.Count - _selectedCards.Count;

            foreach (Card card in cards)
            {
                if (!_selectedCards.Contains(card))
                {
                    Vector3 targetPos = new Vector3(0, 3.85f, 0) + (i - (cardCount - 1) / 2.0f) * 3 * Vector3.right;
                    card.GetComponent<CardMovement>().MoveTowardsFixed(targetPos, 10);
                    i++;
                }
            }

            i = 0;
            foreach (Card card in _selectedCards)
            {
                card.isInteractable = false;
                Vector3 targetPos = Vector3.zero;

                if (isJackpot)
                    targetPos = new Vector3(0, 1.5f, 0) + (i - (_selectedCards.Count - 1) / 2.0f) * 3 * Vector3.right;
                else
                    targetPos = new Vector3(0, -1, 0) + (i - (_selectedCards.Count - 1) / 2.0f) * 3 * Vector3.right;


                card.GetComponent<CardMovement>().MoveTowardsFixed(targetPos, 10);
                i++;
            }
        }

        void EnableDicesAndD3Dices(int diceCount)
        {
            this.diceCount = diceCount;
            this.d3DiceCount = cards.Count;

            for (int i = 0; i < diceCount; i++)
            {
                chessDices[i].button.interactable = true;
            }

            for (int i = 0; i < d3Dices.Count; i++)
            {
                d3Dices[i].button.interactable = true;
                d3Dices[i].SetButtonActive(true);
            }

            DiceRollButton.interactable = true;
        }

        void EnableDicesAndD3DicesJackpot()
        {
            diceCount = 9;
            d3DiceCount = 12;

            for (int i = 0; i < d3DiceCount; i++)
            {
                if (i < diceCount)
                {
                    chessDices[i].button.interactable = true;
                }
                d3Dices[i].button.interactable = true;
                selectedD3Dices.Add(d3Dices[i]);
            }

            DiceRollButton.interactable = true;
        }

        public void RestartScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        [SerializeField] GameObject ResultPanel;
        [SerializeField] TextMeshProUGUI scoreText;

        public void ApplyFormula()
        {
            int jackpotDiceScore = 0;
            if (isJackpot)
            {
                jackpotDiceScore = Utilities.TotalValueOfDices(jackpotDice);
            }

            int totalvalue = Utilities.TotalValueOfDices(selectedCards) + jackpotDiceScore;
            double Result = (double)totalvalue;

            int _diceScore = 1;
            if (CheckMatchedDices(selectedCards))
            {
                Hand hand = Utilities.GetSelectedCardHand(selectedCards);
                if (hand != null)
                {
                    _diceScore = hand.GetScore();
                }
            }

            Result *= (double)_diceScore;

            int _d3DiceScore = 1;

            if (CheckMatchedD3Dices(selectedD3Dices))
            {
                _d3DiceScore = Utilities.GetD3DiceHand(selectedD3Dices);
            }

            Result *= (double)(_d3DiceScore * polygonDiceScore);

            Result *= (double)coinScore;

            Debug.Log($"First result {Result}");

            if (isJackpot)
            {
                double jackpotChessDiceScore = (double)GetDiceHandScore(jackpotDice);
                Result *= jackpotChessDiceScore;

                print("Total Value " + totalvalue + ", Dice hand " + _diceScore +
                ", d3 hand score " + _d3DiceScore + ", jackpot Chess Dice " +
                jackpotChessDiceScore + ", Polygon Dice score " + polygonDiceScore +
                ", Coin Score " + coinScore);
            }
            else
            {
                print("Total Value " + totalvalue + ", Dice hand " + _diceScore +
                ", d3 hand score " + _d3DiceScore + ", Polygon Dice score "
                + polygonDiceScore + ", Coin Score " + coinScore);
            }

            Result /= 1000d;

            string _strResult = String.Format("{0:#,0.00}", Result);

            Debug.Log($"Second result {_strResult}");

            HugeFloat balance = new HugeFloat(CashBalance);
            HugeFloat result = new HugeFloat(_strResult);

            HugeFloat sum = balance + result;
            CashBalance = sum.number;

            scoreText.text = "$" + _strResult.ToString();
        }

        bool CheckMatchedDices(List<Card> _selectedCards)
        {
            foreach(Card _card in _selectedCards)
            {
                if (!_card.isMatched)
                    return false;
            }
            return true;
        }

        bool CheckMatchedD3Dices(List<D3Dice> _d3Dices)
        {
            foreach(D3Dice _d3Dice in _d3Dices)
                if(!_d3Dice.isSelected)
                    return false;
            return true;
        }

        [SerializeField] GameObject ExitPanel;

        public void OnClickReturnToMainMenu()
        {
            ExitPanel.SetActive(true);
        }

        public void OnClickYesReturnToMainMenu()
        {
            ResultPanel.SetActive(false);
            if (AudioManager.instance != null)
            {
                AudioManager.instance.StopActiveSound();
            }

            SceneManager.LoadScene(0);
        }

        public void OnClickNoReturnToMainMenu()
        {
            ExitPanel.SetActive(false);
        }

        int GetDiceHandScore(List<Dice> _jackpotHandDice)
        {
            _jackpotHandDice = Utilities.SortDiceList(_jackpotHandDice);
            return Utilities.GetDiceHandScore(_jackpotHandDice);
        }


        [SerializeField] GameObject RedCoin, BlueCoin;
        string randomCoinType = "";
        int coinScore = 0;

        public void SelectCoinType(string _coinType)
        {
            coinType = _coinType;
            CoinSelectionPanel.SetActive(false);

            int num = Random.Range(0, 2);
            if (num == 0) // Red
            {
                RedCoin.SetActive(true);
                randomCoinType = "Red";
            }
            else // Blue
            {
                BlueCoin.SetActive(true);
                randomCoinType = "Blue";
            }
        }

        public void OnEndOfCoinAnimation()
        {
            if (coinType.Equals(randomCoinType))
            {
                coinScore = 2;
            }
            else
            {
                coinScore = 0;
            }

            ApplyFormula();
            Invoke(nameof(EnableResultPanel), 2f);
        }

        void EnableResultPanel()
        {
            ResultPanel.SetActive(true);
        }

        // //////////////////////////////////////////////////////////////////////////// //
        // Select the dice and then select the card to make a match => bonus game //


        [SerializeField] GameObject DiceSaveButton;

        Dice selectedDice { get; set; } = null;

        public void SelectDice(Dice _dice)
        {
            if (selectedDice != null)
                return;

            selectedDice = _dice;

            for (int i = 0; i < diceCount; i++)
            {
                chessDices[i].button.interactable = false;
            }

            selectedDice.button.interactable = true;

            if (isJackpot)
            {
                DiceSaveButton.SetActive(true);
            }
        }

        public void DeSelectDice()
        {
            for (int i = 0; i < diceCount; i++)
            {
                chessDices[i].button.interactable = true;
            }
            selectedDice = null;

            if (isJackpot)
            {
                DiceSaveButton.SetActive(false);
            }
        }

        public Vector3 CompareDiceWithCards(Dice _dice)
        {
            foreach (var card in selectedCards)
            {
                Vector3 pos = CompareDiceAndCard(_dice, card);
                if (pos != Vector3.zero)
                    return pos;
            }
            return Vector3.zero;
        }

        public Vector3 CompareAceDiceWithCards(Dice _dice)
        {
            if (_dice.Type != "Ace") return Vector3.zero;

            foreach (var card in selectedCards)
            {
                Vector3 pos = CompareAceDiceWithCard(_dice, card);
                if (pos != Vector3.zero)
                    return pos;
            }
            return Vector3.zero;
        }

        /// <summary>
        /// This function is only calls wjen Ace Dice selected
        /// </summary>
        /// <param name="_card">Clicked dice</param>
        public void CompareCardWithSelectedDice(Card _card)
        {
            if (selectedDice == null)
                return;

            Vector3 pos = CompareAceDiceWithCard(selectedDice, _card);

            if (pos != Vector3.zero)
            {
                selectedDice.SpawnDiceOnCard(pos);
                if (isJackpot)
                {
                    CheckMatchedDiceCardsJackpot(1);
                }
                else
                {
                    CheckMatchedDiceCards(1);
                }
                DeSelectDice();
            }
        }

        Vector3 CompareAceDiceWithCard(Dice _dice, Card _card)
        {
            if (_dice.Type == "Ace")
            {
                if (!Equals(_card.Type, "King") &&
                    !_card.isMatched)
                {
                    _dice.isSelected = true;
                    _card.isMatched = true;
                    return _card.transform.position;
                }
            }
            return Vector3.zero;
        }

        Vector3 CompareDiceAndCard(Dice _selectedDice, Card _card)
        {
            if (Equals(_selectedDice.Type, _card.Type) &&
                    !_card.isMatched)
            {
                _selectedDice.isSelected = true;
                _card.isMatched = true;
                return _card.transform.position;
            }
            return Vector3.zero;
        }

        float DiceIndex = 0;
        float multiplerDiceVal = 1;
        bool isFirstDiceJackpotSpawn = true;

        [SerializeField] GameObject DiceChoosePanel;
        [SerializeField] GameObject InvisibleTablePanel;

        public void OnClickSaveDice()
        {
            if (jackpotDice.Count >= 6 || 
                selectedDice == null)
                return;

            if (selectedDice.Type == "Ace")
            {
                DiceChoosePanel.SetActive(true);
                InvisibleTablePanel.SetActive(true);
            }
            else
            {
                SaveSelectedJackpotDice();
            }
        }

        public void SaveSelectedJackpotDice(string _selectDiceType = null, Sprite _diceSprite = null)
        {
            jackpotDice.Add(selectedDice);

            if (_selectDiceType != null && _diceSprite != null)
            {
                DiceChoosePanel.SetActive(false);
                InvisibleTablePanel.SetActive(false);

                if (selectedDice == null)
                {
                    Debug.Log("Null reference dice");
                    return;
                }

                selectedDice.Type = _selectDiceType;
                selectedDice.SetImage(_diceSprite);
            }

            selectedDice.isSelected = true;

            Vector3 targetPos = new Vector3(0, -2f, 0) + (DiceIndex * (multiplerDiceVal * 2)) * new Vector3(1, 0, 0);
            if (isFirstDiceJackpotSpawn)
            {
                multiplerDiceVal = 1;
                DiceIndex++;
            }
            else
            {
                multiplerDiceVal = -1;
            }
            isFirstDiceJackpotSpawn = !isFirstDiceJackpotSpawn;

            selectedDice.SpawnDiceOnCard(targetPos);
            CheckMatchedDiceCardsJackpot(1);
            DeSelectDice();
        }

        public void CheckSelectedDiceWithCards(Card _card)
        {
            if (selectedDice == null)
                return;


            if ((Equals(_card.Type, selectedDice.Type) ||
                (Equals(selectedDice.Type, "Ace") && !Equals(_card.Type, "King")))
                && !_card.isMatched)
            {
                selectedDice.isSelected = true;
                _card.isMatched = true;
                selectedDice.SpawnDiceOnCard(_card.transform.position);

                foreach (Dice dice in chessDices)
                {
                    if (!dice.isSelected)
                    {
                        dice.button.interactable = true;
                    }
                }
                DiceSaveButton?.SetActive(false);
                selectedDice = null;
            }
        }

        public void CheckFinalTime(Dice _dice)
        {
            foreach (Card card in selectedCards)
            {
                if (!card.isMatched)
                {
                    if (Equals(card.Type, _dice.Type) ||
                        (Equals(_dice.Type, "Ace") && !Equals(card.Type, "King")))
                    {
                        _dice.isSelected = true;
                        card.isMatched = true;
                        _dice.SpawnDiceOnCard(card.transform.position);
                        return;
                    }
                }
            }

            _dice.isSelected = true;
            jackpotDice.Add(_dice);

            Vector3 targetPos = new Vector3(0, -2f, 0) + (DiceIndex * (multiplerDiceVal * 2)) * new Vector3(1, 0, 0);
            if (isFirstDiceJackpotSpawn)
            {
                multiplerDiceVal = 1;
                DiceIndex++;
            }
            else
            {
                multiplerDiceVal = -1;
            }
            isFirstDiceJackpotSpawn = !isFirstDiceJackpotSpawn;

            _dice.SpawnDiceOnCard(targetPos);
        }

        public void OnClickResetScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
    