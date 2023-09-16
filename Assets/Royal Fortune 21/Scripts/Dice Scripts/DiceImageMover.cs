using RoyalFortune21;
using RoyalFortune21.BonusGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceImageMover : MonoBehaviour
{
    [SerializeField] float MoveSpeed = 10f;

    public Vector3 towardsPosition { get; set; } = Vector3.zero;

    private void Update()
    {
        if (towardsPosition != Vector3.zero)
        {
            transform.position = Vector3.MoveTowards(this.transform.position, towardsPosition, MoveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, towardsPosition) == 0)
            {
                this.enabled = false;
            }
        }
    }
}
