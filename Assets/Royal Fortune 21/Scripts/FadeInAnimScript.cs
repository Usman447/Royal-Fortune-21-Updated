using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoyalFortune21
{
    public class FadeInAnimScript : MonoBehaviour
    {
        public void OnEndOfFadeIn()
        {
            GameManager.instance.LoadBonusGameScene();
        }
    }
}
    