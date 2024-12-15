using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugPanel : MonoBehaviour
{
    private GameObject _background;
    private TextMeshProUGUI _roundNumberText;
    private TextMeshProUGUI _enemiesAliveText;
    private TextMeshProUGUI _maxEnemiesAtOnceText;
    private TextMeshProUGUI _enemiesSpawnedThisRoundText;
    private TextMeshProUGUI _maxEnemiesPerRoundText;
    
    private void Start()
    {
        _background = GameObject.Find("Background");
        
        _roundNumberText = CreateNumberStatDebug("Round Number", GameManager.Instance.GetRoundNumber());
        _roundNumberText.transform.SetParent(_background.transform);
        
        _enemiesAliveText = CreateNumberStatDebug("Enemies Alive", GameManager.Instance.GetCurrentEnemiesAlive());
        _enemiesAliveText.transform.SetParent(_background.transform);
        
        _maxEnemiesAtOnceText = CreateNumberStatDebug("Max Enemies At Once", GameManager.Instance.GetMaxEnemyCountAtOnce());
        _maxEnemiesAtOnceText.transform.SetParent(_background.transform);
        
        _enemiesSpawnedThisRoundText = CreateNumberStatDebug("Enemies Spawned This Round", GameManager.Instance.GetTotalEnemiesSpawnedThisRound());
        _enemiesSpawnedThisRoundText.transform.SetParent(_background.transform);
        
        _maxEnemiesPerRoundText = CreateNumberStatDebug("Max Enemies Per Round", GameManager.Instance.GetMaxEnemyCountPerRound());
        _maxEnemiesPerRoundText.transform.SetParent(_background.transform);
    }

    private void Update()
    {
        _roundNumberText.text = "Round Number: " + GameManager.Instance.GetRoundNumber();
        _enemiesAliveText.text = "Enemies Alive: " + GameManager.Instance.GetCurrentEnemiesAlive();
        _maxEnemiesAtOnceText.text = "Max Enemies At Once: " + GameManager.Instance.GetMaxEnemyCountAtOnce();
        _enemiesSpawnedThisRoundText.text = "Enemies Spawned This Round: " + GameManager.Instance.GetTotalEnemiesSpawnedThisRound();
        _maxEnemiesPerRoundText.text = "Max Enemies Per Round: " + GameManager.Instance.GetMaxEnemyCountPerRound();
    }

    private TextMeshProUGUI CreateNumberStatDebug(string statName, int statValue)
    {
        GameObject panel = new GameObject();
        panel.name = statName + " Panel";
    
        RectTransform rectTransform = panel.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(350, 70); 

        Image panelBackground = panel.AddComponent<Image>();
        panelBackground.color = new Color(125f / 255f, 125f / 255f, 125f / 255f); 
        panelBackground.gameObject.AddComponent<HorizontalLayoutGroup>().childAlignment = TextAnchor.MiddleCenter;
    
        TextMeshProUGUI statNameTextBox = new GameObject().AddComponent<TextMeshProUGUI>();
        statNameTextBox.name = "Stat Name";
        statNameTextBox.text = statName;
        statNameTextBox.textWrappingMode = TextWrappingModes.Normal;
        statNameTextBox.rectTransform.SetParent(panel.transform);
        statNameTextBox.rectTransform.sizeDelta = new Vector2(350, 30);
    
        TextMeshProUGUI statValueTextBox = new GameObject().AddComponent<TextMeshProUGUI>();
        statValueTextBox.name = "Stat Value";
        statValueTextBox.text = statValue.ToString();
        statValueTextBox.textWrappingMode = TextWrappingModes.Normal;
        statValueTextBox.rectTransform.SetParent(panel.transform);
        statValueTextBox.rectTransform.sizeDelta = new Vector2(350, 30);

        return statValueTextBox;
    }
}
