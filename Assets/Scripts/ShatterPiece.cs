using UnityEngine;

public class ShatterPiece : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private float _fadeElapsedTime = 0f;
    private float _fadeDuration;

    public bool IsFading => _fadeElapsedTime < _fadeDuration;
        
    public void Initialize(SpriteRenderer spriteRenderer, float fadeDuration)
    {
        _spriteRenderer = spriteRenderer;
        _fadeDuration = fadeDuration;
    }

    public void UpdateFade()
    {
        _fadeElapsedTime += Time.deltaTime;
        float alpha = Mathf.Lerp(1f, 0f, Mathf.Clamp01(_fadeElapsedTime / _fadeDuration));

        Color currentColor = _spriteRenderer.color;
        _spriteRenderer.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
    }
}
