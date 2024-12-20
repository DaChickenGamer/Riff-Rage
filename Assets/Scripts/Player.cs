using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private Vector2 pointerInput;
    
    [SerializeField]
    private InputActionReference pointerPosition, attack;

    private int _health;

    private PlayerWeapon _playerWeapon;
    private Animator _animator;
    
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
    }
    private void Update()
    {
        pointerInput = GetPointerInput();
        _playerWeapon.PointerPosition = pointerInput;
    }

    public int GetPlayerHealth()
    {
        return _health;
    }

    public void TakeDamage(int damage)
    {
        _health -= damage;
        AudioManager.Instance.PlaySFX("Pain");
        
        if (_health > 0) return;
        Death();
    }

    public void Death()
    {
        // Trigger Death Animation
        // Hide
        // Pull up death ui
    }

    private Vector2 GetPointerInput()
    {
        Vector3 mousePos = pointerPosition.action.ReadValue<Vector2>();
        mousePos.z = Camera.main.nearClipPlane;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }
}
