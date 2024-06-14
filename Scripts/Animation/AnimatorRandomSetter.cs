using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorRandomSetter : MonoBehaviour
{
    public Animator m_Animator;
    public Vector2 m_AnimationSpeedRange = new Vector2(0.9f, 1.1f);

    private void OnEnable()
    {
        m_Animator.speed = Random.Range(m_AnimationSpeedRange.x, m_AnimationSpeedRange.y);
        int currentNameHash = m_Animator.GetCurrentAnimatorStateInfo(0).shortNameHash;
        m_Animator.Play(currentNameHash, 0, Random.Range(0f, 1f));
    }
}
