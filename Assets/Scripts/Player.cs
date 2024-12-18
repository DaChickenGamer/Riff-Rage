using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private Vector2 pointerInput;
    
    [SerializeField]
    private InputActionReference pointerPosition, attack;

    private ParentWeapon _parentWeapon;

    // add health script here

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

    private Vector2 GetPointerInput()
    {
        Vector3 mousePos = pointerPosition.action.ReadValue<Vector2>();
        mousePos.z = Camera.main.nearClipPlane;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }
}
