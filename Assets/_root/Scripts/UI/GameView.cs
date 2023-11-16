using System;
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
            defaultUI.playButton.onClickDown.AddListener(_ => Time.timeScale = Time.timeScale == 0 ? 1 : 0);
        }

        protected virtual void Update()
        {
            defaultUI.moneyText.text = $"{MoneyManager.Instance.GetMoney():n0}";
            defaultUI.playButtonImage.sprite = Time.timeScale == 0 ? Variables.Instance.pauseSprite : Variables.Instance.playSprite;
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