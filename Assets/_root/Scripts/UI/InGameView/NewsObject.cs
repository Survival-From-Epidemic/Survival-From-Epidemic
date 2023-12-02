using _root.Scripts.Game;
using _root.Scripts.SingleTon;
using UnityEngine.UI;

namespace _root.Scripts.UI.InGameView
{
    public class NewsObject : SingleMono<NewsObject>
    {
        public UIImage close;
        public Image image;

        protected override void Awake()
        {
            base.Awake();
            image = GetComponent<Image>();
            close.onClickDown.AddListener(_ => gameObject.SetActive(false));
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            TimeManager.Pause();
        }

        private void OnDisable()
        {
            TimeManager.UnPause();
        }
    }
}