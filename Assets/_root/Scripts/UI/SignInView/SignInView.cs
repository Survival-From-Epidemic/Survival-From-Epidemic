using _root.Scripts.Game;
using _root.Scripts.Managers;
using _root.Scripts.Managers.UI;
using _root.Scripts.Network;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _root.Scripts.UI.SignInView
{
    public class SignInView : RectBackgroundView
    {
        [SerializeField] private TMP_InputField idInputField;
        [SerializeField] private TMP_InputField passwordInputField;

        [SerializeField] private Button findPwButton;
        [SerializeField] private Button signUpButton;

        private void Start()
        {
            signUpButton.onClick.AddListener(() => UIManager.Instance.EnableUI(UIElements.SignUp));
        }

        protected override void Update()
        {
            base.Update();
            if (Input.GetKeyDown(KeyCode.Return))
                if (idInputField.text.Length != 0 && passwordInputField.text.Length != 0)
                    new Networking.Post<SignInResponse>("/auth/tokens", new SignInRequest
                        {
                            accountId = idInputField.text,
                            password = passwordInputField.text
                        })
                        .OnResponse(data =>
                        {
                            UIManager.Instance.EnableUI(UIElements.GameStart);
                            PlayerPrefs.SetString("Token", data.accessToken);
                            Debugger.Log(data.accessToken);
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