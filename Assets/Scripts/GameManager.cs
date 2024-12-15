using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Pool;
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

    private IObjectPool<Enemy> _enemyPool;
    
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

        _enemyPool = new ObjectPool<Enemy>(CreateEnemy, OnGetEnemy, OnReleaseEnemy, OnDestroyEnemy, false, 20, 20);
    }
    
    private void Start()
    {
        _enemySpawnAreaColliderMaxX = enemySpawnAreaCollider.bounds.max.x;
        _enemySpawnAreaColliderMaxY = enemySpawnAreaCollider.bounds.max.y;
        _enemySpawnAreaColliderMinX = enemySpawnAreaCollider.bounds.min.x;
        _enemySpawnAreaColliderMinY = enemySpawnAreaCollider.bounds.min.y;
        
        _currentEnemiesAlive = 0;
        
        InitializeGrid();
        StartRound();
        AudioManager.Instance.StartBattleMusic();
    }

    private float timeSinceLastSpawned;
    private float timeBetweenSpawns;
    private void Update()
    {
        if (_currentEnemiesAlive < _maxEnemyCountAtOnce &&
            _totalEnemiesSpawnedThisRound < _maxEnemyCountPerRound &&
            Time.time > timeSinceLastSpawned)
        {
            timeBetweenSpawns = Random.Range(0.8f, 1.2f);
            _enemyPool.Get();
            timeSinceLastSpawned = Time.time + timeBetweenSpawns;
        }
    }

    private void StartRound()
    {
        _roundNumber++;
        _totalEnemiesSpawnedThisRound = 0;
        _maxEnemyCountPerRound = 5 + _roundNumber * _roundNumber;
        _maxEnemyCountAtOnce = Mathf.Clamp(Mathf.RoundToInt(_maxEnemyCountPerRound * 0.25f), 5, 20);

        waveCounterText.alpha = 1;
        waveCounterText.text = "Wave " + _roundNumber;

        StartCoroutine(FadeOutStartRoundText());
    }
    
    private int _gridRows = 10;
    private int _gridColumns = 10;

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
                if (spawnViewPortPoint.x < 0f || 
                    spawnViewPortPoint.x > 1f || 
                    spawnViewPortPoint.y < 0f || 
                    spawnViewPortPoint.y > 1f)
                    _potentialSpawnPoints.Add(new Vector3(spawnX, spawnY));
            }
        }
    }

    private bool _isSpawning;

    private Enemy CreateEnemy()
    {
        int randomSpawnPointListPosition = Random.Range(0, _potentialSpawnPoints.Count);
        Vector3 spawnPoint = _potentialSpawnPoints[randomSpawnPointListPosition];
        _potentialSpawnPoints.RemoveAt(randomSpawnPointListPosition);

        GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint, Quaternion.identity);
        GameObject sprite = newEnemy.GetComponentInChildren<SpriteRenderer>().gameObject;
        Enemy enemyScript = sprite.AddComponent<Enemy>();
        enemyScript.gameManagerObject = gameObject;
        enemyScript.SetPool(_enemyPool);

        return enemyScript;
    }

    private void OnGetEnemy(Enemy enemy)
    {
        enemy.gameObject.SetActive(true);
        enemy.transform.position = GetRandomSpawnPoint();
        _currentEnemiesAlive++;
        _totalEnemiesSpawnedThisRound++;
    }

    private void OnReleaseEnemy(Enemy enemy)
    {
        enemy.gameObject.SetActive(false);
        _currentEnemiesAlive--;

        if (_currentEnemiesAlive <= 0)
        {
            StartCoroutine(WaitForNextRound());
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

        int randomIndex = Random.Range(0, _potentialSpawnPoints.Count);
        Vector3 spawnPoint = _potentialSpawnPoints[randomIndex];
        _potentialSpawnPoints.RemoveAt(randomIndex);
        return spawnPoint;
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
    
    IEnumerator FadeOutStartRoundText()
    {
        while (waveCounterText.alpha > 0f)
        {
            waveCounterText.alpha = Mathf.Lerp(waveCounterText.alpha, 0f, Time.deltaTime * .25f);
            yield return null;
        }
    }
    IEnumerator WaitForNextRound()
    {
        yield return new WaitForSeconds(5f);
        StartRound();
    }
     
}
