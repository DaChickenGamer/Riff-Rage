using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Parallax : MonoBehaviour
{
    private float _length, _startpos;
    public GameObject cam;
    public float parallaxEffect;

    void Start()
    {
        _startpos = transform.position.x;
        _length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void Update()
    {
        float distanceMoved = cam.transform.position.x * parallaxEffect;
        transform.position = new Vector3(_startpos + distanceMoved, transform.position.y, transform.position.z);

        if (cam.transform.position.x * (1 - parallaxEffect) > _startpos + _length)
        {
            _startpos += _length;
        }
        else if (cam.transform.position.x * (1 - parallaxEffect) < _startpos - _length)
        {
            _startpos -= _length;
        }
    }
}
