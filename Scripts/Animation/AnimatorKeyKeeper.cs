using UnityEngine;

public class AnimatorKeyKeeper : MonoBehaviour
{
    private Animator m_Animator;
    private int m_DefaultStateNameHash;

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();

        m_Animator.keepAnimatorStateOnDisable = true;
        m_DefaultStateNameHash = m_Animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
    }

    void OnEnable()
    {
        m_Animator.Play(m_DefaultStateNameHash, 0, 0);
    }
}
