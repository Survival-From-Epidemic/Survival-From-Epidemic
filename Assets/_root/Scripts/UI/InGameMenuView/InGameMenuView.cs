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

            var policyGridTransform = policyGrid.transform;
            for (var i = policyGridTransform.childCount - 1; i >= 0; i--)
            {
                var uiImage = policyGridTransform.GetChild(i).GetComponent<UIImage>();
                var data = LocalDataManager.Instance.GetGridData(uiImage.name);
                uiImage.onClickDown.AddListener(_ =>
                {
                    clicker.SetActive(true);
                    clickerTitle.text = data.name;
                    clickerDescription.text = data.message;
                    clickerCost.text = $"{data.weight * 30000}";
                });
            }

            var adminGridTransform = adminGrid.transform;
            for (var i = adminGridTransform.childCount - 1; i >= 0; i--)
            {
                var uiImage = adminGridTransform.GetChild(i).GetComponent<UIImage>();
                uiImage.onClickDown.AddListener(_ => { });
            }

            var serviceGridTransform = serviceGrid.transform;
            for (var i = serviceGridTransform.childCount - 1; i >= 0; i--)
            {
                var uiImage = serviceGridTransform.GetChild(i).GetComponent<UIImage>();
                uiImage.onClickDown.AddListener(_ => { });
            }
        }

        public override void OnTimeChanged(DateTime dateTime)
        {
            dateText.text = dateTime.ToShortDateString();
        }
    }
}