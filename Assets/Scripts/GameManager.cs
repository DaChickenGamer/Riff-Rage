using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.UI;
using UnityEngine;
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

    private bool _isNewRoundStarting = false;
    
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
    
    private void Start()
    {
        _enemySpawnAreaColliderMaxX = enemySpawnAreaCollider.bounds.max.x;
        _enemySpawnAreaColliderMaxY = enemySpawnAreaCollider.bounds.max.y;
        _enemySpawnAreaColliderMinX = enemySpawnAreaCollider.bounds.min.x;
        _enemySpawnAreaColliderMinY = enemySpawnAreaCollider.bounds.min.y;
        
        _currentEnemiesAlive = 0;
        
        InitializeGrid();
        StartRound();
    }

    private void StartRound()
    {
        _roundNumber++;
        _totalEnemiesSpawnedThisRound = 0;
        
        _maxEnemyCountPerRound = 5 + _roundNumber * _roundNumber;        
        
        // Quadratic Scaling
        // _maxEnemyCount = Mathf.CeilToInt(1 + _roundNumber * 0.5f); 
        
        // Log Growth
        // _maxEnemyCount = Mathf.RoundToInt(_enemyCount * Mathf.Log(_roundNumber + 1, 2));
        
        // Controlled Growth with a min of 5 and cap of 20
        _maxEnemyCountAtOnce = Mathf.Clamp(Mathf.RoundToInt(_maxEnemyCountPerRound * 0.25f), 5, 20);
        
        waveCounterText.alpha = 1;
        waveCounterText.text = "Wave " + _roundNumber.ToString();

        StartCoroutine(FadeOutStartRoundText());
        SpawnEnemies();
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
    public void SpawnEnemies()
    {
        if (_isSpawning) return;
        
        _isSpawning = true;
        
        while (_currentEnemiesAlive < _maxEnemyCountAtOnce)
        {
            
            int randomSpawnPointListPosition = Random.Range(0, _potentialSpawnPoints.Count);
            
            Vector3 spawnPoint = _potentialSpawnPoints[randomSpawnPointListPosition]; ;
            GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint, Quaternion.identity);
            
            GameObject sprite = newEnemy.GetComponentInChildren<SpriteRenderer>().gameObject;
            Enemy enemyScript = sprite.AddComponent<Enemy>();
            enemyScript.gameManagerObject = gameObject;
            
            enemyScript.SetSpawnPosition(spawnPoint);
            
            _potentialSpawnPoints.RemoveAt(randomSpawnPointListPosition);
            
            _totalEnemiesSpawnedThisRound += 1;
            _currentEnemiesAlive += 1;
        }
        
        if (_potentialSpawnPoints.Count == 0)
        {
            InitializeGrid();
        }
        
        _isSpawning = false;
    }

    public void AddPotentialSpawnPoint(Vector3 point)
    {
        _potentialSpawnPoints.Add(point);
    }

    public void DecreaseCurrentEnemiesAlive()
    {
        _currentEnemiesAlive -= 1;
        
        if (_currentEnemiesAlive < _maxEnemyCountAtOnce && _totalEnemiesSpawnedThisRound < _maxEnemyCountPerRound)
        {
            SpawnEnemies();
        }

        if (_currentEnemiesAlive != 0) return;
        
        StartCoroutine(WaitForNextRound());
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
