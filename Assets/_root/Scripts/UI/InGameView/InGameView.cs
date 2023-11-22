using System;
using _root.Scripts.Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _root.Scripts.UI.InGameView
{
    public class InGameView : GameView
    {
        [SerializeField] private TextMeshProUGUI banbalText;
        [SerializeField] private Image banbalImage;
        [SerializeField] private TextMeshProUGUI authorityText;
        [SerializeField] private Image authorityImage;
        [SerializeField] private TextMeshProUGUI healthyText;
        [SerializeField] private TextMeshProUGUI infectText;
        [SerializeField] private TextMeshProUGUI deadText;
        [SerializeField] private Image vaccineGageImage;
        [SerializeField] private Image vaccineBackground;
        [SerializeField] private TextMeshProUGUI vaccinePercentText;

        public override void OnTimeChanged(DateTime dateTime)
        {
            base.OnTimeChanged(dateTime);
            banbalText.text = $"{Mathf.FloorToInt(ValueManager.Instance.banbal * 100):n0}%";
            banbalImage.fillAmount = ValueManager.Instance.banbal;

            authorityText.text = $"{100 - Mathf.FloorToInt(ValueManager.Instance.authority * 100):n0}%";
            authorityImage.fillAmount = 1 - ValueManager.Instance.authority;

            healthyText.text = $"{ValueManager.Instance.person.healthyPerson}";
            infectText.text = $"{ValueManager.Instance.person.infectedPerson}";
            deadText.text = $"{ValueManager.Instance.person.deathPerson}";
            if (ValueManager.Instance.vaccineResearch)
            {
                vaccineBackground.gameObject.SetActive(true);
                var vaccinePercent = vaccineGageImage.fillAmount = (float)TimeManager.Instance.GetVaccinePercent();
                vaccinePercentText.text = $"{vaccinePercent * 100:n0}%";
            }
            else
            {
                vaccineBackground.gameObject.SetActive(false);
                vaccineGageImage.fillAmount = 0;
                vaccinePercentText.text = "";
            }
        }
    }
}