using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TextTyper : MonoBehaviour
{
    [Header("텍스트 컴포넌트")]
    public Text m_TextUI;
    public TextMesh m_TextMesh;
    public TextMeshProUGUI m_TextMeshProUGUI;
    public TextMeshPro m_TextMeshPro;

    [Header("타이핑 설정")]
    [TextArea]
    public string m_Text;

    public float m_Delay = 0.1f;
    public UnityEvent m_FinishEvent;

    private StringBuilder m_TextBuffer = new StringBuilder();
    private bool skipTyping = false;
    private int currentIndex = 0;

    private void OnEnable()
    {
        skipTyping = false;
        currentIndex = 0;
        SetText("");
        m_TextBuffer.Clear();
        StartCoroutine(TypingText());
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // 스페이스 키로 타이핑 스킵
        {
            skipTyping = true;
        }
    }
#endif

    private IEnumerator TypingText()
    {
        float elapsedTime = 0f;

        while (currentIndex < m_Text.Length)
        {
            if (skipTyping)
            {
                SetText(m_Text);
                break;
            }

            elapsedTime += Time.deltaTime;
            while (elapsedTime >= m_Delay && currentIndex < m_Text.Length)
            {
                m_TextBuffer.Append(m_Text[currentIndex]);
                currentIndex++;
                elapsedTime -= m_Delay;
                SetText(m_TextBuffer.ToString());
            }

            yield return null;
        }

        SetText(m_Text);

        yield return new WaitForSeconds(1);
        m_FinishEvent?.Invoke();
    }

    private void SetText(string text)
    {
        if (m_TextUI != null)
        {
            m_TextUI.text = text;
        }
        if (m_TextMesh != null)
        {
            m_TextMesh.text = text;
        }
        if (m_TextMeshProUGUI != null)
        {
            m_TextMeshProUGUI.text = text;
        }
        if (m_TextMeshPro != null)
        {
            m_TextMeshPro.text = text;
        }
    }
}