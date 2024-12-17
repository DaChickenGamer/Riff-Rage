using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TextMeshProUGUI _text;
    private Vector3 _originalScale;

    private void Start()
    {
        _text = GetComponent<TextMeshProUGUI>();
        _originalScale = transform.localScale;
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
    }

    
    public void OnExitButtonClicked()
    {
        AudioManager.Instance.PlaySFX("switch_007");
        Application.Quit();
    }
}
