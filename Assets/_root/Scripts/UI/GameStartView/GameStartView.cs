using _root.Scripts.Managers;
using _root.Scripts.Managers.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace _root.Scripts.UI.GameStartView
{
    public class GameStartView : RectBackgroundView
    {
        [SerializeField] private Button gameStartButton;
        [SerializeField] private Button howToPlayButton;
        [SerializeField] private Button logOutButton;
        [SerializeField] private Button quitButton;
        [SerializeField] private GameObject howTo;
        [SerializeField] private int howToIdx;
        private bool _sceneLoading;

        private void Start()
        {
            _sceneLoading = false;
            // StartCoroutine(SceneLoad());
            gameStartButton.onClick.AddListener(() =>
            {
                if (_sceneLoading) return;
                _sceneLoading = true;
                UIManager.Instance.EnableUI(UIElements.InGame);
            });
            quitButton.onClick.AddListener(() =>
            {
#if UNITY_EDITOR
                EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            });
            howToPlayButton.onClick.AddListener(() =>
            {
                howToIdx = 0;
                howTo.gameObject.SetActive(true);
            });
        }

        protected override void Update()
        {
            base.Update();
            if (howTo.activeSelf)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    howTo.gameObject.SetActive(false);
                }
                else if (Input.GetKeyDown(KeyCode.A) && howToIdx > 0)
                {
                    howTo.transform.GetChild(howToIdx - 1).gameObject.SetActive(true);
                    howTo.transform.GetChild(howToIdx).gameObject.SetActive(false);
                    howToIdx--;
                }
                else if (Input.GetKeyDown(KeyCode.D) && howToIdx < howTo.transform.childCount - 1)
                {
                    howTo.transform.GetChild(howToIdx + 1).gameObject.SetActive(true);
                    howTo.transform.GetChild(howToIdx).gameObject.SetActive(false);
                    howToIdx++;
                }
            }
        }
    }
}