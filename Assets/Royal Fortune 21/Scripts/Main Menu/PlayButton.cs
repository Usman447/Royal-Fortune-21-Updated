using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoyalFortune21
{
    public class PlayButton : MonoBehaviour
    {
        [SerializeField] float MoveSpeed = 5;
        bool isReached { get; set; }
        Vector3 ReachedPosition;

        RectTransform trans;

        void OnEnable()
        {
            isReached = false;
            trans = GetComponent<RectTransform>();
        }

        void Update()
        {
            if (isReached)
                return;

            trans.anchoredPosition = Vector3.MoveTowards(trans.anchoredPosition, ReachedPosition, MoveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.localPosition, ReachedPosition) == 0)
            {
                isReached = true;
                enabled = false;
            }
        }

        public void BringPlayButtonDown()
        {
            enabled = true;

            ReachedPosition = new Vector3(2, -235, 0);
        }

        public void BringPlayButtonUp()
        {
            enabled = true;

            float buttonHeight = transform.GetComponent<RectTransform>().rect.height;

            float yPos = buttonHeight / 2;

            ReachedPosition = new Vector3(2, yPos + yPos, 0);
        }
    }
}
    