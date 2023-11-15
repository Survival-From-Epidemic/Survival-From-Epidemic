using System;
using _root.Scripts.Game;
using TMPro;
using UnityEngine;

namespace _root.Scripts.UI.InGameMenuView
{
    public class InGameMenuView : View
    {
        [SerializeField] private TextMeshProUGUI dateText;
        [SerializeField] private UIImage policyImage;
        [SerializeField] private UIImage adminImage;
        [SerializeField] private UIImage serviceImage;
        [SerializeField] private GameObject policyGrid;
        [SerializeField] private GameObject adminGrid;
        [SerializeField] private GameObject serviceGrid;
        [SerializeField] private GameObject clicker;
        [SerializeField] private TextMeshProUGUI clickerTitle;
        [SerializeField] private TextMeshProUGUI clickerDescription;
        [SerializeField] private GameObject clickerDisable;
        [SerializeField] private GameObject clickerEnable;
        [SerializeField] private TextMeshProUGUI clickerCost;
        [SerializeField] private InGameMenuPage inGameMenuPage = InGameMenuPage.Policy;

        private void Start()
        {
            policyImage.onClickDown.AddListener(_ =>
            {
                policyGrid.SetActive(true);
                adminGrid.SetActive(false);
                serviceGrid.SetActive(false);
            });
            adminImage.onClickDown.AddListener(_ =>
            {
                policyGrid.SetActive(false);
                adminGrid.SetActive(true);
                serviceGrid.SetActive(false);
            });
            serviceImage.onClickDown.AddListener(_ =>
            {
                policyGrid.SetActive(false);
                adminGrid.SetActive(false);
                serviceGrid.SetActive(true);
            });

            Revoker(policyGrid.transform);
            Revoker(adminGrid.transform);
            Revoker(serviceGrid.transform);
        }

        private int SellPricer(DateTime date)
        {
            var today = TimeManager.Instance.today;
            if (date >= today.AddDays(-14)) return 15000;
            if (date >= today.AddDays(-30)) return 8000;
            if (date >= today.AddDays(-60)) return 3500;
            return 500;
        }

        private void Revoker(Transform trans)
        {
            for (var i = trans.childCount - 1; i >= 0; i--)
            {
                var uiImage = trans.GetChild(i).GetComponent<UIImage>();
                var data = LocalDataManager.Instance.GetGridData(uiImage.name);
                uiImage.onClickDown.AddListener(_ =>
                {
                    clicker.SetActive(true);
                    clickerTitle.text = data.name;
                    clickerDescription.text = data.message;
                    clickerCost.text = LocalDataManager.Instance.IsBought(data.name)
                        ? $"{data.weight * 30000}"
                        : $"{data.weight * SellPricer(LocalDataManager.Instance.GetBuy(data.name) ?? TimeManager.Instance.today)}";
                });
            }
        }

        public override void OnTimeChanged(DateTime dateTime)
        {
            dateText.text = dateTime.ToShortDateString();
        }
    }
}