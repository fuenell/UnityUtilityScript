using UnityEngine;

public class SoundPlayHandler : MonoBehaviour
{
    [SerializeField]
    private string _clipName;

    [SerializeField]
    private bool _playOnEnable = true;

    private void OnEnable()
    {
        if (_playOnEnable)
        {
            PlaySound();
        }
    }

    public void PlaySound()
    {
        SoundManager.Instance.PlaySound(_clipName);
    }
}
