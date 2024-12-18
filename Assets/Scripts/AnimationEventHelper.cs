using UnityEngine;
using UnityEngine.Events;

public class Animatior : MonoBehaviour
{
    public UnityEvent OnAnimationEventTriggered;

    public void TriggerEvent()
    {
        OnAnimationEventTriggered?.Invoke();
    }
}
