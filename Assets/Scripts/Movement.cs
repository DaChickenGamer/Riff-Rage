using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private int speed = 5;

    private Vector2 direction;
    private Rigidbody2D rb;

    public GameObject weaponSprite;
    public Animator animator;
    private Player _player;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); 
        _player = GetComponent<Player>();
    }
    public void OnMovement(InputAction.CallbackContext ctxt)
    {
        direction = ctxt.ReadValue<Vector2>();

        if (_player.IsDead()) return;

        if (direction.x != 0 || direction.y != 0)
        {
            animator.transform.localScale = direction.x switch
            {
                > 0 => new Vector3(1, 1, 1),
                < 0 => new Vector3(-1, 1, 1),
                _ => animator.transform.localScale
            };

            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }
    private void Update()
    {
        rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
    }

}
