using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float cameraSpeed = 2f;
    public float resetThreshold = 1000f;
    public float smoothResetSpeed = 5f;

    private Vector3 _targetPosition;
    private bool _isResetting;

    void Start()
    {
        _targetPosition = transform.position;
    }

    void Update()
    {
        transform.position = new Vector3(transform.position.x + cameraSpeed * Time.deltaTime, transform.position.y, transform.position.z);

        if (transform.position.x > resetThreshold && !_isResetting)
        {
            _isResetting = true;
            _targetPosition = new Vector3(0, transform.position.y, transform.position.z);
        }

        if (!_isResetting) return;

        transform.position = Vector3.Lerp(transform.position, _targetPosition, smoothResetSpeed * Time.deltaTime);

        if (Mathf.Abs(transform.position.x - _targetPosition.x) < 1f)
        {
            transform.position = _targetPosition;
            _isResetting = false;
        }
    }
}
