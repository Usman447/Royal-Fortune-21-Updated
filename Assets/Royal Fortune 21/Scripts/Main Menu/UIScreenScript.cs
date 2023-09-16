using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using RoyalFortune21.Audio;
using System;
using TMPro;

namespace RoyalFortune21
{
    public class UIScreenScript : MonoBehaviour
    {
        [SerializeField] GameObject SettingsPanel;
        [SerializeField] GameObject SelectCardsPanel;
        [SerializeField] Animator mainScreenAnimator;

        [SerializeField] PlayButton playButton;
        [SerializeField] SlotScript slotScript;

        [SerializeField] TextMeshProUGUI BalanceText;

        public string CashBalance
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
                BalanceText.text = PlayerPrefs.GetString("Cash Balance", "500.00");
            }
        }

        string deductedAmount = "";

        private void Awake()
        {
            Application.targetFrameRate = 60;

            mainScreenAnimator = GetComponentInChildren<Animator>();
            deductedAmount = "";
        }

        private void Start()
        {
            AudioManager.instance.Play("main menu");
            mainScreenAnimator.enabled = true;
            playButton.BringPlayButtonUp();

            CashBalance = "";
        }

        public void OpenSettingsPanel()
        {
            SettingsPanel.SetActive(true);
        }

        public void CloseSettingsPanel()
        {
            SettingsPanel.SetActive(false);
        }

        public void OnClickPlayButton()
        {
            playButton.BringPlayButtonDown();
            slotScript.BringSlotUp();
        }

        public void OnCloseSlotButton()
        {
            slotScript.BringSlotDown();
            playButton.BringPlayButtonUp();
        }

        public void SetGameCategory(int _val)
        {
            if (canPlayGame(_val))
            {
                PlayerPrefs.SetInt("SlotType", _val);
                SelectCardsPanel.SetActive(true);
            }
        }

        bool canPlayGame(int _gameCat)
        {
            switch (_gameCat)
            {
                case 1:
                    return CompareBalance("0.21");
                case 2:
                    return CompareBalance("1.05");
                case 3:
                    return CompareBalance("2.10");
                case 4:
                    return CompareBalance("5.25");
                case 5:
                    return CompareBalance("10.50");
                case 6:
                    return CompareBalance("21.0");
            }
            return false;
        }

        bool CompareBalance(string _amount)
        {
            HugeFloat amount = new HugeFloat(_amount);
            HugeFloat balance = new HugeFloat(CashBalance);
            print(balance >= amount);

            if (balance >= amount)
            {
                balance -= amount;
                deductedAmount = balance.number;

                return true;
            }
            return false;
        }

        public void DeductAmount()
        {
            CashBalance = deductedAmount;
        }
    }
}
