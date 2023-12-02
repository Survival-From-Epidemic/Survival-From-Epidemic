using System;
using _root.Scripts.Game;
using _root.Scripts.Game.Data;
using _root.Scripts.Game.Data.Child;
using _root.Scripts.Managers.Sound;
using _root.Scripts.SingleTon;
using _root.Scripts.Utils;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _root.Scripts.Managers
{
    public class MoneyManager : SingleMono<MoneyManager>, IDataUpdateable
    {
        [SerializeField] private int money;
        [SerializeField] private RectTransform coinAnchor;
        [SerializeField] private Image coinIcon;
        [SerializeField] private TextMeshProUGUI coinText;

        protected override void Awake()
        {
            base.Awake();
            var color = coinIcon.color;
            color.a = 0;
            coinIcon.color = color;

            var coinTextColor = coinText.color;
            coinTextColor.a = 0;
            coinText.color = coinTextColor;
        }

        private void Start()
        {
            money = 0;
        }

        public void RegisterData(KGlobalData kGlobalData)
        {
            money = kGlobalData.kMoneyManager.money;
        }

        public KMoneyManager Parse()
        {
            var kMoneyManager = new KMoneyManager();
            kMoneyManager.money = money;
            return kMoneyManager;
        }

        // private void OnEnable()
        // {
        //     if (!GameManager.Instance.gameEnd) return;
        //     Start();
        // }

        public int GetMoney() => money;

        public void AddMoney(int value, bool data = false)
        {
            money += Math.Max(0, value);
            ServerDataManager.Instance.money[0] += value;
            if (data) ServerDataManager.Instance.money[1] -= value;
        }

        public void AddMoneyNotify(int value)
        {
            money += Math.Max(0, value);
            ServerDataManager.Instance.money[0] += value;
            SoundManager.Instance.PlayEffectSound(SoundKey.CoinUpload1);
            SoundManager.Instance.PlayEffectSound(SoundKey.CoinUpload2);

            coinAnchor.gameObject.SetActive(true);
            coinText.text = $"+{value:n0}￦";
            coinAnchor.anchoredPosition = new Vector2(368, 336);
            coinAnchor.DOAnchorPosY(375, 2.25f);
            DOTween.Sequence()
                .Append(coinIcon.DOFade(1, 0.25f))
                .Join(coinText.DOFade(1, 0.25f))
                .Append(coinIcon.DOFade(0, 2f))
                .Join(coinText.DOFade(0, 2f))
                .OnComplete(() => coinAnchor.gameObject.SetActive(false));
        }

        public bool RemoveMoney(int value, bool data = false)
        {
            if (!value.IsNatural() || !HasMoney(value)) return false;
            ServerDataManager.Instance.money[1] += value;
            if (data) ServerDataManager.Instance.money[0] -= value;
            money -= value;
            return true;
        }

        public bool HasMoney(int value) => money >= value;
    }
}