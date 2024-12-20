using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class Enemy : MonoBehaviour
{
    private GameObject _player;
    private float _speed = 6.0f;

    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private Vector3 _spawnPosition;
    
    private int _health = 100;

    private IObjectPool<Enemy> _enemyPool;
    private bool _isAlive = true;
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        EnemyMove();
    }

    private void EnemyMove()
    {
        if (_player == null || !_isAlive) return;
        
        float step = _speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, step);

        float direction = _player.transform.position.x - transform.position.x;

        if (Mathf.Abs(direction) > 0f)
        {
            _animator.SetBool("isWalking", true);

            if (!Mathf.Approximately(Mathf.Sign(direction), Mathf.Sign(transform.localScale.x)))
            {
                transform.localScale = new Vector3(
                    Mathf.Sign(direction) * Mathf.Abs(transform.localScale.x),
                    transform.localScale.y,
                    transform.localScale.z
                );
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
    
    public void TakeDamage(int damageReceived)
    {
        
        _health -= damageReceived;

        StartCoroutine(TakeDamageAnimation());

        if (_health > 0) return;
        
        Die();
    }

    private void Die()
    {
        GameManager.Instance.AddPotentialSpawnPoint(_spawnPosition);
        GameManager.Instance.DecreaseCurrentEnemiesAlive();
        _isAlive = false;
        _animator.SetTrigger("isDead");
        
        StartCoroutine(WaitForDeathAnimation());
    }

    public void ResetEnemy()
    {
        _health = 100;
        _isAlive = true;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<Player>().TakeDamage(10);
        }
    }

    public void SetSpawnPosition(Vector3 spawnPosition)
    {
        _spawnPosition = spawnPosition;
    }

    private IEnumerator TakeDamageAnimation()
    {
        _spriteRenderer.color = new Color(1f, 0.2f, 0f);
        yield return new WaitForSeconds(0.1f);
        _spriteRenderer.color = new Color(1, 1, 1, 1f);
    }
    private IEnumerator WaitForDeathAnimation()
    {
        yield return new WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length);

        _enemyPool.Release(this);
    }
}
