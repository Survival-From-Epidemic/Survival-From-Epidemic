using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStartView : MonoBehaviour
{
    [SerializeField] private Button gameStartButton;
    [SerializeField] private Button settingButton;
    [SerializeField] private Button rankButton;
    [SerializeField] private Button quitButton;

    // Start is called before the first frame update
    // Update is called once per frame
    void Update()
    {
        gameStartButton.onClick.AddListener((() =>
        {
            Debug.Log("game");
        }));
        quitButton.onClick.AddListener((() =>
        {
            Debug.Log("quit");
            Application.Quit();
        }));
    }
}
