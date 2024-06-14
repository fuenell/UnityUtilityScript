using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DoubleTouchDelayEvent : MonoBehaviour
{
    public float m_Delay = 1f;

    public UnityEvent m_StartDelay;
    public UnityEvent m_FinishDelay;

    public void OnEnable()
    {
        m_StartDelay?.Invoke();
        StartCoroutine(ButtonDelay());
    }

    private IEnumerator ButtonDelay()
    {
        yield return new WaitForSeconds(m_Delay);
        m_FinishDelay?.Invoke();
    }
}
