using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private Vector2 pointerInput;
    
    [SerializeField]
    private InputActionReference pointerPosition, attack;

    private int _health;

    private PlayerWeapon _playerWeapon;
    public Animator animator;
    private bool _isDead;
    private bool _isPlayingDeathSound;

    public GameObject deathUI;
    
    public Slider healthSlider;
    
    private void OnEnable()
    {
        attack.action.performed += PerformAttack;
    }

    private void OnDisable()
    {
        attack.action.performed -= PerformAttack;
    }

    private void PerformAttack(InputAction.CallbackContext obj)
    {
        _playerWeapon.Attack();
    }

    private void Awake()
    {
        _playerWeapon = GetComponentInChildren<PlayerWeapon>();
        deathUI.SetActive(false);
        _health = 100;
    }
    private void Update()
    {
        pointerInput = GetPointerInput();
        _playerWeapon.PointerPosition = pointerInput;
        healthSlider.value = _health;
    }

    public void OnMenuOpen(InputAction.CallbackContext ctxt)
    {
        if (!ctxt.started && IsDead()) return;
        if (UIManager.Instance.GetIngameMenuActive())
        {
            UIManager.Instance.CloseIngameMenu();
        }
        else if (!UIManager.Instance.GetIngameMenuActive())
        {
            UIManager.Instance.OpenIngameMenu();
        }
    }

    public int GetPlayerHealth()
    {
        return _health;
    }

    public void ResetPlayer()
    {
        _health = 100;
        _isDead = false;
        animator.ResetTrigger("isDead");
        animator.Play("BikerIdle");
    }

    public void TakeDamage(int damage)
    {
        _health -= damage;
        PlayHitSound();
        
        if (_health > 0) return;
        Death();
    }

    public void Death()
    {
        _isDead = true;
        animator.SetTrigger("isDead");
        StartCoroutine(WaitForDeathAnimation());
        foreach (Transform child in gameObject.transform)
        {
            if (child.name == "Biker Sprite")
                child.GetComponent<SpriteRenderer>().enabled = true;
        }        
        deathUI.SetActive(true);
        Time.timeScale = 0;
    }

    public void DisableDeathUI()
    {
        deathUI.SetActive(false);
    }
    
    public bool IsDead()
    {
        return _isDead;
    }

    public void PlayHitSound()
    {
        if (!_isPlayingDeathSound)
        {
            _isPlayingDeathSound = true;
            AudioManager.Instance.PlaySFX("Pain");
            StartCoroutine(WaitForHitSound());
        }
            
    }

    private Vector2 GetPointerInput()
    {
        Vector3 mousePos = pointerPosition.action.ReadValue<Vector2>();
        mousePos.z = Camera.main.nearClipPlane;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    private IEnumerator WaitForHitSound()
    {
        yield return new WaitForSeconds(AudioManager.Instance.GetSfxLength("Pain"));
        _isPlayingDeathSound = false;
    }
    
    private IEnumerator WaitForDeathAnimation()
    {
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

    }
}
