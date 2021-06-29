using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Added editor support.
using UnityEditor;
//Added system read support.
using System.IO;
//Added support for reading assets from a remote sources.
using System.Net;

public class CustomImportSettings : AssetPostprocessor
{
    //Input directory location
    string inputDirectory = "Assets/Art/Input";
    //Local destination of JSON file.
    string configPath = "Assets/Art/JSON/config.json";

    void OnPreprocessAsset()
    {
        string JSONData = "";

        using (StreamReader stream = new StreamReader(configPath))
        {
            //Read the JSON contents.
            JSONData = stream.ReadToEnd();
        }

        //Get the config collection information.
        ConfigData configsInJSON = JsonUtility.FromJson<ConfigData>(JSONData);

        //Check path to texture size 0 assets directory.
        if (!Directory.Exists("Assets/Art/Input/" + configsInJSON.config.target_sizes[0]))
        {
            Debug.Log("Asset " + configsInJSON.config.target_sizes[0] + "directory does not exist, creating new folder.");

            //Create asset bundle directory if it does not already exist.
            Directory.CreateDirectory("Assets/Art/Input/" + configsInJSON.config.target_sizes[0]);
        }

        //Check path to texture size 1 assets directory.
        if (!Directory.Exists("Assets/Art/Input/" + configsInJSON.config.target_sizes[1]))
        {
            Debug.Log("Asset " + configsInJSON.config.target_sizes[1] + "directory does not exist, creating new folder.");

            //Create asset bundle directory if it does not already exist.
            Directory.CreateDirectory("Assets/Art/Input/" + configsInJSON.config.target_sizes[1]);
        }

        //Check path to texture size 2 assets directory.
        if (!Directory.Exists("Assets/Art/Input/" + configsInJSON.config.target_sizes[2]))
        {
            Debug.Log("Asset " + configsInJSON.config.target_sizes[2] + "directory does not exist, creating new folder.");

            //Create asset bundle directory if it does not already exist.
            Directory.CreateDirectory("Assets/Art/Input/" + configsInJSON.config.target_sizes[2]);
        }

        //Check path to converted images assets directory.
        if (!Directory.Exists("Assets/Art/ConvertedImages"))
        {
            Debug.Log("Asset converted images directory does not exist, creating new folder.");

            //Create asset bundle directory if it does not already exist.
            Directory.CreateDirectory("Assets/Art/ConvertedImages");
        }
    }

    //Add this function to a subclass to get a notification when a texture has completed importing.
    void OnPreprocessTexture()
    {
        string JSONData = "";

        using (StreamReader stream = new StreamReader(configPath))
        {
            //Read the JSON contents.
            JSONData = stream.ReadToEnd();
        }

        //Get the config collection information.
        ConfigData configsInJSON = JsonUtility.FromJson<ConfigData>(JSONData);

        //Get the texture importer.
        TextureImporter textureImporter = assetImporter as TextureImporter;

        //Get asset located in input folder
        var asset = AssetDatabase.LoadAssetAtPath(inputDirectory, typeof(Texture2D));

        if (!asset)
        {
            //Set texture to be readable, so that we can edit the texture sizes.
            textureImporter.isReadable = true;

            if (assetPath.Contains("_NormalMap"))
            {
                //Set texture import type to normal map
                textureImporter.textureType = TextureImporterType.NormalMap;
            }

            if (assetPath.Contains("_ConvertToNormalMap"))
            {
                //Convert texture to normal map
                textureImporter.convertToNormalmap = true;
            }

            //Set texture default max size
            textureImporter.maxTextureSize = configsInJSON.config.target_sizes[0];

            //Check if texture(s) are in input folder.
            if (assetPath.IndexOf(inputDirectory) == 0)
            {
                if (assetPath.Contains(".png"))
                {
                    //Get asset path and replace texture size 0 with texture size 1.
                    string configTextureSize0 = configsInJSON.config.target_sizes[0].ToString();
                    string configTextureSize1 = configsInJSON.config.target_sizes[1].ToString();
                    string configTextureSize2 = configsInJSON.config.target_sizes[2].ToString();

                    string asset1Path = (assetPath.Replace(configTextureSize0, configTextureSize1));

                    //Make a copy of original asset and move it to texture size 1 directory.
                    AssetDatabase.CopyAsset(assetPath, asset1Path);

                    //Get asset path and replace texture size 0 with texture size 2.
                    string asset2Path = (assetPath.Replace(configTextureSize0, configTextureSize2));

                    //Make a copy of original asset and move it to texture size 2 directory.
                    AssetDatabase.CopyAsset(assetPath, asset2Path);
                }
            }

            //Save asests.
            //AssetDatabase.SaveAssets();
        }
        else
        {
            Debug.Log("Asset already processed.");
        }
    }

