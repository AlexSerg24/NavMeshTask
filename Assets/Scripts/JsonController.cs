using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text;

public class JsonController : MonoBehaviour
{
    public LevelSaves saves;

    void Start()
    {
        Debug.Log(Application.persistentDataPath);
        LoadField();
        SaveAll(saves);
        Debug.Log("level - " + saves.level + ", bots - " + saves.botsNum);
    }

    [ContextMenu("Load")]
    public void LoadField()
    {
        string path = Application.persistentDataPath + "/JSON.json";

        // if the file path or name does not exist
        if (!Directory.Exists(Path.GetDirectoryName(path)))
        {
            Debug.LogWarning("File or path does not exist! " + path);
            Debug.Log("File was craeted");
            return;
        }

        // load in the save data as byte array
        byte[] jsonDataAsBytes = null;

        try
        {
            jsonDataAsBytes = File.ReadAllBytes(path);
            Debug.Log("<color=green>Loaded all data from: </color>" + path);
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed to load data from: " + path);
            Debug.LogWarning("Error: " + e.Message);
            return;
        }

        if (jsonDataAsBytes == null)
            return;

        // convert the byte array to json
        string jsonData;

        // convert the byte array to json
        jsonData = Encoding.UTF8.GetString(jsonDataAsBytes);

        // convert to the specified object type
        //AllObjects returnedData;

        JsonUtility.FromJsonOverwrite(jsonData, saves);
    }


    [ContextMenu("Save")]
    public void SaveAll(LevelSaves ls)
    {      
        File.WriteAllText(Application.persistentDataPath + "/JSON.json", JsonUtility.ToJson(ls));
    }

    [System.Serializable]
    public class LevelSaves
    {
        public string level;
        public int botsNum;
    }
}
