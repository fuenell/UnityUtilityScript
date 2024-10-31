using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundList", menuName = "ScriptableObject/SoundList", order = int.MaxValue)]
public class SoundList : ScriptableObject
{
    public List<AudioClip> _audioClipList;

    private Dictionary<string, AudioClip> _audioClipDictionary;

    public Dictionary<string, AudioClip> AudioClips
    {
        get
        {
            if (_audioClipDictionary == null || _audioClipList.Count != _audioClipDictionary.Count)
                _audioClipDictionary = GetSoundList();

            return _audioClipDictionary;
        }
    }

    private const string ResourcesFolderPath = "Assets/Resources";
    private const string SoundSubFolder = "Sounds";
    private const string SoundListFileName = "SoundList";

    public static SoundList GetOrCreateSoundList()
    {
        string resourcePath = $"{SoundSubFolder}/{SoundListFileName}";
        string assetFolderPath = $"{ResourcesFolderPath}/{SoundSubFolder}";
        string assetPath = $"{assetFolderPath}/{SoundListFileName}.asset";

        SoundList soundList = Resources.Load<SoundList>(resourcePath);

        if (soundList != null)
        {
            return soundList;
        }

        if (Application.isEditor)
        {
            soundList = ScriptableObject.CreateInstance<SoundList>();

            // 한 줄로 폴더 생성
            Directory.CreateDirectory(assetFolderPath); // 상위 폴더가 없으면 자동으로 생성

            // SoundList를 지정된 경로에 저장
            AssetDatabase.CreateAsset(soundList, assetPath);
            AssetDatabase.SaveAssets();

            Debug.Log($"새로운 SoundList가 {assetPath}에 생성되었습니다.");
        }
        else
        {
            soundList = ScriptableObject.CreateInstance<SoundList>();
        }

        return soundList;
    }

    private Dictionary<string, AudioClip> GetSoundList()
    {
        Dictionary<string, AudioClip> audioDictionary = new Dictionary<string, AudioClip>();

        foreach (AudioClip item in _audioClipList)
        {
            if (audioDictionary.ContainsKey(item.name))
                continue;
            audioDictionary.Add(item.name, item);
        }

        return audioDictionary;
    }
}
