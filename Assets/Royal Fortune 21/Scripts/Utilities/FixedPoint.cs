using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoyalFortune21.Utility
{
    [ExecuteAlways]
    public class FixedPoint : MonoBehaviour
    {
        [SerializeField] Camera cam;

        private void Update()
        {
            if (cam == null)
                return;

            float height = 2f * cam.orthographicSize;
            float width = height * cam.aspect;

            float leftPoint = -(width / 2);

            transform.localPosition = new Vector3(leftPoint, transform.localPosition.y, 0);
        }
    }
}
    