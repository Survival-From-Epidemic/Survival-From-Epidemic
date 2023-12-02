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
        [SerializeField] private Button settingButton;
        [SerializeField] private Button rankButton;
        [SerializeField] private Button quitButton;
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
        }
    }
}