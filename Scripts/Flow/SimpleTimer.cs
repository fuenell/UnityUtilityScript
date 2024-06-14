using System;
using System.Collections;
using UnityEngine;

public class SimpleTimer : MonoBehaviour
{
    private Coroutine m_TimerCoroutine;

    public void StartTimer(float timerTime, Action callback)
    {
        StopTimer();
        m_TimerCoroutine = StartCoroutine(TimerCoroutine(timerTime, callback));
    }

    public void StopTimer()
    {
        if (m_TimerCoroutine != null)
        {
            StopCoroutine(m_TimerCoroutine);
        }
    }

    private IEnumerator TimerCoroutine(float timerTime, Action callback)
    {
        yield return new WaitForSeconds(timerTime);
        callback?.Invoke();
    }
}