using UnityEngine;
using System.IO;
public class SaveSystem
{
    public static string GetSaveFilePath(int slot) 
    {
        return Application.persistentDataPath + $"/savefile{slot}.json";
    }

        public static void SaveGame(PlayerData data, int slot) 
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetSaveFilePath(slot), json);
    }

    public static PlayerData LoadGame(int slot) 
    {
        string path = GetSaveFilePath(slot);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<PlayerData>(json);
        }
        else 
        {
            Debug.Log("Save file not found!");
            return null;
        }
    }

    public static void deleteSave(int slot) 
    {
        string path = GetSaveFilePath(slot);
        if (File.Exists(path))
        { 
            File.Delete(path);
        }
    }

    // The following code has to do with general Options
    public static string GetOptionsPath()
    {
        return Application.persistentDataPath + $"/options.json";
    }

    public static void SaveOptions(OptionData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetOptionsPath(), json);
    }

    public static OptionData LoadOptions()
    {
        string path = GetOptionsPath();
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<OptionData>(json);
        }
        else
        {
            Debug.Log("Save file not found!");
            return new OptionData
            {
                language = "English"
            };
        }
    }
}
