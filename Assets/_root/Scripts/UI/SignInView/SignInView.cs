using _root.Scripts.Network;
using SingleTon;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _root.Scripts.UI.SignInView
{
    public class SignInView : SingleMonoComponent<SignInView, Canvas>
    {
        [SerializeField] private TMP_InputField idInputField;
        [SerializeField] private TMP_InputField passwordInputField;

        [SerializeField] private Button findPwButton;
        [SerializeField] private Button signUpButton;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
                if (idInputField.text.Length != 0 && passwordInputField.text.Length != 0)
                    new Networking.Post<string>("/auth/tokens", new SignInRequest
                        {
                            accountId = idInputField.text,
                            password = passwordInputField.text
                        })
                        .OnResponse(Debug.Log)
                        .Build();
        }
    }
}