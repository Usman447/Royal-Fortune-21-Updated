using RoyalFortune21.BonusGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoyalFortune21
{
    public class Coin : MonoBehaviour
    {
        public void OnEndOfCoinAnimation()
        {
            if(BonusGameManager.instance != null)
            {
                BonusGameManager.instance.OnEndOfCoinAnimation();
            }
        }
    }
}
    