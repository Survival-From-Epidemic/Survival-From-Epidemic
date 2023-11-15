using _root.Scripts.Managers;
using _root.Scripts.Managers.UI;
using _root.Scripts.Network;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _root.Scripts.UI.SignUpView
{
    public class SignUpView : View
    {
        [SerializeField] private TMP_InputField idInputField;
        [SerializeField] private TMP_InputField passwordInputField;

        [SerializeField] private Button signInButton;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
                if (idInputField.text.Length != 0 && passwordInputField.text.Length != 0)
                    new Networking.Post<string>("/users", new SignUpRequest
                        {
                            accountId = idInputField.text,
                            password = passwordInputField.text
                        })
                        .OnResponse(Debug.unityLogger.Log)
                        .Build();
            signInButton.onClick.AddListener(() => UIManager.Instance.EnableUI(UIElements.SignIn));
        }
    }
}