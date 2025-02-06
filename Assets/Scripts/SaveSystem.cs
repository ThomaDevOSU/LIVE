using UnityEngine;
using System.IO;
using Newtonsoft.Json;

public class SaveSystem
{
    /// <summary>
    /// GetSaveFilePath returns the save file path to a given save slot.
    /// Typically it will be in /Appdata/LocalLow/DefaultCompany/Live
    /// </summary>
    /// <param name="slot"></param>
    /// <returns></returns>
    public static string GetSaveFilePath(int slot) 
    {
        return Application.persistentDataPath + $"/savefile{slot}.json";
    }

    /// <summary>
    /// GetOptionsPath simply returns the location where options are saved.
    /// Typically it will be in /Appdata/LocalLow/DefaultCompany/Live
    /// </summary>
    /// <returns></returns>
    public static string GetOptionsPath()
    {
        return Application.persistentDataPath + $"/options.json";
    }

    /// <summary>
    /// SaveGame is a function which saves all player data locally to a valid SaveFilePath.
    /// It does this by serializing the player class into a string, and writing that string into a file.
    /// </summary>
    /// <param name="data"></param>
    /// <param name="slot"></param>
    public static void SaveGame(PlayerData data, int slot) 
    {
        using (StreamWriter file = File.CreateText(GetSaveFilePath(slot))) 
        {
            JsonSerializer serializer = new JsonSerializer() { Formatting = Formatting.Indented };
            serializer.Serialize(file, data);
        }
    }

    /// <summary>
    /// LoadGame attempts to deserialize a JSON file from the given slot location using the NewtonSoft deserializer
    /// If a valid save file cannot be found, An error is returned
    /// </summary>
    /// <param name="slot"></param>
    /// <returns></returns>
    public static PlayerData LoadGame(int slot) 
    {
        if (File.Exists(GetSaveFilePath(slot)))
        {
            using (StreamReader file = File.OpenText(GetSaveFilePath(slot)))
            {
                JsonSerializer serializer = new JsonSerializer() { Formatting = Formatting.Indented };
                return (PlayerData)serializer.Deserialize(file, typeof(PlayerData));
            }
        }
        else 
        {
            Debug.LogError("Save file not found!");
            return null;
        }
    }

    /// <summary>
    /// Delete Save simply deletes a given save file at a slot
    /// </summary>
    /// <param name="slot"></param>
    public static void deleteSave(int slot) 
    {
        string path = GetSaveFilePath(slot);
        if (File.Exists(path))
        { 
            File.Delete(path);
            return;
        }
        Debug.LogError("Save file not found at this location"); //  This should never be reached
    }

    /// <summary>
    /// SaveOptions serializes the given option data class into a string and saves it into a file.
    /// </summary>
    /// <param name="data"></param>
    public static void SaveOptions(OptionData data)
    {
        using (StreamWriter file = File.CreateText(GetOptionsPath()))
        {
            JsonSerializer serializer = new JsonSerializer() { Formatting = Formatting.Indented };
            serializer.Serialize(file, data);
        }
    }

    /// <summary>
    /// LoadOptions will load the JSON options file into a optiondata object and return it.
    /// If the file cannot be found, it returns a default english optiondata object
    /// </summary>
    /// <returns></returns>
    public static OptionData LoadOptions()
    {
        if (File.Exists(GetOptionsPath()))
        {
            using (StreamReader file = File.OpenText(GetOptionsPath()))
            {
                JsonSerializer serializer = new JsonSerializer() { Formatting = Formatting.Indented };
                return (OptionData)serializer.Deserialize(file, typeof(OptionData));
            }
        }
        else
        {
            Debug.Log("Option file not found! Creating new Option Data");
            return new OptionData
            {
                language = "en"
            };
        }
    }
}
