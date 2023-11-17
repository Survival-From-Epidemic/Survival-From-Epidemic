using _root.Scripts.Game;
using _root.Scripts.SingleTon;
using UnityEngine;
using UnityEngine.UI;

namespace _root.Scripts.UI.InGameView
{
    public class NewsObject : SingleMono<NewsObject>
    {
        private float _preTimeScale;
        private float _preRealGameTimeScale;
        public UIImage close;
        public Image image;

        private void Start()
        {
            image = GetComponent<Image>();
            close.onClickDown.AddListener(_ => gameObject.SetActive(false));
        }

        private void OnEnable()
        {
            _preRealGameTimeScale = Time.timeScale;
            _preTimeScale = TimeManager.Instance.timeScale;
        }

        private void OnDisable()
        {
            Time.timeScale = _preRealGameTimeScale;
            TimeManager.Instance.timeScale = _preTimeScale;
        }
    }
}