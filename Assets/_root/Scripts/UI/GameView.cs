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
            defaultUI.playButton.onClickDown.AddListener(_ => TimeManager.SpeedCycle(TimeManager.Instance.speedIdx = (TimeManager.Instance.speedIdx + 1) % 4));
        }

        protected virtual void Update()
        {
            defaultUI.moneyText.text = $"{MoneyManager.Instance.GetMoney():n0}";
            defaultUI.playButtonImage.sprite = GetPlaySprite();
        }

        protected virtual void OnEnable()
        {
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