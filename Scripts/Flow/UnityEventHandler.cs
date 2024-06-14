using UnityEngine;
using UnityEngine.Events;

public class UnityEventHandler : MonoBehaviour
{
    public UnityEvent m_UnityEvent;

    public void InvokeUnityEvent()
    {
        m_UnityEvent?.Invoke();
    }
}
