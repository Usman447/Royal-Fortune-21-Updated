using UnityEngine;
using MPUIKIT;

namespace RoyalFortune21.BonusGame
{
    public class DiceChoose : MonoBehaviour
    {
        [SerializeField] float m_RotationSpeed = 5;
        MPImage _strokPanel;

        private void Awake()
        {
            _strokPanel = GameObject.Find("strok").GetComponent<MPImage>();
        }


        private void Update()
        {
            if(_strokPanel == null)
            {
                this.enabled = false;
                return;
            }

            var gridEffect = _strokPanel.GradientEffect;
            gridEffect.Rotation -= m_RotationSpeed * Time.deltaTime;

            if(gridEffect.Rotation <= -360)
            {
                gridEffect.Rotation = 0;
            }

            _strokPanel.GradientEffect = gridEffect;
        }
    }
}
    