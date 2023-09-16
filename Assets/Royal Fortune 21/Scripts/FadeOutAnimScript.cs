using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoyalFortune21.BonusGame
{
    public class FadeOutAnimScript : MonoBehaviour
    {
        public void OnEndFadeOut()
        {
            FindObjectOfType<BonusShuffleCard>().StartGame();
        }
    }
}
    