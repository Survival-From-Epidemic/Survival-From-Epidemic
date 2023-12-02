using _root.Scripts.Game;
using _root.Scripts.Managers.Sound;
using _root.Scripts.Managers.UI;
using _root.Scripts.UI.InGameView;
using UnityEngine;

namespace _root.Scripts.Managers
{
    public class InputManager : MonoBehaviour
    {
        // private float _xRotate;
        // private float _xRotateMove;
        // private float _yRotate;
        // private float _yRotateMove;
        //
        // private void Start()
        // {
        //     _xRotate = _yRotate = _xRotateMove = _yRotateMove = 0;
        // }

        private void Update()
        {
            if (UIManager.Instance.GetKey() is not UIElements.InGameMenu)
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    UIManager.Instance.EnableUI(UIElements.InGameMenu);
                }
            }

            if (Input.GetKeyDown(KeyCode.Tab) && NewsObject.Instance.isActiveAndEnabled)
            {
                NewsObject.Instance.gameObject.SetActive(false);
            }

            // Cursor.lockState = UIManager.Instance.GetKey() is not UIElements.InGame || Input.GetKey(KeyCode.LeftAlt) || NewsObject.Instance.isActiveAndEnabled
            //     ? CursorLockMode.None
            //     : CursorLockMode.Locked;

            if (UIManager.Instance.GetKey() is not UIElements.InGame) return;
            if (!NewsObject.Instance.isActiveAndEnabled)
            {
                if (Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.BackQuote) || Input.GetKeyDown(KeyCode.Tilde))
                {
                    TimeManager.SpeedCycle(0);
                    SoundManager.Instance.PlayEffectSound(SoundKey.ClickSound);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    TimeManager.SpeedCycle(1);
                    SoundManager.Instance.PlayEffectSound(SoundKey.ClickSound);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    TimeManager.SpeedCycle(2);
                    SoundManager.Instance.PlayEffectSound(SoundKey.ClickSound);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    TimeManager.SpeedCycle(3);
                    SoundManager.Instance.PlayEffectSound(SoundKey.ClickSound);
                }
            }

            var velocity = Vector3.zero;

            if (Input.GetKey(KeyCode.W)) velocity += Vector3.forward;
            if (Input.GetKey(KeyCode.A)) velocity += Vector3.left;
            if (Input.GetKey(KeyCode.D)) velocity += Vector3.right;
            if (Input.GetKey(KeyCode.S)) velocity += Vector3.back;
            if (Input.GetKey(KeyCode.Space)) velocity += Vector3.up;
            if (Input.GetKey(KeyCode.LeftShift)) velocity += Vector3.down;

            velocity = velocity.normalized;

            // _xRotateMove = -Input.GetAxis("Mouse Y") * Time.deltaTime;
            // _yRotateMove = Input.GetAxis("Mouse X") * Time.deltaTime;
            //
            // _yRotate = transform.eulerAngles.y + _yRotateMove;
            // _xRotate += _xRotateMove;
            //
            // _xRotate = Mathf.Clamp(_xRotate, -90, 90);
            //
            // Player.Player.Instance.transform.eulerAngles = new Vector3(_xRotate, _yRotate, 0);
            //
            Player.Player.Instance.transform.Translate(Player.Player.Instance.speed * Time.deltaTime * velocity);
        }
    }
}