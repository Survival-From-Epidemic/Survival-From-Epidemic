using System;
using System.Collections.Generic;
using _root.Scripts.Managers.UI;
using _root.Scripts.SingleTon;
using _root.Scripts.UI;
using UnityEngine;

namespace _root.Scripts.Managers
{
    public class UIManager : SingleMono<UIManager>
    {
        [SerializeField] private List<UIElement> elements;
        [SerializeField] private UIElements startUIKey;
        private View _currentUI;

        private Dictionary<UIElements, UIElement> _elementDictionary;

        private void Start()
        {
            _elementDictionary = new Dictionary<UIElements, UIElement>();

            foreach (var uiElement in elements)
            {
                _elementDictionary.Add(uiElement.key, uiElement);
                uiElement.targetUI.gameObject.SetActive(false);
            }

            (_currentUI = _elementDictionary[startUIKey].targetUI).gameObject.SetActive(true);
        }

        public void EnableUI(UIElements key)
        {
            _currentUI.gameObject.SetActive(false);
            (_currentUI = _elementDictionary[key].targetUI).gameObject.SetActive(true);
        }

        public void UpdateTime(DateTime dateTime)
        {
            _currentUI.OnTimeChanged(dateTime);
        }

        [Serializable]
        public struct UIElement
        {
            public UIElements key;
            public View targetUI;
        }
    }
}