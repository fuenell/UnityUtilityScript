using UnityEngine;
using UnityEngine.Events;

public class PauseManager : MonoBehaviour
{
    private bool m_IsPlaying = true;

    public GameObject m_TouchBlockPanel;
    public UnityEvent m_PlayEvent;

    public void OnFlipPause()
    {
        if (m_IsPlaying)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    private void ResumeGame()
    {
        m_IsPlaying = true;

        Time.timeScale = 1f;
        AudioListener.pause = false;

        m_PlayEvent?.Invoke();

        if (m_TouchBlockPanel != null)
        {
            m_TouchBlockPanel.SetActive(false);
        }
    }

    private void PauseGame()
    {
        m_IsPlaying = false;

        Time.timeScale = 0f;
        AudioListener.pause = true;

        if (m_TouchBlockPanel != null)
        {
            m_TouchBlockPanel.SetActive(true);
        }
    }
}