using UnityEngine;
using UnityEngine.Pool;

public class Enemy : MonoBehaviour
{
    private GameObject _player;
    private const float DefaultSpeed = 4.0f;

    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private Vector3 _spawnPosition;

    private const int MaxHealth = 100;
    private int _health = MaxHealth;

    private IObjectPool<Enemy> _enemyPool;
    private bool _isAlive = true;
    private bool _damagePlayerDelay = false;
    private Shatter _shatter;

    private float _hitDelayTimer = 0f;
    private float _damageAnimationTimer = 0f;
    private bool _isDamageAnimating = false;

    private const float HitDelayDuration = 0.5f;
    private const float DamageAnimationDuration = 0.1f;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _player = GameObject.FindGameObjectWithTag("Player");
        _shatter = GetComponent<Shatter>();
    }

    private void Update()
    {
        EnemyMove();

        if (_damagePlayerDelay)
        {
            _hitDelayTimer += Time.deltaTime;
            if (_hitDelayTimer >= HitDelayDuration)
            {
                _damagePlayerDelay = false;
                _hitDelayTimer = 0f;
            }
        }

        if (_isDamageAnimating)
        {
            _damageAnimationTimer += Time.deltaTime;
            if (_damageAnimationTimer >= DamageAnimationDuration)
            {
                SetSpriteColor(new Color(1f, 1f, 1f, 1f));
                _isDamageAnimating = false;
                _damageAnimationTimer = 0f;
            }
        }
    }

    private void EnemyMove()
    {
        if (_player == null || !_isAlive || !gameObject.GetComponent<SpriteRenderer>().enabled) return;

        float step = DefaultSpeed * Time.deltaTime;
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
        StartDamageAnimation();

        if (_health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (_enemyPool == null)
        {
            Destroy(gameObject);
            return;
        }

        GameManager.Instance.AddPotentialSpawnPoint(_spawnPosition);
        GameManager.Instance.DecreaseCurrentEnemiesAlive();
        _isAlive = false;

        _shatter.ShatterObject(gameObject);

        _spriteRenderer.enabled = false;
        _enemyPool.Release(this);
    }

    public void ResetEnemy()
    {
        _health = MaxHealth;
        _isAlive = true;
        _damagePlayerDelay = false;
        _hitDelayTimer = 0f;
        _damageAnimationTimer = 0f;
        _isDamageAnimating = false;

        _spriteRenderer.enabled = true;
        SetSpriteColor(Color.white);
        transform.position = _spawnPosition;
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag("Player") || _damagePlayerDelay) return;

        _damagePlayerDelay = true;
        other.gameObject.GetComponent<Player>().TakeDamage(10);
    }

    public void SetSpawnPosition(Vector3 spawnPosition)
    {
        _spawnPosition = spawnPosition;
    }

    private void StartDamageAnimation()
    {
        _isDamageAnimating = true;
        SetSpriteColor(new Color(1f, 0.2f, 0f));
    }

    private void SetSpriteColor(Color color)
    {
        _spriteRenderer.color = color;
    }
}
