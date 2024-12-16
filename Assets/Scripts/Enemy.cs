using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class Enemy : MonoBehaviour
{
    private GameObject _player;
    private float _speed = 10.0f;

    private Animator _animator;
    private Vector3 _spawnPosition;
    
    private int _health = 100;

    private IObjectPool<Enemy> _enemyPool;
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        if (_player == null) return;

        float step = _speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, step);

        float direction = _player.transform.position.x - transform.position.x;

        if (Mathf.Abs(direction) > 0f)
        {
            _animator.SetBool("isWalking", true);

            if (Mathf.Sign(direction) != Mathf.Sign(transform.localScale.x))
            {
                transform.localScale = new Vector3(Mathf.Sign(direction) * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
        else
        {
            _animator.SetBool("isWalking", false);
        }
    }

    public void SetPool(IObjectPool<Enemy> pool)
    {
        _enemyPool = pool;
    }
    
    public void GetDamaged(int damageReceived)
    {
        _health -= damageReceived;

        if (_health > 0) return;
        
        Die();
    }

    private void Die()
    {
        GameManager.Instance.AddPotentialSpawnPoint(_spawnPosition);
        GameManager.Instance.DecreaseCurrentEnemiesAlive();
        _enemyPool.Release(this);
    }

    public void ResetEnemy()
    {
        _health = 100;
        transform.position = _spawnPosition;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Decrease player health here
            
            // For Testing On Death Features
            GetDamaged(100);
        }
    }

    public void SetSpawnPosition(Vector3 spawnPosition)
    {
        _spawnPosition = spawnPosition;
    }
}
