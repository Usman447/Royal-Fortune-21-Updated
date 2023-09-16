using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using EditorColor;
using RoyalFortune21.CardProperty;
using System.Linq;
using RoyalFortune21.Audio;

namespace RoyalFortune21
{
    #region Classes
    public class Hand
    {
        public Hand()
        {
            score = -1;
        }

        public int GetScore()
        {
            return (int)score;
        }

        public string GetHandName()
        {
            return handName;
        }

        public bool isHand()
        {
            return score != -1;
        }

        public void SetStatus(int _score, string _handName)
        {
            score = _score;
            handName = _handName;
        }
        int score;
        string handName;
    }
    #endregion

    public class GameManager : MonoBehaviour
    {
        public bool isChessCards = false;
        [SerializeField] Vector3 DeckPos;
        [SerializeField] GameObject DrawButton;
        [SerializeField] GameObject RebetButton;
        [SerializeField] Button ChessRollButton;
        [SerializeField] TextMeshProUGUI ChessRollCount;
        [SerializeField] Transform GridPointsTransform;
        [SerializeField] Transform ChessDices;
        [SerializeField] GameObject DiceBlockingPanel;

        [Header("Winning Panel")]
        [SerializeField] GameObject WinningPanel;
        [SerializeField] GameObject TryAgainButton;
        [SerializeField] GameObject ContinueButton;
        [SerializeField] TextMeshProUGUI EarningText;
        [SerializeField] TextMeshProUGUI WinningPanelHandName;

        [Header("Extra Panel Stuff")]
        [SerializeField] GameObject ExtraAttemptPanel;


        public static GameManager instance;
        public List<Card> selectedCards { get; private set; }
        public List<Dice> selectedDices { get; private set; }
        public List<GridPoint> gridPoints { get; private set; }
        List<Card> discardedCards { get; set; }

        List<Dice> Dices { get; set; }
        Dice selectedDice { get; set; } = null;

        float handScore { get; set; } = 0;
        string handName { get; set; }

        bool isCardDiscartOnce = false;
        
        int m_rollsAllowed;
        public int RollsAllowed
        {
            get => m_rollsAllowed;
            set
            {
                m_rollsAllowed = value;
                ChessRollCount.text = ((m_rollsAllowed > 1) ? "Rolls" : "Roll") + " Available: " + m_rollsAllowed;
            }
        }

        bool[][] ArrayOfCombination { get; set; }

        [Header("Game Selection Stuff")]

        [SerializeField] GameObject CardsArrangeDoneButton;
        [SerializeField] GameObject ChessDiceAnimationPanel;
        [SerializeField] GameObject TableAnim;
        [SerializeField] GameObject DiscardButton;
        [SerializeField] GameObject BinObject;
        
        public bool penny { get; private set; }
        public bool dime { get; private set; }
        public bool nikel { get; private set; }
        public bool quarter { get; private set; }
        public bool halfDollar { get; private set; }
        public bool dollar { get; private set; }

        [Header("Other Stuff")]
        [SerializeField] TextMeshProUGUI WagerText;
        [SerializeField] TextMeshProUGUI PotentialWinningText;
        [SerializeField] TextMeshProUGUI BalanceText;

        string wager;
        string Wager
        {
            get => wager;
            set
            {
                wager = value;
                WagerText.text = "$" + wager;
            }
        }

        float potentionalWinning;
        float PotentialWinning
        {
            get => potentionalWinning;
            set
            {
                potentionalWinning = value;
                PotentialWinningText.text = "$" + potentionalWinning.ToString();
            }
        }

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

        int spawnCardCounter = 0;
        public int CardsOnGrid
        {
            get { return spawnCardCounter; }
            set
            {
                spawnCardCounter = value;

                if(spawnCardCounter == 9)
                {
                    if(!penny && !nikel)
                    {
                        CardsArrangeDoneButton.SetActive(true);
                    }
                    else
                    {
                        Invoke(nameof(TriggerCardChecking), 1);
                    }
                    SetDrawButtonActive = false;
                    RebetButton.SetActive(true);
                    InvisiblePanel.SetActive(false);
                }
            }
        }

