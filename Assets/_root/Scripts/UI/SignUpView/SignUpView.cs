using _root.Scripts.Game;
using _root.Scripts.Managers;
using _root.Scripts.Managers.UI;
using _root.Scripts.Network;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _root.Scripts.UI.SignUpView
{
    public class SignUpView : RectBackgroundView
    {
        [SerializeField] private TMP_InputField idInputField;
        [SerializeField] private TMP_InputField passwordInputField;

        [SerializeField] private Button signInButton;

        private void Start()
        {
            signInButton.onClick.AddListener(() => UIManager.Instance.EnableUI(UIElements.SignIn));
        }

        protected override void Update()
        {
            base.Update();
            if (Input.GetKeyDown(KeyCode.Return))
                if (idInputField.text.Length != 0 && passwordInputField.text.Length != 0)
                    new Networking.Post<string>("/users/signup", new SignUpRequest
                        {
                            accountId = idInputField.text,
                            password = passwordInputField.text
                        })
                        .OnResponse(value =>
                        {
                            Debugger.Log("회원가입 성공");
                            Debugger.Log(value);
                        })
                        .Build();
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (idInputField.isFocused)
                {
                    passwordInputField.Select();
                }
            }
        }

        private void OnEnable()
        {
            idInputField.text = "";
            passwordInputField.text = "";
        }
    }
}