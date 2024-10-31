using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager _instance;

    public static SoundManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SoundManager>();

                if (_instance == null)
                {
                    GameObject soundManager = new GameObject(typeof(SoundManager).Name);
                    _instance = soundManager.AddComponent<SoundManager>();
                }
            }
            return _instance;
        }
    }

    [SerializeField]
    private SoundList _soundList;

    [SerializeField]
    private int _initSoundChannelCount = 10;

    private AudioSource _musicChannel;

    private List<AudioSource> _soundChannels;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(_instance.gameObject);
        }
        else if (_instance != this)
        {
            Destroy(this);
            return;
        }

        if (_soundList == null)
        {
            _soundList = SoundList.GetOrCreateSoundList();
        }

        _musicChannel = gameObject.AddComponent<AudioSource>();
        _musicChannel.loop = true;
        _musicChannel.playOnAwake = false;

        _soundChannels = new List<AudioSource>();
        for (int i = 0; i < _initSoundChannelCount; i++)
        {
            var audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            _soundChannels.Add(audioSource);
        }
    }

    #region Sound
    public float PlaySound(string clipName)
    {
        if (_soundList.AudioClips.TryGetValue(clipName, out AudioClip clip))
        {
            int channelIndex = GetEmptyChannelIndex(_soundChannels);

            _soundChannels[channelIndex].clip = clip;
            _soundChannels[channelIndex].Play();
            return _soundChannels[channelIndex].clip.length;
        }
        else
        {
            Debug.LogError("audio clip " + clipName + " does not exist");
            return 0f;
        }
    }

    private int GetEmptyChannelIndex(List<AudioSource> soundChannels)
    {
        for (int i = 0; i < soundChannels.Count; i++)
        {
            if (soundChannels[i].isPlaying == false)
                return i;
        }
        soundChannels.Add(this.gameObject.AddComponent<AudioSource>());
        return soundChannels.Count - 1;
    }

    public void StopSound()
    {
        for (int i = 0; i < _soundChannels.Count; i++)
        {
            _soundChannels[i].Stop();
        }
    }

    public void MuteSound()
    {
        for (int i = 0; i < _soundChannels.Count; i++)
        {
            _soundChannels[i].volume = 0f;
        }
    }

    public void ListenSound()
    {
        for (int i = 0; i < _soundChannels.Count; i++)
        {
            _soundChannels[i].volume = 1f;
        }
    }
    #endregion

    #region Music
    public float PlayMusic(string clipName)
    {
        if (_musicChannel.clip != null && _musicChannel.isPlaying && _musicChannel.clip.name.Equals(clipName))
        {
            return 0f;
        }

        if (_soundList.AudioClips.TryGetValue(clipName, out AudioClip clip))
        {
            _musicChannel.clip = clip;
            _musicChannel.Play();
            return _musicChannel.clip.length;
        }
        else
        {
            Debug.LogError("audio clip " + clipName + " does not exist");
            return 0f;
        }
    }

    public void StopMusic()
    {
        _musicChannel.Stop();
    }

    public void SetMusicVolum(float volume, float duration)
    {
        StartCoroutine(LerpVolume(_musicChannel, volume, duration));
    }

    private IEnumerator LerpVolume(AudioSource audioChannel, float targetVolume, float duration)
    {
        float beginVolume = audioChannel.volume;
        float volumeOffset = targetVolume - beginVolume;
        for (float time = 0; time < duration; time += Time.deltaTime)
        {
            audioChannel.volume = beginVolume + (volumeOffset * (time / duration));
            yield return null;
        }
        audioChannel.volume = targetVolume;
    }

    public void MuteMusic()
    {
        _musicChannel.volume = 0f;
    }

    public void ListenMusic()
    {
        _musicChannel.volume = 1f;
    }
    #endregion

    public void MuteAll()
    {
        MuteMusic();
        MuteSound();
    }

    public void ListenAll()
    {
        ListenMusic();
        ListenSound();
    }
}
