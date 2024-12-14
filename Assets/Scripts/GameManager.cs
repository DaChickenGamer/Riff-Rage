using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI waveCounterText;
    
    public Collider2D enemySpawnAreaCollider;
    public GameObject enemyPrefab;

    private int _roundNumber = 0;
    private int _enemyCount;

    private int _currentEnemiesAlive;

    private float _enemySpawnAreaColliderMinX, _enemySpawnAreaColliderMinY, _enemySpawnAreaColliderMaxX, _enemySpawnAreaColliderMaxY;
    private float _playerCameraMinX, _playerCameraMaxX, _playerCameraMinY, _playerCameraMaxY;
    
    private void Start()
    {
        _enemySpawnAreaColliderMaxX = enemySpawnAreaCollider.bounds.max.x;
        _enemySpawnAreaColliderMaxY = enemySpawnAreaCollider.bounds.max.y;
        _enemySpawnAreaColliderMinX = enemySpawnAreaCollider.bounds.min.x;
        _enemySpawnAreaColliderMinY = enemySpawnAreaCollider.bounds.min.y;
        
        StartRound();
    }

    private void Update()
    {
        if (_currentEnemiesAlive <= 0 && _roundNumber != 0)
            EndRound();
    }

    private void StartRound()
    {
        _enemyCount = 4 + _roundNumber ^ 2;
        
        _roundNumber++;

        waveCounterText.alpha = 1;
        waveCounterText.text = "Wave " + _roundNumber.ToString();

        StartCoroutine(FadeOutStartRoundText());
        SpawnEnemies();
    }

    private void EndRound()
    {
        StartCoroutine(WaitForNextRound());
        StartRound();

    }
    
    private void SpawnEnemies()
{
    int gridRows = 10;
    int gridColumns = 10;

    float cellWidth = (_enemySpawnAreaColliderMaxX - _enemySpawnAreaColliderMinX) / gridColumns;
    float cellHeight = (_enemySpawnAreaColliderMaxY - _enemySpawnAreaColliderMinY) / gridRows;

    List<Vector3> potentialSpawnPoints = new();

    for (int x = 0; x < gridColumns; x++)
    {
        for (int y = 0; y < gridRows; y++)
        {
            float spawnX = _enemySpawnAreaColliderMinX + x * cellWidth + cellWidth / 2;
            float spawnY = _enemySpawnAreaColliderMinY + y * cellHeight + cellHeight / 2;
            
            Vector3 spawnPos = new Vector3(spawnX, spawnY, 0);
            
            Vector3 spawnViewPortPoint = Camera.main.WorldToViewportPoint(spawnPos);
            
            if (spawnViewPortPoint.x < 0f || 
                spawnViewPortPoint.x > 1f || 
                spawnViewPortPoint.y < 0f || 
                spawnViewPortPoint.y > 1f)
                potentialSpawnPoints.Add(new Vector3(spawnX, spawnY));
        }
    }

    for (int i = 0; i < potentialSpawnPoints.Count; i++)
    {
        int randomIndex = Random.Range(0, potentialSpawnPoints.Count);
        Vector3 temp = potentialSpawnPoints[i];
        potentialSpawnPoints[i] = potentialSpawnPoints[randomIndex];
        potentialSpawnPoints[randomIndex] = temp;
    }

    for (int i = 0; i < _enemyCount && i < potentialSpawnPoints.Count; i++)
    {
        Vector3 spawnPoint = potentialSpawnPoints[i];
        Instantiate(enemyPrefab, spawnPoint, Quaternion.identity);
        _currentEnemiesAlive++;
    }
}

    
    IEnumerator FadeOutStartRoundText()
    {
        int _currentFadeOutStep = 50;
        
        while (waveCounterText.alpha > 0)
        {
            waveCounterText.alpha -=  1f/_currentFadeOutStep;
            _currentFadeOutStep -= 1;
            yield return new WaitForSeconds(.1f);
        }
    }
    IEnumerator WaitForNextRound()
    {
        yield return new WaitForSeconds(5f);
    }
    
     
}
