using System.Collections.Generic;
using UnityEngine;

public class Shatter : MonoBehaviour
{
    public GameObject[] objectsToShatterInto;
    private List<ShatterPiece> _shatteredObjects = new();

    [SerializeField] private int minimumShatterDirectionX = -5;
    [SerializeField] private int minimumShatterDirectionY = -5;
    [SerializeField] private int maximumShatterDirectionX = 5;
    [SerializeField] private int maximumShatterDirectionY = 5;
    [SerializeField] private int minimumTorque = 10;
    [SerializeField] private int maximumTorque = 20;
    [SerializeField] private float fadeDuration = 2.0f;

    public void ShatterObject(GameObject objectToShatter)
    {
        if (objectsToShatterInto == null || objectsToShatterInto.Length == 0)
        {
            Debug.LogError("objectsToShatterInto is not populated.");
            return;
        }

        _shatteredObjects.Clear();

        foreach (GameObject obj in objectsToShatterInto)
        {
            GameObject newShatterObject = Instantiate(obj, objectToShatter.transform.position, Quaternion.identity);

            Rigidbody2D rb = newShatterObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;

            float directionX = Random.Range(minimumShatterDirectionX, maximumShatterDirectionX);
            float directionY = Random.Range(minimumShatterDirectionY, maximumShatterDirectionY);
            float torque = Random.Range(minimumTorque, maximumTorque);

            rb.AddForce(new Vector2(directionX, directionY), ForceMode2D.Impulse);
            rb.AddTorque(torque, ForceMode2D.Force);

            SpriteRenderer spriteRenderer = newShatterObject.GetComponent<SpriteRenderer>();
            ShatterPiece shatteredObject = newShatterObject.AddComponent<ShatterPiece>();
            
            shatteredObject.Initialize(spriteRenderer, fadeDuration);
            
            _shatteredObjects.Add(shatteredObject);
        }
    }

    private void Update()
    {
        for (int i = _shatteredObjects.Count - 1; i >= 0; i--)
        {
            _shatteredObjects[i].UpdateFade();
            if (!_shatteredObjects[i].IsFading)
            {
                _shatteredObjects.RemoveAt(i);
            }
        }
    }
}
