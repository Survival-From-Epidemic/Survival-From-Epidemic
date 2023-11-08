using System;
using System.Collections.Generic;
using _root.Scripts.Managers.UI;
using _root.Scripts.SingleTon;
using UnityEngine;

namespace _root.Scripts.Managers
{
    public class UIManager : SingleMono<UIManager>
    {
        [SerializeField] private List<UIElement> elements;
        [SerializeField] private UIElements startUIKey;
        private GameObject _currentUI;

        private Dictionary<UIElements, UIElement> _elementDictionary;

        private void Start()
        {
            _elementDictionary = new Dictionary<UIElements, UIElement>();

            foreach (var uiElement in elements)
            {
                _elementDictionary.Add(uiElement.key, uiElement);
                uiElement.targetUI.gameObject.SetActive(false);
            }

            (_currentUI = _elementDictionary[startUIKey].targetUI.gameObject).SetActive(true);
        }

        public void EnableUI(UIElements key)
        {
            _currentUI.SetActive(false);
            (_currentUI = _elementDictionary[key].targetUI.gameObject).SetActive(true);
        }

        [Serializable]
        public struct UIElement
        {
            public UIElements key;
            public Canvas targetUI;
        }
    }
}