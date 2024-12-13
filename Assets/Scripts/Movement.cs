using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private int speed = 5;

    private Vector2 direction;
    private Rigidbody2D rb; 
    
    // No Animations Currently
    //private Animator animator;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); 
        //animator = GetComponent<Animator>();
    }
    public void OnMovement(InputAction.CallbackContext ctxt)
    {
        direction = ctxt.ReadValue<Vector2>();

        /*if (direction.x != 0 || direction.y != 0)
        {
            animator.SetFloat("X", direction.x);
            animator.SetFloat("Y", direction.y);
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }*/
    }
    private void Update()
    {
        rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
    }

}
