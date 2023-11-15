using System;
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
        [SerializeField] private InGameMenuPage inGameMenuPage = InGameMenuPage.Policy;

        private void Start()
        {
            policyImage.onClickDown.AddListener(_ => policyGrid.SetActive(true));
            adminImage.onClickDown.AddListener(_ => adminGrid.SetActive(true));
            serviceImage.onClickDown.AddListener(_ => serviceGrid.SetActive(true));
        }

        public override void OnTimeChanged(DateTime dateTime)
        {
            dateText.text = dateTime.ToShortDateString();
        }
    }
}