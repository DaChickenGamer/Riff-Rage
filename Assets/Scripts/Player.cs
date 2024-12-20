using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private Vector2 pointerInput;
    
    [SerializeField]
    private InputActionReference pointerPosition, attack;

    private int _health;

    private ParentWeapon _parentWeapon;
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
        _parentWeapon.Attack();
    }

    private void Awake()
    {
        _parentWeapon = GetComponentInChildren<ParentWeapon>();
    }
    private void Update()
    {
        pointerInput = GetPointerInput();
        _parentWeapon.pointerPosition = pointerInput;
    }

    public int GetPlayerHealth()
    {
        return _health;
    }

    public void TakeDamage(int damage)
    {
        _health -= damage;
    }

    private Vector2 GetPointerInput()
    {
        Vector3 mousePos = pointerPosition.action.ReadValue<Vector2>();
        mousePos.z = Camera.main.nearClipPlane;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }
}
