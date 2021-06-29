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
    //Global default max size
    int textureDefaultMaxSize = 1024;
    //Input directory location
    string inputDirectory = "Assets/Art/Input";

    void OnPreprocessAsset(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        //Check path to 256 assets directory.
        if (!Directory.Exists("Assets/Art/Input/256"))
        {
            Debug.Log("Asset 512 directory does not exist, creating new folder.");

            //Create asset bundle directory if it does not already exist.
            Directory.CreateDirectory("Assets/Art/Input/256");
        }

        //Check path to 512 assets directory.
        if (!Directory.Exists("Assets/Art/Input/512"))
        {
            Debug.Log("Asset 512 directory does not exist, creating new folder.");

            //Create asset bundle directory if it does not already exist.
            Directory.CreateDirectory("Assets/Art/Input/512");
        }

        //Check path to 1024 assets directory.
        if (!Directory.Exists("Assets/Art/Input/1024"))
        {
            Debug.Log("Asset 1024 directory does not exist, creating new folder.");

            //Create asset bundle directory if it does not already exist.
            Directory.CreateDirectory("Assets/Art/Input/1024");
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

            textureImporter.maxTextureSize = textureDefaultMaxSize;

            //Check if texture(s) are in input folder.
            if (assetPath.IndexOf(inputDirectory) == 0)
            {
                if (assetPath.Contains(".png"))
                {
                    //Get asset path and replace 1024 with 256.
                    string asset256Path = (assetPath.Replace("1024", "256"));

                    //Make a copy of original asset and move it to 256 directory.
                    AssetDatabase.CopyAsset(assetPath, asset256Path);

                    //Get asset path and replace 1024 with 512.
                    string asset512Path = (assetPath.Replace("1024", "512"));

                    //Make a copy of original asset and move it to 512 directory.
                    AssetDatabase.CopyAsset(assetPath, asset512Path);
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
            TextureScaler.ThreadedScalePNG(inputTexture, 1024, 1024, true, bundleName);
            AssetDatabase.DeleteAsset(assetPath);
        }
        else
        {
            if (assetPath.Contains("256"))
            {
                //Scale input texture using bilinear scaling.
                TextureScaler.Bilinear(inputTexture, 256, 256);
                //Scale input texture using point scaling.
                //TextureScaler.Point(inputTexture, 256, 256);

                //Set asset bundle name and variant.
                assetImporter.SetAssetBundleNameAndVariant(bundleName, "256");
            }

            if (assetPath.Contains("512"))
            {
                //Scale input texture using bilinear scaling.
                TextureScaler.Bilinear(inputTexture, 512, 512);
                //Scale input texture using point scaling.
                //TextureScaler.Point(inputTexture, 512, 512);

                //Set asset bundle name and variant.
                assetImporter.SetAssetBundleNameAndVariant(bundleName, "512");
            }

            if (assetPath.Contains("1024"))
            {
                //Scale input texture using bilinear scaling.
                TextureScaler.Bilinear(inputTexture, 1024, 1024);
                //Scale input texture using point scaling
                //TextureScaler.Point(inputTexture, 1024, 1024);

                //Set asset bundle name and variant.
                assetImporter.SetAssetBundleNameAndVariant(bundleName, "1024");
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