        bool SetDrawButtonActive
        {
            set
            {
                DrawButton.SetActive(value);
                if (!value)
                {
                    PotentialWinning = 0;
                }
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
            Initialize();
        }

        bool isFinished = false;
        bool fold = false;

        void Initialize()
        {
            penny = dime = nikel = quarter = halfDollar = dollar = false;

            SetGameType(PlayerPrefs.GetInt("SlotType", 1));

            TableAnim.SetActive(true);

            List<Dice> _allDices = new List<Dice>();
            foreach(Transform diceTrans in ChessDices)
            {
                _allDices.Add(diceTrans.GetComponentInChildren<Dice>());
            }

            Dices = _allDices;

            gridPoints = new List<GridPoint>();

            foreach(Transform gridTrans in GridPointsTransform)
            {
                gridPoints.Add(gridTrans.GetComponent<GridPoint>());
            }
            
            selectedCards = new List<Card>();
            selectedDices = new List<Dice>();
            discardedCards = new List<Card>();
            selectedDice = null;
            isCardDiscartOnce = false;

            PotentialWinning = 0;
            Wager = "";
            CashBalance = "";
            isFinished = false;
            fold = false;
            isGetExtraRoll = false;

            if (penny || nikel || halfDollar || dollar)
                InvisiblePanel.SetActive(true);
        }

        public void CardPropertyOnGameSelected(Card card)
        {
            if (penny)
            {
                ChessDiceAnimationPanel.SetActive(true);
            }
            else if (nikel)
            {
                ChessDiceAnimationPanel.SetActive(true);
                card.enabled = false;
                card.GetComponent<CardPlacementMovement>().enabled = true;
            }
            else if(dime || quarter 
                || halfDollar || dollar)
            {
                card.enabled = false;
                card.GetComponent<CardPlacementMovement>().enabled = true;
            }
        }

        private void Start()
        {
            SetCombinations();
            SetSoundWithSlotType();
            WagerAmount();
        }

        #region Penny Auto Detection

        public void TriggerCardChecking()
        {
            List<List<Card>> _AllCards = GetCardsFromAllIndexes();

            float maxScore = 0;
            string maxScoreHandName = "";

            List<Card> _selectedHandCards = new List<Card>();

            Hand hand = null;
            bool isAceHighScore = false;
            foreach (List<Card> _selectedCards in _AllCards)
            {
                hand = Utilities.GetSelectedCardHand(_selectedCards);
                if (hand.GetScore() != 0)
                {
                    handScore = hand.GetScore();
                    if (handScore > maxScore)
                    {
                        maxScore = handScore;
                        maxScoreHandName = hand.GetHandName();

                        _selectedHandCards = _selectedCards;
                        isAceHighScore = Utilities.AceHighScore;

                    }
                }
            }
            print(maxScore.ToString() % Colorize.Green);
            Utilities.printList("Print Highest List", _selectedHandCards, true);

            if (maxScore != 0)
            {
                Utilities.AceHighScore = isAceHighScore;
                print("Is Ace High Score " + Utilities.AceHighScore);
                selectedCards = _selectedHandCards;
                handScore = maxScore;
                handName = maxScoreHandName;

                SetDrawButtonActive = true;

                CalculatePotentialWinningAmount();
                foreach (Card card in selectedCards)
                {
                    card.transform.localScale = isChessCards ? new Vector3(0.38f, 0.38f, 1) : new Vector3(1.36f, 1.36f, 1);
                }
            }
        }

        void CalculatePotentialWinningAmount()
        {
            print("Print Potential Winning Amount");

            PotentialWinning = (handScore * selectedCards.Count * Utilities.TotalValueOfDices(selectedCards)) / 100;
        }


        public void OnClickResetScene()
        {
            if (!canPlayGame())
            {
                Debug.Log("Out of Balance" % Colorize.Red);
                return;
            }
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        bool canPlayGame()
        {
            if (penny)
            {
                return CompareBalance("0.21");
            }
            else if (nikel)
            {
                return CompareBalance("1.05");
            }
            else if(dime)
            {
                return CompareBalance("2.10");
            }
            else if (quarter)
            {
                return CompareBalance("5.25");
            }
            else if (halfDollar)
            {
                return CompareBalance("10.50");
            }
            else if (dollar)
            {
                return CompareBalance("21.0");
            }
            else
            {
                Debug.LogError("No game category is selected");
                return false;
            }
        }

        bool CompareBalance(string _amount)
        {
            HugeFloat amount = new HugeFloat(_amount);
            HugeFloat balance = new HugeFloat(CashBalance);

            if (balance >= amount)
            {
                balance -= amount;
                CashBalance = balance.number;

                return true;
            }
            return false;
        }

        List<List<Card>> GetCardsFromAllIndexes()
        {
            List<List<int>> _indexes = GetAvailableIndexes();
            List<List<Card>> _allCardsOnIndex = new List<List<Card>>();

            List<Card> cards = FindObjectsOfType<Card>().ToList();

            foreach (List<int> _index in _indexes)
            {
                List<Card> _cardsOnIndex = new List<Card>();
                foreach (int i in _index)
                {
                    Card card = GetCardOnIndex(cards, i);
                    if(card != null)
                    {
                        _cardsOnIndex.Add(card);
                    }
                }
                _allCardsOnIndex.Add(_cardsOnIndex);
            }
            return _allCardsOnIndex;
        }

        Card GetCardOnIndex(List<Card> _cards, int _index)
        {
            foreach(Card card in _cards)
            {
                if(card.Grid.Index == _index)
                    return card;
            }
            return null;
        }

        List<List<int>> GetAvailableIndexes()
        {
            List<List<int>> _availableIndexes = new List<List<int>>();

            for (int index = ArrayOfCombination.Length - 1; index >= 0; index--)
            {
                bool[] combination = ArrayOfCombination[index];
                List<int> indexList = new List<int>();

                for (int i = 0; i < combination.Length; i++)
                {
                    if (combination[i])
                    {
                        indexList.Add(i);
                    }
                }
                _availableIndexes.Add(indexList);
            }
            return _availableIndexes;
        }

        #endregion

        void WagerAmount()
        {
            HugeFloat value = new HugeFloat(Wager);
            HugeFloat gameValue = new HugeFloat("");

            if (penny)
            {
                gameValue.number = "0.21"; //0.21
            }
            else if (nikel)
            {
                gameValue.number = "1.05"; //1.05
            }
            else if (dime)
            {
                gameValue.number = "2.10"; //2.10
            }
            else if (quarter)
            {
                gameValue.number = "5.25"; //5.25
            }
            else if (halfDollar)
            {
                gameValue.number = "10.50"; //10.50
            }
            else if (dollar)
            {
                gameValue.number = "21.0"; //21.0
            }

            Wager = (value + gameValue).number;
        }

        void SetSoundWithSlotType()
        {
            AudioManager.instance.StopActiveSound();
            if (penny)
            {
                AudioManager.instance.Play("penny");
            }
            else if (dime)
            {
                AudioManager.instance.Play("dime");
            }
            else if (nikel)
            {
                AudioManager.instance.Play("nikel");
            }
            else if(quarter)
            {
                AudioManager.instance.Play("quarter");
            }
            else if(halfDollar)
            {
                AudioManager.instance.Play("half dollar");
            }
            else if(dollar)
            {
                AudioManager.instance.Play("dollar");
            }
            else
            {
                Debug.LogWarning("No slot is available");
            }
        }

        void SetGameType(int _type)
        {
            switch (_type)
            {
                case 1:
                    penny = true;
                    break;
                case 2:
                    nikel = true;
                    break;
                case 3:
                    dime = true;
                    break;
                case 4:
                    quarter = true;
                    break;    
                case 5:
                    halfDollar = true;
                    break;
                case 6:
                    dollar = true;
                    break;
            }
        }

        void SetCombinations()
        {
            ArrayOfCombination = new bool[23][];
            ArrayOfCombination[0] = new bool[] { true, true, true, false, false, false, false, false, false };//012
            ArrayOfCombination[1] = new bool[] { false, false, false, true, true, true, false, false, false };//345
            ArrayOfCombination[2] = new bool[] { false, false, false, false, false, false, true, true, true };//678

            ArrayOfCombination[3] = new bool[] { true, false, false, true, false, false, true, false, false };//036
            ArrayOfCombination[4] = new bool[] { false, true, false, false, true, false, false, true, false };//147
            ArrayOfCombination[5] = new bool[] { false, false, true, false, false, true, false, false, true };//258

            ArrayOfCombination[6] = new bool[] { true, false, false, false, true, false, false, false, true };//048
            ArrayOfCombination[7] = new bool[] { false, false, true, false, true, false, true, false, false };//246

            ArrayOfCombination[8] = new bool[] { true, false, false, false, true, false, true, false, false };//046
            ArrayOfCombination[9] = new bool[] { true, false, true, false, true, false, false, false, false };//024
            ArrayOfCombination[10] = new bool[] { false, false, true, false, true, false, false, false, true };//248
            ArrayOfCombination[11] = new bool[] { false, false, false, false, true, false, true, false, true };//468

            ArrayOfCombination[12] = new bool[] { false, true, false, true, false, false, false, true, false };//137
            ArrayOfCombination[13] = new bool[] { false, true, false, true, false, true, false, false, false };//135
            ArrayOfCombination[14] = new bool[] { false, true, false, false, false, true, false, true, false };//157
            ArrayOfCombination[15] = new bool[] { false, false, false, true, false, true, false, true, false };//357

            ArrayOfCombination[16] = new bool[] { true, false, true, false, false, false, true, false, true };//0268

            ArrayOfCombination[17] = new bool[] { false, true, false, true, true, true, false, true, false };//13457

            ArrayOfCombination[18] = new bool[] { true, true, true, true, true, true, false, false, false };//012345
            ArrayOfCombination[19] = new bool[] { false, false, false, true, true, true, true, true, true }; //345678

            ArrayOfCombination[20] = new bool[] { true, true, true, true, true, true, true, false, false };//0123456

            ArrayOfCombination[21] = new bool[] { true, true, true, true, true, true, true, true, false };//01234567

            ArrayOfCombination[22] = new bool[] { true, true, true, true, true, true, true, true, true };//012345678
        }

        // ///////////////////////////////////////////////////////////////// //
        // ///////////////////////////////////////////////////////////////// //
        //                          Support Functions                        //
        // ///////////////////////////////////////////////////////////////// //
        // ///////////////////////////////////////////////////////////////// //

        public void SelectCard(Card _card)
        {
            selectedCards.Add(_card);
            _card.transform.localScale = isChessCards ? new Vector3(0.38f, 0.38f, 1) : new Vector3(1.36f, 1.36f, 1);
            CheckPossibleHand(selectedCards);
        }

        public void DeselectCard(Card _card)
        {
            selectedCards.Remove(_card);
            if (selectedCards.Count < 3)
            {
                SetDrawButtonActive = false;
                //DrawButton.SetActive(false);
            }
            _card.transform.localScale = isChessCards ? new Vector3(0.32f, 0.32f, 1) : new Vector3(1.15f, 1.15f, 1);
            CheckPossibleHand(selectedCards);
        }

        public void SelectCardDiscarded(Card _card)
        {
            if(isCardDiscartOnce)
            {
                return;
            }

            if (nikel && discardedCards.Count == 0)
            {
                foreach(Card card in selectedCards)
                {
                    card.transform.localScale = isChessCards ? new Vector3(0.32f, 0.32f, 1) : new Vector3(1.15f, 1.15f, 1);
                }
                selectedCards.Clear();
                SetDrawButtonActive = false;
                //DrawButton.SetActive(false);
            }

            discardedCards.Add(_card);
            if (discardedCards.Count > 0)
                DiscardButton.SetActive(true);
            _card.transform.localScale = isChessCards ? new Vector3(0.38f, 0.38f, 1) : new Vector3(1.36f, 1.36f, 1);
        }

        public void DeselectCardDiscarded(Card _card)
        {
            if (isCardDiscartOnce)
            {
                return;
            }
            discardedCards.Remove(_card);

            _card.transform.localScale = isChessCards ? new Vector3(0.32f, 0.32f, 1) : new Vector3(1.15f, 1.15f, 1);

            if (discardedCards.Count == 0)
            {
                DiscardButton.SetActive(false);
                if (nikel)
                {
                    TriggerCardChecking();
                }
            }
        }

        public void DiscardSelectedCards()
        {
            if (discardedCards.Count > 0)
            {
                int removeIndex = 0;
                for (int i = 0; i < discardedCards.Count; i++)
                {
                    Card _card = discardedCards[removeIndex];
                    removeIndex++;
                    _card.Grid.IsOccupied = false;
                    _card.cardMovement.MoveTowards(new Vector3(0, -18, 0), true);
                    GameManager.instance.CardsOnGrid--;
                    if (nikel || halfDollar || dollar)
                        FindObjectOfType<ShuffleCards>().SpawnCardOnGrid(_card.cardMovement.currentPosition, _card.Grid);
                }
            }
            discardedCards.Clear();
            DiscardButton.SetActive(false);
            BinObject.SetActive(true);
            isCardDiscartOnce = true;
        }

        public bool AutogenerateOrNot()
        {
            if (penny || nikel || halfDollar || dollar) return true;
            else if(dime || quarter) return false;

            Debug.LogError("No slot is selected");
            return false;
        }

        public void SelectDice(Dice _dice)
        {
            if (selectedDice != null)
                return;

            selectedDice = _dice;

            foreach(Dice dice in selectedDices)
            {
                dice.button.interactable = false;
            }

            selectedDice.button.interactable = true;
        }

        public void DeselectDice()
        {
            foreach (Dice dice in selectedDices)
            {
                dice.button.interactable = true;
            }

            selectedDice = null;
        }

        public GridPoint GetAvailableGridPoint()
        {
            foreach(GridPoint _gridPoint in gridPoints)
            {
                if (!_gridPoint.IsOccupied)
                {
                    return _gridPoint;
                }
            }
            return null;
        }

        #region Function for Check Combinations

        void CheckPossibleHand(List<Card> _selectedCards)
        {
            List<int> _selectedCardIndexes = GetCardIndexes(_selectedCards);
            if (_selectedCardIndexes != null)
            {
                if (CompareAllPossibleHands(ArrayOfCombination, _selectedCardIndexes))
                {
                    Hand hand = Utilities.GetSelectedCardHand(_selectedCards);
                    if (hand.isHand())
                    {
                        SetDrawButtonActive = true;
                        handScore = hand.GetScore();
                        handName = hand.GetHandName();

                        CalculatePotentialWinningAmount();
                    }
                    else
                    {
                        SetDrawButtonActive = false;
                    }
                }
                else
                {
                    SetDrawButtonActive = false;
                }
            }
        }

        List<int> GetCardIndexes(List<Card> _selectedCards)
        {
            if (_selectedCards.Count < 3)
                return null;

            List<int> _cardIndexes = new List<int>();
            foreach (Card _card in _selectedCards)
            {
                _cardIndexes.Add(_card.Grid.Index);
            }

            _cardIndexes.Sort();
            return _cardIndexes;
        }

        bool CompareAllPossibleHands(bool[][] _combinations, List<int> _cardIndex)
        {
            bool isMatch = false;
            for (int i = 0; i < _combinations.Length; i++)
            {
                if (!CompareHand(_combinations[i], _cardIndex))
                {
                    continue;
                }
                isMatch = true;
                break;
            }
            return isMatch;
        }

        bool CompareHand(bool[] _possibleHand, List<int> _cardIndex)
        {
            int index = 0;
            for (int i = 0; i < _possibleHand.Length; i++)
            {
                if (i == _cardIndex[index] && _possibleHand[i])
                {
                    if (index < _cardIndex.Count - 1)
                        index++;
                    continue;
                }
                else if (!_possibleHand[i] && i != _cardIndex[index])
                {
                    continue;
                }
                return false;
            }
            return true;
        }

        #endregion

        #region Compare Functions
        public Vector3 CompareDiceWithCards(Dice _dice)
        {
            foreach(var card in selectedCards)
            {
                Vector3 pos = CompareDiceAndCard(_dice, card);
                if (pos != Vector3.zero)
                {
                    return pos;
                }
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

        public void CompareCardWithDice(Card _card)
        {
            if (selectedDice == null)
                return;

            Vector3 pos = CompareAceDiceWithCard(selectedDice, _card);

            if(pos != Vector3.zero)
            {
                selectedDice.SpawnDiceOnCard(pos);
                CheckMatchedDiceCards(1.5f);
                DeselectDice();
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
                    !_card.isMatched && !_selectedDice.isSelected)
            {
                _selectedDice.isSelected = true;
                _card.isMatched = true;

                return _card.transform.position;
            }
            return Vector3.zero;
        }

        int GetMatchedDiceScore(Card _card)
        {
            if (_card.isMatched)
                return Utilities.ValueOfDice(_card);
            return 1;
        }

        #endregion

        bool isGetExtraRoll = false;


        public void OnClickGetExtraRollingAttempt()
        {
            InvisiblePanel.SetActive(false);
            ExtraAttemptPanel.SetActive(false);
            
            if (!canPlayGame())
            {
                OnClickDontGetExtraRollingAttempt();
                return;
            }

            WagerAmount();
            isGetExtraRoll = true;
            RollsAllowed = 1;
            StartCoroutine(ResetRollButton(0.5f));
            PotentialWinning *= 2;
        }

        public void OnClickDontGetExtraRollingAttempt()
        {
            InvisiblePanel.SetActive(false);
            ExtraAttemptPanel.SetActive(false);
            fold = true;
            CheckMatchedDiceCards(0);
        }

        public void CheckMatchedDiceCards(float _time)
        {
            StartCoroutine(CorontineCheckMatchedDicesAndCards(_time));
        }

        IEnumerator CorontineCheckMatchedDicesAndCards(float _time)
        {
            yield return new WaitForSeconds(_time);
            __CheckMatchCardsAndDices__();
        }

        bool PredictChancePanel()
        {
            int numberOfDices = selectedDices.Count;

            if ((numberOfDices == 4 || numberOfDices == 6 ||
                numberOfDices == 8 || numberOfDices == 9) &&
                RollsAllowed == 0 && !isGetExtraRoll && !fold)
            {
                Debug.Log("Can make an extra roll" % Colorize.Cyan);
                return true;
            }
            Debug.Log("Cannot make an extra roll" % Colorize.Gold);
            return false;
        }

        void __CheckMatchCardsAndDices__()
        {
            if(isFinished) return;

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
                foreach (Dice dice in selectedDices)
                {
                    if (!dice.isSelected)
                    {
                        isDiceAndCardMatched = false;
                        break;
                    }
                }
            }
            else
            {
                print("Losing with one or more cards are not Matched" % Colorize.Red);
            }

            if (isDiceAndCardMatched)
            {
                RollsAllowed = 0;
                ChessRollButton.interactable = false;
                isFinished = true;
                Invoke(nameof(OnWinning), 0.75f);
            }
            else
            {
                if (RollsAllowed > 0)
                    return;
                Invoke(nameof(OnLost), 0.75f);
            }
        }

        void OnWinning()
        {
            print("Winning" % Colorize.Green);
            WinningPanel.SetActive(true);
            WinningPanelHandName.text = handName;
            int _totalVal = Utilities.TotalValueOfDices(selectedCards);
            ShowResultsPanel(_totalVal);
        }

        void OnLost()
        {
            print("Losing" % Colorize.Red);
            if (PredictChancePanel())
            {
                ExtraAttemptPanel.SetActive(true);
                InvisiblePanel.SetActive(true);
                return;
            }
            isFinished = true;
            WinningPanel.SetActive(true);
            WinningPanelHandName.text = handName;
            ContinueButton.SetActive(false);
            TryAgainButton.SetActive(true);
            EarningText.text = "$0";
            CashBalance = "";
        }

        void ShowResultsPanel(int diceScore)
        {
            print(handScore + " " + selectedCards.Count + " " + diceScore);

            float Result = PotentialWinning;

            EarningText.text = "$" + Result.ToString();

            HugeFloat balance = new HugeFloat(CashBalance);
            HugeFloat result = HugeFloat.NormalizeNumber(Result);
            HugeFloat sum = balance + result;
            CashBalance = sum.number;

            if (Result >= 10)
            {
                ContinueButton.SetActive(true);
                TryAgainButton.SetActive(false);
            }
            else
            {
                ContinueButton.SetActive(false);
                TryAgainButton.SetActive(true);
            }
        }

        #region On Click Events Functions

        public void OnClickRollButton()
        {
            if (RollsAllowed > 0)
            {
                ChessRollButton.interactable = false;
                DiceBlockingPanel.SetActive(true);

                foreach (Dice dice in selectedDices)
                {
                    dice.RollDice();
                }

                RollsAllowed--;

                if (RollsAllowed == 0)
                {
                    StartCoroutine(FinalChecking(1.5f)); // 1.75
                }
                else
                {
                    CheckMatchedDiceCards(1.5f);
                    StartCoroutine(ResetRollButton(1.25f));
                }
            }
        }

        bool CheckRemainingDices(List<Dice> _selectedDices)
        {
            int count = 0;
            foreach (Dice dice in _selectedDices)
                if (!dice.isSelected)
                    count++;

            if (count == 1)
                return true;
            return false;
        }

        IEnumerator ResetRollButton(float _time)
        {
            yield return new WaitForSeconds(_time);
            ChessRollButton.interactable = true;
            DiceBlockingPanel.SetActive(false);
        }

        IEnumerator FinalChecking(float _time)
        {
            yield return new WaitForSeconds(_time);    // 1f

            Debug.Log("Final Checking");

            foreach(Dice dice in selectedDices)
            {
                if (!dice.isSelected)
                {
                    dice.CompareDice(true);
                }
            }

            CheckMatchedDiceCards(1.5f);
        }


        // Draw the selected card on one side and remove the all non selected cards
        public void OnClickDrawButtonClicked()
        {
            DrawButton.SetActive(false);
            EnableChessDices(selectedCards.Count);

            RollsAllowed = Utilities.RollsCount(selectedCards.Count);

            StartCoroutine(ResetRollButton(1.5f));

            Card[] _allCards = FindObjectsOfType<Card>();

            foreach (Card card in _allCards)
            {
                card.isInteractable = false;

                if (nikel)
                {
                    card.enabled = true;
                    card.GetComponent<CardPlacementMovement>().enabled = false;
                }

                if(penny)
                {
                    card.enabled = true;
                }

                if (!selectedCards.Contains(card))
                {
                    card.cardMovement.MoveTowards(Vector3.zero);
                }
            }

            StartCoroutine(MoveNonSelectedCardsAway(_allCards));
        }

        IEnumerator MoveNonSelectedCardsAway(Card[] _cards)
        {
            yield return new WaitForSeconds(0.5f);

            foreach (Card card in _cards)
            {
                if (!selectedCards.Contains(card))
                {
                    card.cardMovement.MoveTowards(new Vector3(0, 16, 0));
                }
            }

            StartCoroutine(RearrangeSelectedCards());
        }

        [SerializeField] Transform moveCardGrid;

        IEnumerator RearrangeSelectedCards()
        {
            yield return new WaitForSeconds(0.5f);

            List<Vector3> moveCardToPos = new List<Vector3>();

            foreach(Transform trans in  moveCardGrid)
            {
                moveCardToPos.Add(trans.position);
            }

            for (int i = 0; i < selectedCards.Count; i++)
            {
                int index = i;
                if(selectedCards.Count == 3)
                {
                    index += 3;
                }
                
                selectedCards[i].cardMovement.MoveTowards(moveCardToPos[index]);
            }
        }

        void EnableChessDices(int _count)
        {
            for (int i = 0; i < Dices.Count; i++)
            {
                if (i < _count)
                {
                    selectedDices.Add(Dices[i]);
                    Dices[i].button.interactable = true;
                }
                else
                    Dices[i].button.interactable = false;
            }
        }

        public void ReturnToMainMenu()
        {
            if(AudioManager.instance != null)
            {
                AudioManager.instance.StopActiveSound();
            }

            SceneManager.LoadScene(0);
        }

        [SerializeField] GameObject LoadingPanel;

        public void MoveToBonusGame()
        {
            LoadingPanel.SetActive(true);
        }

        public void LoadBonusGameScene()
        {
            StartCoroutine(LoadYourAsyncScene());
        }

        IEnumerator LoadYourAsyncScene()
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }

        #endregion


        public void OnClickSetAllCardsIndex()
        {
            foreach(CardPlacementMovement cardPlacement in FindObjectsOfType<CardPlacementMovement>())
            {
                Card placedCard = cardPlacement.GetComponent<Card>();

                placedCard.enabled = true;
                placedCard.isInteractable = true;
                placedCard.boxCollider.size = isChessCards ? new Vector2(7.56f, 10.5f) : new Vector2(2.06f, 2.88f);
                cardPlacement.enabled = false;
            }
            ChessDiceAnimationPanel.SetActive(true);
            CardsArrangeDoneButton.SetActive(false);
        }

        public void OnClickCard()
        {
            foreach(Card _card in FindObjectsOfType<Card>())
            {
                _card.boxCollider.size = isChessCards ? new Vector2(4.75f, 5.75f) : new Vector2(1.33f, 1.60f);
            }
        }

        public void OnLeaveCard()
        {
            foreach (Card _card in FindObjectsOfType<Card>())
            {
                _card.boxCollider.size = isChessCards ? new Vector2(7.25f, 10.25f) : new Vector2(2.06f, 2.88f);
            }
        }

        [SerializeField] GameObject ExitPanel;
        [SerializeField] GameObject InvisiblePanel;

        public void OnClickReturnToMainMenu()
        {
            ExitPanel.SetActive(true);
            InvisiblePanel.SetActive(true);
        }

        public void OnClickYesReturnToMainMenu()
        {
            AudioManager.instance.StopActiveSound();
            SceneManager.LoadScene(0);
        }

        public void OnClickNoReturnToMainMenu()
        {
            InvisiblePanel.SetActive(false);
            ExitPanel.SetActive(false);
        }
    }
}
    