using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class KeystoreSetter
{
    static KeystoreSetter()
    {
        // 유니티가 초기화되면 키스토어 설정
        PlayerSettings.Android.useCustomKeystore = true;
        PlayerSettings.Android.keystoreName = "키스토어 경로";
        PlayerSettings.Android.keystorePass = "키스토어 암호";
        PlayerSettings.Android.keyaliasName = "키 alias 이름";
        PlayerSettings.Android.keyaliasPass = "키 alias 암호";

        Debug.Log("[KeystoreSetter] 키스토어 설정 완료");
    }
}

