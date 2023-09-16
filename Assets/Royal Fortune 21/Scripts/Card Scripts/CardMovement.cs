using System.Collections;
using UnityEngine;
using System;

namespace RoyalFortune21.CardProperty
{
    public class CardMovement : MonoBehaviour
    {
        [SerializeField] float MoveSpeed = 30;
        public Vector3 ReachedPosition { get; private set; }
        public Vector3 currentPosition { get; set; }
        public bool isReached { get; set; } = false;

        public event Action OnReached;

        float moveSpeed = 0;
        bool wantsToDestroy = false;


        private void Awake()
        {
            this.enabled = false;
        }

        private void OnEnable()
        {
            isReached = false;
        }

        private void Update()
        {
            if (isReached)
                return;

            transform.position = Vector3.MoveTowards(this.transform.position, ReachedPosition, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, ReachedPosition) == 0)
            {
                isReached = true;
                enabled = false;
                if (wantsToDestroy)
                {
                    BinScript bin = FindObjectOfType<BinScript>();
                    if (bin != null)
                        bin.GetBinDown();

                    Destroy(gameObject);
                }
                wantsToDestroy = false;
            }
        }

        public void MoveTowards(Vector3 position, bool _wantsToDestroy = false)
        {
            moveSpeed = MoveSpeed;
            ReachedPosition = position;
            wantsToDestroy = _wantsToDestroy;
            enabled = true;
        }

        public void MoveTowards(Vector3 position, float _moveSpeed, bool _wantsToDestroy = false)
        {
            moveSpeed = _moveSpeed;
            ReachedPosition = position;
            wantsToDestroy = _wantsToDestroy;
            enabled = true;
        }

        public void MoveTowardsFixed(Vector3 position, float speed)
        {
            moveSpeed = speed;
            StartCoroutine(MoveCardTowardsPos(position));
        }

        public void MoveTowardsFixed(Vector3 position)
        {
            moveSpeed = MoveSpeed;
            StartCoroutine(MoveCardTowardsPos(position));
        }

        IEnumerator MoveCardTowardsPos(Vector3 pos)
        {
            ReachedPosition = pos;
            enabled = true;

            while (!isReached)
                yield return null;

            OnReached?.Invoke();
        }
    }
}
    