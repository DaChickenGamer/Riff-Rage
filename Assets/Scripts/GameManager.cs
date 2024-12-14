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
        List<Vector3> enemyLocations = new();
        int attempts = 0;

        while (_currentEnemiesAlive < _enemyCount)
        {
            if (attempts > 1000)
            {
                Debug.LogWarning("Unable to spawn all enemies. Ending loop to avoid infinite loop.");
                break;
            }
            attempts++;

            float randomEnemyX = Random.Range(_enemySpawnAreaColliderMinX, _enemySpawnAreaColliderMaxX);
            float randomEnemyY = Random.Range(_enemySpawnAreaColliderMinY, _enemySpawnAreaColliderMaxY);

            Vector3 randomEnemyvector = new Vector3(randomEnemyX, randomEnemyY);
            bool isNearExistingEnemy = false;

            foreach (Vector3 enemyLocation in enemyLocations)
            {
                float distance = Vector3.Distance(randomEnemyvector, enemyLocation);
                if (distance < 10)
                {
                    isNearExistingEnemy = true;
                    break;
                }
            }

            Vector3 viewportPoint = Camera.main.WorldToViewportPoint(randomEnemyvector);
            if (!isNearExistingEnemy &&
                (viewportPoint.x < 0f || viewportPoint.x > 1f || 
                 viewportPoint.y < 0f || viewportPoint.y > 1f))
            {
                enemyLocations.Add(randomEnemyvector);
                _currentEnemiesAlive++;
                Instantiate(enemyPrefab, randomEnemyvector, Quaternion.identity);
            }
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
