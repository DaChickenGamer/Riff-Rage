using UnityEngine;

public class Enemy : MonoBehaviour
{
    private GameObject _player;
    
    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        if (_player != null)
        {
            
        }
    }
}
