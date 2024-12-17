using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject startButton, settingsButton, exitButton;
    
    private void Start()
    {
        if (!(SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Main Menu"))) return;
        
        AudioManager.Instance.PlayMusic("Hip Hop Vol2 Convos Main");
    }
}
