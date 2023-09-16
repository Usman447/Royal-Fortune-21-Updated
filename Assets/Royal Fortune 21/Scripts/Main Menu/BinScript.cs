using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoyalFortune21
{
    public class BinScript : MonoBehaviour
    {
        public void DisableBin()
        {
            gameObject.SetActive(false);
        }

        public void GetBinDown()
        {
            GetComponent<Animator>().SetTrigger("BinDown");
        }
    }
}
    