using System;
using _root.Scripts.Game;
using _root.Scripts.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _root.Scripts.UI
{
    public class GameView : View
    {
        [SerializeField] private DefaultUI defaultUI;

        protected virtual void Start()
        {
            defaultUI.playButton.onClickDown.AddListener(_ =>
            {
                if (Time.timeScale == 0)
                {
                    Time.timeScale = 1;
                    TimeManager.Instance.timeScale = 1;
                }
                else
                {
                    switch (TimeManager.Instance.timeScale)
                    {
                        case <= 0.3f:
                            Time.timeScale = 0;
                            break;
                        case <= 0.6f:
                            TimeManager.Instance.timeScale = 0.3f;
                            break;
                        default:
                            TimeManager.Instance.timeScale = 0.55f;
                            break;
                    }
                }
            });
        }

        protected virtual void Update()
        {
            defaultUI.moneyText.text = $"{MoneyManager.Instance.GetMoney():n0}";
            defaultUI.playButtonImage.sprite = GetPlaySprite();
        }

        private Sprite GetPlaySprite()
        {
            if (Time.timeScale == 0) return Variables.Instance.pauseSprite;
            return TimeManager.Instance.timeScale switch
            {
                <= 0.3f => Variables.Instance.superFastPlaySprite,
                <= 0.6f => Variables.Instance.fastPlaySprite,
                _ => Variables.Instance.playSprite
            };
        }

        [Serializable]
        public struct DefaultUI
        {
            public TextMeshProUGUI moneyText;
            public UIImage playButton;
            public Image playButtonImage;
        }
    }
}