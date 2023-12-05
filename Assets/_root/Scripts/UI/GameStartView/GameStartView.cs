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
                howTo.SetActive(true);
                howTo.transform.GetChild(0).gameObject.SetActive(true);
                howTo.transform.GetChild(1).gameObject.SetActive(false);
                howTo.transform.GetChild(2).gameObject.SetActive(false);
                howTo.transform.GetChild(3).gameObject.SetActive(false);
                howTo.transform.GetChild(4).gameObject.SetActive(false);
            });
        }

        protected override void Update()
        {
            base.Update();
            if (howTo.activeSelf)
            {
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    howTo.SetActive(false);
                }
                else if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) && howToIdx > 0)
                {
                    howTo.transform.GetChild(howToIdx - 1).gameObject.SetActive(true);
                    howTo.transform.GetChild(howToIdx).gameObject.SetActive(false);
                    howToIdx--;
                }
                else if ((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) && howToIdx < howTo.transform.childCount - 1)
                {
                    howTo.transform.GetChild(howToIdx + 1).gameObject.SetActive(true);
                    howTo.transform.GetChild(howToIdx).gameObject.SetActive(false);
                    howToIdx++;
                }
            }
        }
    }
}