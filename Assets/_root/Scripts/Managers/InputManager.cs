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
            else if (Input.GetKeyDown(KeyCode.Tab) && NewsObject.Instance.isActiveAndEnabled)
            {
                NewsObject.Instance.gameObject.SetActive(false);
            }
            else if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Q)) && UIManager.Instance.GetKey() is UIElements.InGameMenu)
            {
                UIManager.Instance.EnableUI(UIElements.InGame);
            }
        }
    }
}