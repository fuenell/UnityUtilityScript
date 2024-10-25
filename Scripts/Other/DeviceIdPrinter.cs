using Microsoft.Win32;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

// 윈도우 고유 UUID 출력
public class DeviceIdPrinter : MonoBehaviour
{
    private readonly string UUIDFilePath = "./UUID.txt";
    private readonly string DUIDsavePath = "./DUID.txt";

    public Text text;

    void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        text.text = "";
        CheckID(SystemInfo.deviceUniqueIdentifier, "DUID", DUIDsavePath);
        CheckID(GetWindowsUUID(), "UUID", UUIDFilePath);
    }

    private void CheckID(string curID, string tag, string savePath)
    {
        if (File.Exists(savePath))
        {
            string[] lines = File.ReadAllLines(savePath);
            string preDUID = lines[0];
            if (!preDUID.Equals(curID))
            {
                text.text += "[" + tag + "]" + "키 충돌" + "\n";
                text.text += "[" + tag + "]" + "pre: " + preDUID + "\n";
                File.AppendAllText(savePath, curID + "\n");
            }
        }
        else
        {
            text.text += "[" + tag + "]" + "파일 생성" + "\n";
            File.WriteAllText(savePath, curID + "\n");
        }

        text.text += "[" + tag + "]" + "cur: " + curID + "\n";
        text.text += "\n";
    }


    private string GetWindowsUUID()
    {
        string UUID = "";
        try
        {
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey("SYSTEM\\HardwareConfig"))
            {
                if (key != null)
                {
                    System.Object o = key.GetValue("LastConfig");

                    string tempUUID = o as string;
                    UUID = tempUUID.Replace("{", "").Replace("}", "").ToUpper();
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(":: Can Find UUID/" + ex.Message);
        }
        return UUID;
    }
}
