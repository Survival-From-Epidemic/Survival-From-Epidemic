using System;
using _root.Scripts.Game;
using _root.Scripts.Managers;
using _root.Scripts.Managers.UI;
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
                if (UIManager.Instance.GetKey() == UIElements.InGameMenu) return;
                if (Time.timeScale == 0)
                {
                    Time.timeScale = 2;
                    TimeManager.Instance.timeScale = 1;
                }
                else
                {
                    switch (TimeManager.Instance.timeScale)
                    {
                        case <= 0.7f:
                            Time.timeScale = 0;
                            break;
                        case <= 1.3f:
                            TimeManager.Instance.timeScale = 0.66f;
                            break;
                        default:
                            TimeManager.Instance.timeScale = 1.22f;
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

        public override void OnTimeChanged(DateTime dateTime)
        {
            defaultUI.dateText.text = dateTime.ToShortDateString();
        }

        private Sprite GetPlaySprite()
        {
            if (Time.timeScale == 0) return Variables.Instance.pauseSprite;
            return TimeManager.Instance.timeScale switch
            {
                <= 0.7f => Variables.Instance.superFastPlaySprite,
                <= 1.3f => Variables.Instance.fastPlaySprite,
                _ => Variables.Instance.playSprite
            };
        }

        [Serializable]
        public class DefaultUI
        {
            public TextMeshProUGUI moneyText;
            public TextMeshProUGUI dateText;
            public UIImage playButton;
            public Image playButtonImage;
        }
    }
}