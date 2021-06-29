using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Net;

[Serializable]
public class CustomJSONSettings
{
    public string JSONAddress = "https://space-stage.s3.amazonaws.com/ta-test-config.json";

    public int[] targetSizes;
    public string format;

    
    public void ReadJSONFile()
    {
        using (var client = new WebClient())
        {
            string JSON = client.DownloadString(JSONAddress);
            var JSONText = JsonUtility.FromJson<CustomJSONSettings>(JSON);

            Debug.Log(JSONText);
        }
    }
}
