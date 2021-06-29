using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using System.Net;

public class ConfigLoader
{
    //Test editor function for saving remote JSON file
    [MenuItem("Assets/JSON/Save Remote JSON")]
    static void SaveLocalJSONFile()
    {
        //String for storing JSON web address.
        string JSONAddress = "https://space-stage.s3.amazonaws.com/ta-test-config.json";
        string saveFileLocation = "Assets/Art/JSON/config.json";

        if (File.Exists(saveFileLocation))
        {
            //We could force overwrite here if need be.
            Debug.Log("Local JSON file already exists!");
        }
        else
        {
            //Create local copy of remote file.
            Debug.Log("Local JSON file does not exist, create a copy.");

            using (var client = new WebClient())
            {
                string JSONData = client.DownloadString(JSONAddress);

                //ConfigData configsInJSON = JsonUtility.FromJson<ConfigData>(JSONData);

                //Use the write all text to save remote content.
                File.WriteAllText(saveFileLocation, JSONData);
            }
        }
    }

    //Test editor function for reading remote JSON file.
    [MenuItem("Assets/JSON/Read Remote JSON")]
    static void ReadRemoteJSONConfig()
    {
        //String for storing JSON web address.
        string JSONAddress = "https://space-stage.s3.amazonaws.com/ta-test-config.json";

        using (var client = new WebClient())
        {
            //Get JSON data from web address.
            string JSONData = client.DownloadString(JSONAddress);

            ConfigData configsInJSON = JsonUtility.FromJson<ConfigData>(JSONData);

            foreach (int size in configsInJSON.config.target_sizes)
            {
                Debug.Log(size);
            }
        }
    }

    //Test editor function for reading local JSON file.
    [MenuItem("Assets/JSON/Read Local JSON")]
    static void ReadLocalJSONConfig()
    {
        //Local destination of JSON file based off of web version.
        string configPath = "Assets/Art/JSON/config.json";

        using (StreamReader stream = new StreamReader(configPath))
        {
            //Read the JSON contents.
            string JSONData = stream.ReadToEnd();

            //Get the config collection information.
            ConfigData configsInJSON = JsonUtility.FromJson<ConfigData>(JSONData);

            foreach (int size in configsInJSON.config.target_sizes)
            {
                Debug.Log(size);
            }
        }
    }
}
