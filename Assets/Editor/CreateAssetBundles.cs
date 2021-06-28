using System.Collections;
using System.Collections.Generic;
//Added editor support.
using UnityEditor;
using UnityEngine;
//Added system read support.
using System.IO;

public class CreateAssetBundles
{
    /// <summary>
    /// Menu item for building asset bundles.
    /// </summary>
    [MenuItem ("Assets/Build Asset Bundles")]
    static void BuildAllAssetBundles()
    {
        //Asset bundle output directory.
        string assetBundleDirectory = "Assets/Art/Output";

        //Check path to streaming assets directory.
        if (!Directory.Exists(Application.streamingAssetsPath))
        {
            Debug.Log("Folder 'Output' does not exist, creating new folder");

            //Create asset bundle directory if it does not already exist.
            Directory.CreateDirectory(assetBundleDirectory);
        }

        //Builds asset bundles based on bundle options and current build target.
        //You would probably want to setup a build machine to handle multiple build targets.
        //The build target can manually be set. For instance to Android or IOS.
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);

        //Bundle option notes (BuildAssetBundleOptions):
        //None: no special options.
        //DryRunBuild: this doesn't actully build the Asset Bundle, but goes through the process. Mainly used for error checking.
        //StrictMode: stops Asset Bundle creation if any error occurs.
        //UncompressedAssetBundle: builds, but does not compress bundles. Use this for testing.
        //Uses more disk space. Don't use this for final distribution builds.
    }
}
