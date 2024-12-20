using System;
using System.Collections;
using UnityEngine;

public class ParentWeapon : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public SpriteRenderer characterRenderer, weaponRenderer;
    public Vector2 pointerPosition { get; set; }
    
    public Animator animator;
    public float delay = 0.3f;
    private bool attackBlocked;

    public bool IsAttacking { get; private set; }

    //combat
    public Transform circleOrigin;
    public float radius;
    public void ResetIsAttacking()
    {
        IsAttacking = false;
    }

    private void Update()
    {
        if (IsAttacking)
            return; 
        Vector2 direction = (pointerPosition -(Vector2)transform.position).normalized;
        transform.right = direction;
        
        Vector2 scale = transform.localScale;

        scale.y = direction.x switch
        {
            < 0 => -1,
            > 0 => 1,
            _ => scale.y
        };
        transform.localScale = scale;

        if (transform.eulerAngles.z > 0 && transform.eulerAngles.z < 180)
        {
            weaponRenderer.sortingOrder = characterRenderer.sortingOrder - 1;
        }
        else
        {
            weaponRenderer.sortingOrder = characterRenderer.sortingOrder + 1;
        }
    }

    public void Attack()
    {
        if (attackBlocked)
            return;
        animator.SetTrigger("Attack");
        IsAttacking = true;
        attackBlocked = true;
        StartCoroutine(DelayAttack());
    }

    private IEnumerator DelayAttack()
    {
        yield return new WaitForSeconds(delay);
        attackBlocked = false;
    }
    
    //combat
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Vector3 position = circleOrigin == null ? Vector3.zero : circleOrigin.position;
    }

    public void DetectColliders()
    {
        foreach (Collider2D collider in Physics2D.OverlapCircleAll(circleOrigin.position, radius))
        {
            if (collider.transform.parent == null) return;
            
            if (!collider.transform.parent.CompareTag("Enemy")) return;
            collider.gameObject.GetComponent<Enemy>().TakeDamage(50);
        }
    }
}
