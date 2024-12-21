using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI waveCounterText;
    
    public Collider2D enemySpawnAreaCollider;
    public GameObject enemyPrefab;

    public static GameManager Instance;
    
    private int _roundNumber;
    private int _maxEnemyCountPerRound;
    private int _maxEnemyCountAtOnce;

    private int _currentEnemiesAlive;
    private int _totalEnemiesSpawnedThisRound;

    private float _enemySpawnAreaColliderMinX, _enemySpawnAreaColliderMinY, _enemySpawnAreaColliderMaxX, _enemySpawnAreaColliderMaxY;
    private float _playerCameraMinX, _playerCameraMaxX, _playerCameraMinY, _playerCameraMaxY;
    private int _currentScene = -1;

    private IObjectPool<Enemy> _enemyPool;
    
    private List<Enemy> _allEnemies = new List<Enemy>();
    private bool _fadeOutText = false;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    private float _timeSinceLastSpawned;
    private float _timeBetweenSpawns;
    private void Update()
    {
        DoSceneChanges();
        
        if (_currentEnemiesAlive < _maxEnemyCountAtOnce &&
            _totalEnemiesSpawnedThisRound < _maxEnemyCountPerRound)
        {
            SpawnNextEnemy(); 
        }
        
        if (_fadeOutText)
            FadeOutRoundText();
    }

    private void DoSceneChanges()
    {
        if (SceneManager.GetActiveScene().buildIndex == _currentScene) return;
        int newCurrentScene = SceneManager.GetActiveScene().buildIndex;

        switch (newCurrentScene)
        {
            case 0:
                AudioManager.Instance.PlayMusic("Hip Hop Vol2 Convos Main");
                break;
            case 1:
                AudioManager.Instance.StopMusic();
                _enemyPool = new ObjectPool<Enemy>(CreateEnemy, OnGetEnemy, OnReleaseEnemy, OnDestroyEnemy, false, 20, 20);
                
                enemySpawnAreaCollider = GameObject.Find("Enemy Spawner").GetComponent<Collider2D>();
                waveCounterText = GameObject.Find("Wave Text").GetComponent<TextMeshProUGUI>();
                
                _enemySpawnAreaColliderMaxX = enemySpawnAreaCollider.bounds.max.x;
                _enemySpawnAreaColliderMaxY = enemySpawnAreaCollider.bounds.max.y;
                _enemySpawnAreaColliderMinX = enemySpawnAreaCollider.bounds.min.x;
                _enemySpawnAreaColliderMinY = enemySpawnAreaCollider.bounds.min.y;

                _currentEnemiesAlive = 0;

                InitializeGrid();
                PreSpawnEnemies();
                
                StartFadeOutRoundText();
                AudioManager.Instance.StartBattleMusic();
                break;
        }
            
        _currentScene = SceneManager.GetActiveScene().buildIndex;
    }

    private void SpawnNextEnemy()
    {
        if (_currentEnemiesAlive >= _maxEnemyCountAtOnce || _totalEnemiesSpawnedThisRound >= _maxEnemyCountPerRound) return;

        if (_potentialSpawnPoints.Count <= 0) return;
        Enemy enemy = _enemyPool.Get();
        if (enemy == null) return;
        enemy.GetComponent<SpriteRenderer>().enabled = true;
        Vector3 randomSpawnPosition = GetRandomSpawnPoint();
        enemy.transform.position = randomSpawnPosition;
        enemy.SetSpawnPosition(randomSpawnPosition);
        _currentEnemiesAlive++;
        _totalEnemiesSpawnedThisRound++;
    }
    
    private void PreSpawnEnemies()
    {
        for (int i = 0; i < 20; i++)
        {
            Enemy enemy = _enemyPool.Get();
            enemy.GetComponent<SpriteRenderer>().enabled = false;
            _allEnemies.Add(enemy);
        }
    }

    private void StartRound()
    {
        _fadeOutText = false;
        _roundNumber++;
        _totalEnemiesSpawnedThisRound = 0;
        _maxEnemyCountPerRound = Random.Range(3, 7) + _roundNumber * _roundNumber;

        // This is a desmos graph for the below equation
        // https://www.desmos.com/calculator/tthykrvgcq
        _maxEnemyCountAtOnce = Mathf.Clamp(Mathf.RoundToInt(_maxEnemyCountPerRound * 0.25f), 5, 20);
    }
    
    // Increased both to 100 because 10 was making the spawning break for some reason
    private int _gridRows = 100;
    private int _gridColumns = 100;

    private float _cellWidth;
    private float _cellHeight;
    private List<Vector3> _potentialSpawnPoints = new();

    private void InitializeGrid()
    {
        _cellWidth = (_enemySpawnAreaColliderMaxX - _enemySpawnAreaColliderMinX) / _gridColumns;
        _cellHeight = (_enemySpawnAreaColliderMaxY - _enemySpawnAreaColliderMinY) / _gridRows;

        for (int x = 0; x < _gridColumns; x++)
        {
            for (int y = 0; y < _gridRows; y++)
            {
                float spawnX = _enemySpawnAreaColliderMinX + x * _cellWidth + _cellWidth / 2;
                float spawnY = _enemySpawnAreaColliderMinY + y * _cellHeight + _cellHeight / 2;

                Vector3 spawnPos = new Vector3(spawnX, spawnY, 0);

                Vector3 spawnViewPortPoint = Camera.main.WorldToViewportPoint(spawnPos);
                _potentialSpawnPoints.Add(new Vector3(spawnX, spawnY));
            }
        }
    }
    private Enemy CreateEnemy()
    {
        int randomSpawnPointListPosition = Random.Range(0, _potentialSpawnPoints.Count);
        Vector3 spawnPoint = _potentialSpawnPoints[randomSpawnPointListPosition];
        _potentialSpawnPoints.RemoveAt(randomSpawnPointListPosition);

        GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint, Quaternion.identity);
        GameObject sprite = newEnemy.GetComponentInChildren<SpriteRenderer>().gameObject;
        Enemy enemyScript = sprite.AddComponent<Enemy>();
        enemyScript.SetPool(_enemyPool);

        return enemyScript;
    }

    private void OnGetEnemy(Enemy enemy)
    {
        enemy.GetComponent<SpriteRenderer>().enabled = true;
        enemy.transform.position = GetRandomSpawnPoint();
    }

    private void OnReleaseEnemy(Enemy enemy)
    {
        enemy.ResetEnemy();
        enemy.GetComponent<SpriteRenderer>().enabled = false;
        _currentEnemiesAlive = Mathf.Max(0, _currentEnemiesAlive - 1);

        if (_currentEnemiesAlive == 0 && _totalEnemiesSpawnedThisRound >= _maxEnemyCountPerRound)
        {
            StartFadeOutRoundText();
        }
    }

    private void OnDestroyEnemy(Enemy enemy)
    {
        Destroy(enemy.gameObject);
    }

    private Vector3 GetRandomSpawnPoint()
    {
        if (_potentialSpawnPoints.Count == 0)
        {
            InitializeGrid();
        }

        Vector3 spawnPoint = Vector3.zero;
        int maxAttempts = _potentialSpawnPoints.Count;
        int attempt = 0;

        do
        {
            int randomIndex = Random.Range(0, _potentialSpawnPoints.Count);
            spawnPoint = _potentialSpawnPoints[randomIndex];

            Vector3 viewPortPoint = Camera.main.WorldToViewportPoint(spawnPoint);
            bool isOutsideView = viewPortPoint.x < 0f || viewPortPoint.x > 1f || viewPortPoint.y < 0f || viewPortPoint.y > 1f;

            if (isOutsideView)
            {
                _potentialSpawnPoints.RemoveAt(randomIndex);
                return spawnPoint;
            }

            attempt++;
        } while (attempt < maxAttempts);

        // Not adding spawn points back
        Debug.LogWarning("No valid spawn point found outside the camera view. Using fallback.");
        return _potentialSpawnPoints[Random.Range(0, _potentialSpawnPoints.Count)];
    }

    public void AddPotentialSpawnPoint(Vector3 point)
    {
        _potentialSpawnPoints.Add(point);
    }

    public void DecreaseCurrentEnemiesAlive()
    {
        if (_currentEnemiesAlive < 0)
        {
            Debug.LogWarning("Enemies alive count went below zero. Check enemy handling logic.");
            _currentEnemiesAlive = 0;
        }

        if (_currentEnemiesAlive < _maxEnemyCountAtOnce && _totalEnemiesSpawnedThisRound < _maxEnemyCountPerRound)
        {
            _enemyPool.Get();
        }
    }
    
    public int GetCurrentEnemiesAlive()
    {
        return _currentEnemiesAlive;
    }

    public int GetMaxEnemyCountAtOnce()
    {
        return _maxEnemyCountAtOnce;
    }

    public int GetRoundNumber()
    {
        return _roundNumber;
    }

    public int GetTotalEnemiesSpawnedThisRound()
    {
        return _totalEnemiesSpawnedThisRound;
    }

    public int GetMaxEnemyCountPerRound()
    {
        return _maxEnemyCountPerRound;
    }

    private float _transitionDuration = 3f;
    
    private float _fadeStartTime;

    private void FadeOutRoundText()
    {
        if (!_fadeOutText) return;

        float elapsedTime = Time.time - _fadeStartTime;
        waveCounterText.alpha = Mathf.Lerp(1f, 0f, elapsedTime / _transitionDuration);

        if (!(elapsedTime >= _transitionDuration)) return;
        waveCounterText.alpha = 0f;
        _fadeOutText = false;

        if (_currentEnemiesAlive == 0)
        {
            StartRound();
        }
    }

    private void StartFadeOutRoundText()
    {
        waveCounterText.text = "Wave " + (_roundNumber + 1);
        waveCounterText.alpha = 1f;
        _fadeOutText = true;
        _fadeStartTime = Time.time;
    }
}