using _root.Scripts.Managers;
using _root.Scripts.Managers.UI;
using UnityEngine;
using UnityEngine.UI;

namespace _root.Scripts.UI.GameStartView
{
    public class GameStartView : View
    {
        [SerializeField] private Button gameStartButton;
        [SerializeField] private Button settingButton;
        [SerializeField] private Button rankButton;
        [SerializeField] private Button quitButton;

        // Start is called before the first frame update
        // Update is called once per frame
        void Update()
        {
            gameStartButton.onClick.AddListener((() => { UIManager.Instance.EnableUI(UIElements.InGame); }));
            quitButton.onClick.AddListener((() =>
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }));
        }
    }
}
