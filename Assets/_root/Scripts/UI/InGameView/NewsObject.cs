using _root.Scripts.Game;
using _root.Scripts.SingleTon;
using UnityEngine;
using UnityEngine.UI;

namespace _root.Scripts.UI.InGameView
{
    public class NewsObject : SingleMono<NewsObject>
    {
        public UIImage close;
        public Image image;
        private float _preRealGameTimeScale;
        private float _preTimeScale;

        protected override void Awake()
        {
            base.Awake();
            image = GetComponent<Image>();
            close.onClickDown.AddListener(_ => gameObject.SetActive(false));
            _preRealGameTimeScale = Time.timeScale;
            _preTimeScale = TimeManager.Instance.timeScale;
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            _preRealGameTimeScale = Time.timeScale;
            _preTimeScale = TimeManager.Instance.timeScale;
            Time.timeScale = 0;
        }

        private void OnDisable()
        {
            Time.timeScale = _preRealGameTimeScale;
            TimeManager.Instance.timeScale = _preTimeScale;
        }
    }
}