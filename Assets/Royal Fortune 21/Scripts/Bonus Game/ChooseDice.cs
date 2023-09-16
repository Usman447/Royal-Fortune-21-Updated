using UnityEngine;
using UnityEngine.UI;

namespace RoyalFortune21.BonusGame
{
    public class ChooseDice : MonoBehaviour
    {
        [SerializeField] string DiceType;
        Button button;

        private void Awake()
        {
            button = GetComponent<Button>();

            button.onClick.AddListener(() =>
            {
                Sprite diceSprite = GetComponent<Image>().sprite;
                if (diceSprite != null)
                {
                    BonusGameManager.instance.SaveSelectedJackpotDice(DiceType, diceSprite);
                }
            });
        }
    }
}
    