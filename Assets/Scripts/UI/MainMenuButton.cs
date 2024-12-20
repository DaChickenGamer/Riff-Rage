using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MainMenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TextMeshProUGUI _text;
    private Vector3 _originalScale;

    private GameObject _defaultMainMenu, _settingsMenu, _audioMenu, _graphicsMenu;
    
    private void Start()
    {
        _text = GetComponent<TextMeshProUGUI>();
        _originalScale = transform.localScale;

        GameObject canvas = transform.parent.parent.gameObject;
    
        foreach (Transform child in canvas.transform)
        {
            switch (child.gameObject.name)
            {
                case "Default Main Menu":
                    _defaultMainMenu = child.gameObject;
                    break;
                case "Settings Menu":
                    _settingsMenu = child.gameObject;
                    break;
                case "Audio Menu":
                    _audioMenu = child.gameObject;
                    break;
                case "Graphics Menu":
                    _graphicsMenu = child.gameObject;
                    break;
                default:
                    Debug.LogWarning($"Could not find child {child.gameObject.name}");
                    break;
            }
        }

        if (transform.name == "Full Screen Button")
            GetComponentInParent<TextMeshProUGUI>().text = Screen.fullScreen ? "Go Windowed" : "Go Fullscreen";
        
        if (transform.name == "Fullscreen Resolution Button")
        {
            UpdateResolutionText();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.Instance.PlaySFX("switch_006");
        _text.color = new Color32(193, 193, 193, 255);
        transform.localScale = _originalScale * 1.2f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _text.color = new Color32(255, 255, 255, 255);
        transform.localScale = _originalScale;
    }

    public void OnStartButtonClicked()
    {
        AudioManager.Instance.PlaySFX("switch_007");
        SceneManager.LoadScene(1);
    }
    
    public void OnSettingsButtonClicked()
    {
        AudioManager.Instance.PlaySFX("switch_007");
        _defaultMainMenu.SetActive(false);
        _settingsMenu.SetActive(true);
    }

    
    public void OnExitButtonClicked()
    {
        AudioManager.Instance.PlaySFX("switch_007");
        Application.Quit();
    }

    public void OnAudioButtonClicked()
    {
        AudioManager.Instance.PlaySFX("switch_007");
        _settingsMenu.SetActive(false);
        _audioMenu.SetActive(true);

    }

    public void OnGraphicButtonClicked()
    {
        AudioManager.Instance.PlaySFX("switch_007");


        foreach (Transform child in _graphicsMenu.transform)
        {
            if (child.name != "Fullscreen Resolution Button") continue;

            UIManager.Instance.SetResolutionNumber(UIManager.Instance.GetCurrentScreenResolutionNumber());
            child.GetComponent<TextMeshProUGUI>().text = "Resolution: " + UIManager.Instance.GetResolutionText();
            break;
        }

        _settingsMenu.SetActive(false);
        _graphicsMenu.SetActive(true);
    }

    public void OnSettingsMenuBackButtonClicked()
    {
        AudioManager.Instance.PlaySFX("switch_007");
        _defaultMainMenu.SetActive(true);
        _settingsMenu.SetActive(false);
    }

    public void OnFullScreenButtonClicked()
    {
        AudioManager.Instance.PlaySFX("switch_007");

        if (Screen.fullScreen)
        {
            Screen.fullScreen = false;
            GetComponentInParent<TextMeshProUGUI>().text = "Go Fullscreen";
        }
        else
        {
            Screen.fullScreen = true;
            GetComponentInParent<TextMeshProUGUI>().text = "Go Windowed";
        }
    }
    
    public void OnResolutionButtonClicked()
    {
        AudioManager.Instance.PlaySFX("switch_007");
        
        if (UIManager.Instance.GetResolutionNumber() < UIManager.Instance.GetResolutionCount() - 1)
        {
            UIManager.Instance.NextResolution();
            UpdateResolutionText(); 
        }
        else
        {
            UIManager.Instance.SetResolutionNumber(0); 
            UpdateResolutionText();
        }
    }

    private void UpdateResolutionText()
    {
        GetComponentInParent<TextMeshProUGUI>().text = $"Resolution: {UIManager.Instance.GetResolutionText()}";
    }

    public void OnApplyButtonClicked()
    {
        AudioManager.Instance.PlaySFX("switch_007");

        UIManager.Instance.ApplyResolution();
    }

    public void OnGraphicsMenuBackButtonClicked()
    {
        AudioManager.Instance.PlaySFX("switch_007");
        
        _graphicsMenu.SetActive(false);
        _settingsMenu.SetActive(true);
    }

    public void OnAudioMenuBackButtonClicked()
    {
        AudioManager.Instance.PlaySFX("switch_007");
        _audioMenu.SetActive(false);
        _settingsMenu.SetActive(true);
    }
}

