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
            defaultUI.playButton.onClickDown.AddListener(_ => SpeedCycle());
        }

        protected virtual void Update()
        {
            defaultUI.moneyText.text = $"{MoneyManager.Instance.GetMoney():n0}";
            defaultUI.playButtonImage.sprite = GetPlaySprite();
        }

        protected virtual void OnEnable()
        {
            Start();
        }

        public static void SpeedCycle(int idx)
        {
            switch (idx)
            {
                case 0:
                    Time.timeScale = 0;
                    break;
                case 1:
                    Time.timeScale = 1;
                    TimeManager.Instance.timeScale = 2;
                    break;
                case 2:
                    Time.timeScale = 1;
                    TimeManager.Instance.timeScale = 1.1f;
                    break;
                case 3:
                    Time.timeScale = 1;
                    TimeManager.Instance.timeScale = 0.5f;
                    break;
            }
        }

        public static void SpeedCycle()
        {
            if (UIManager.Instance.GetKey() is UIElements.InGameMenu) return;
            if (Time.timeScale == 0)
            {
                Time.timeScale = 1;
                TimeManager.Instance.timeScale = 2;
            }
            else
            {
                switch (TimeManager.Instance.timeScale)
                {
                    case <= 0.6f:
                        Time.timeScale = 0;
                        break;
                    case <= 1.2f:
                        TimeManager.Instance.timeScale = 0.5f;
                        break;
                    default:
                        TimeManager.Instance.timeScale = 1.1f;
                        break;
                }
            }
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
                <= 0.6f => Variables.Instance.superFastPlaySprite,
                <= 1.2f => Variables.Instance.fastPlaySprite,
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