    //Add this function to a subclass to get a notification when a texture has completed importing just before.
    void OnPostprocessTexture(Texture2D inputTexture)
    {
        //Check if texture(s) are in input folder.
        if (assetPath.IndexOf(inputDirectory) == -1)
        {
            Debug.LogError("Asset is not in the correct folder location!");
            //The removal of asset bundle names get a bit sticky if there already exists a generated asset bundle.
            //Remove asset bundle names if it is not located in input folder.
            AssetDatabase.RemoveAssetBundleName(assetPath, true);
            //Remove any unused asset bundle names.
            AssetDatabase.RemoveUnusedAssetBundleNames();
            return;
        }

        string JSONData = "";

        using (StreamReader stream = new StreamReader(configPath))
        {
            //Read the JSON contents.
            JSONData = stream.ReadToEnd();
        }

        //Get the config collection information.
        ConfigData configsInJSON = JsonUtility.FromJson<ConfigData>(JSONData);

        //Get the asset importer based off of input texture asset path.
        AssetImporter assetImporter = AssetImporter.GetAtPath(assetPath);

        //Get last index of / in asset path.
        int idx = (assetPath.LastIndexOf('/')) + 1;
        //Get asset name based off of path; and covert to lower case.
        string actualAssetName = (assetPath.Remove(0, idx));
        
        string bundleName = Path.GetFileNameWithoutExtension(assetPath);

        //Remove any unused asset bundle names.
        AssetDatabase.RemoveUnusedAssetBundleNames();

        if (!actualAssetName.Contains(".png"))
        {
            int configTextureSize0 = configsInJSON.config.target_sizes[0];
            TextureScaler.ThreadedScalePNG(inputTexture, configTextureSize0, configTextureSize0, true, bundleName);
            AssetDatabase.DeleteAsset(assetPath);
        }
        else
        {
            if (assetPath.Contains(configsInJSON.config.target_sizes[0].ToString()))
            {
                int configTextureSize0 = configsInJSON.config.target_sizes[0];
                //Scale input texture using bilinear scaling.
                TextureScaler.Bilinear(inputTexture, configTextureSize0, configTextureSize0);
                //Scale input texture using point scaling.
                //TextureScaler.Point(inputTexture, configTextureSize0, configTextureSize0);

                //Set asset bundle name and variant.
                assetImporter.SetAssetBundleNameAndVariant(bundleName, configTextureSize0.ToString());
            }

            if (assetPath.Contains(configsInJSON.config.target_sizes[1].ToString()))
            {
                int configTextureSize1 = configsInJSON.config.target_sizes[1];
                //Scale input texture using bilinear scaling.
                TextureScaler.Bilinear(inputTexture, configTextureSize1, configTextureSize1);
                //Scale input texture using point scaling.
                //TextureScaler.Point(inputTexture, configTextureSize1, configTextureSize1);

                //Set asset bundle name and variant.
                assetImporter.SetAssetBundleNameAndVariant(bundleName, configTextureSize1.ToString());
            }

            if (assetPath.Contains(configsInJSON.config.target_sizes[2].ToString()))
            {
                int configTextureSize2 = configsInJSON.config.target_sizes[2];
                //Scale input texture using bilinear scaling.
                TextureScaler.Bilinear(inputTexture, configTextureSize2, configTextureSize2);
                //Scale input texture using point scaling
                //TextureScaler.Point(inputTexture, configTextureSize2, configTextureSize2);

                //Set asset bundle name and variant.
                assetImporter.SetAssetBundleNameAndVariant(bundleName, configTextureSize2.ToString());
            }
        }

        AssetDatabase.SaveAssets();
    }

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (string str in importedAssets)
        {
            Debug.Log("Reimported Asset: " + str);
        }

        foreach (string str in deletedAssets)
        {
            Debug.Log("Deleted Asset: " + str);
        }

        for (int i = 0; i < movedAssets.Length; i++)
        {
            Debug.Log("Moved Asset: " + movedAssets[i] + " from: " + movedFromAssetPaths[i]);
        }

        AssetDatabase.Refresh();

        BuildAllAssetBundles();
    }

    static void BuildAllAssetBundles()
    {
        // Clear all loaded assetbundles
        Caching.ClearCache();

        //Asset bundle output directory.
        string assetBundleDirectory = "Assets/Art/Output";

        //Check path to streaming assets directory.
        if (!Directory.Exists(assetBundleDirectory))
        {
            Debug.Log("Asset bundle directory does not exist, creating new folder.");

            //Create asset bundle directory if it does not already exist.
            Directory.CreateDirectory(assetBundleDirectory);
        }

        //Builds asset bundles based on bundle options and current build target.
        //You would probably want to setup a build machine to handle multiple build targets.
        //The build target can manually be set. For instance to Android or IOS.
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);

        AssetDatabase.Refresh();
    }
}
