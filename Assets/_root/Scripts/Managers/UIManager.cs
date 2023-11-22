using System;
using System.Collections.Generic;
using _root.Scripts.Managers.UI;
using _root.Scripts.SingleTon;
using _root.Scripts.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _root.Scripts.Managers
{
    public class UIManager : SingleMono<UIManager>
    {
        [SerializeField] private List<UIElement> elements;
        [SerializeField] private UIElements startUIKey;
        private View _currentUI;
        private UIElements _currentUIKey;

        private Dictionary<UIElements, UIElement> _elementDictionary;

        private void Start()
        {
            _elementDictionary = new Dictionary<UIElements, UIElement>();

            foreach (var uiElement in elements)
            {
                _elementDictionary.Add(uiElement.key, uiElement);
                if (uiElement.targetUI)
                {
                    uiElement.targetUI.gameObject.SetActive(false);
                }
            }

            var element = _elementDictionary[startUIKey];

            if (SceneManager.GetActiveScene().name != element.scene.GetString()) SceneManager.LoadScene(element.scene.GetString());

            (_currentUI = element.targetUI)?.gameObject.SetActive(true);
            _currentUIKey = startUIKey;
        }

        public UIElements GetKey() => _currentUIKey;

        public void EnableUI(UIElements key)
        {
            if (_currentUI) _currentUI.gameObject.SetActive(false);
            _currentUIKey = key;
            var element = _elementDictionary[key];
            if (SceneManager.GetActiveScene().name != element.scene.GetString()) SceneManager.LoadScene(element.scene.GetString());
            (_currentUI = element.targetUI)?.gameObject.SetActive(true);
        }

        public void UpdateTime(DateTime dateTime)
        {
            _currentUI.OnTimeChanged(dateTime);
        }

        [Serializable]
        public struct UIElement
        {
            public UIElements key;
            public UIScenes scene;
            public View targetUI;
        }
    }
}