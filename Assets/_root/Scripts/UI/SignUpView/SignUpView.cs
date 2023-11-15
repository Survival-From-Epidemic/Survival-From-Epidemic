using _root.Scripts.Managers;
using _root.Scripts.Managers.UI;
using _root.Scripts.Network;
using _root.Scripts.SingleTon;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _root.Scripts.UI.SignUpView
{
    public class SignUpView : SingleMonoComponent<SignUpView, Canvas>
    {
        [SerializeField] private TMP_InputField idInputField;
        [SerializeField] private TMP_InputField passwordInputField;

        [SerializeField] private Button signInButton;

        private void OnEnable()
        {
            idInputField.text = "";
            passwordInputField.text = "";
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
                if (idInputField.text.Length != 0 && passwordInputField.text.Length != 0)
                    new Networking.Post<string>("/users/signup", new SignUpRequest
                        {
                            accountId = idInputField.text,
                            password = passwordInputField.text
                        })
                        .OnResponse(value =>
                        {
                            Debug.Log("회원가입 성공");
                            Debug.Log(value);
                        })
                        .Build();
            signInButton.onClick.AddListener((() =>
            {
                UIManager.Instance.EnableUI(UIElements.SignIn);
            }));
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (idInputField.isFocused == true)
                {
                    passwordInputField.Select();
                }
            }
        }
    }
}