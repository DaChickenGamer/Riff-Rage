using UnityEngine;

public class ParentWeapon : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Vector2 pointerPosition { get; set; }

    private void Update()
    {
        transform.right = (pointerPosition-(Vector2)transform.position).normalized;
    }
}
