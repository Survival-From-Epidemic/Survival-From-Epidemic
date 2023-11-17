using _root.Scripts.Managers.UI;
using _root.Scripts.UI.InGameView;
using UnityEngine;

namespace _root.Scripts.Managers
{
    public class InputManager : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                UIManager.Instance.EnableUI(UIElements.InGameMenu);
            }

            if (Input.GetKeyDown(KeyCode.Tab) && NewsObject.Instance.isActiveAndEnabled)
            {
                NewsObject.Instance.gameObject.SetActive(false);
            }

            if (Input.GetKeyDown(KeyCode.Escape) && UIManager.Instance.GetKey() is UIElements.InGameMenu)
            {
                UIManager.Instance.EnableUI(UIElements.InGame);
            }
        }
    }
}