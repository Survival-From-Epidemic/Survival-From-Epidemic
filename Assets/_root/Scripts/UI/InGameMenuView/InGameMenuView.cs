using System;
using TMPro;
using UnityEngine;

namespace _root.Scripts.UI.InGameMenuView
{
    public class InGameMenuView : View
    {
        [SerializeField] private TextMeshProUGUI dateText;


        public override void OnTimeChanged(DateTime dateTime)
        {
            dateText.text = dateTime.ToShortDateString();
        }
    }
}