using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Added system read support.
using System.IO;

public class LoadAssetBundles : MonoBehaviour
{
    [Header ("Bundle Name")]
    public string bundleName;
    [Header("Asset Name")]
    public string assetName;

    //Changed return type to IEnumerator, allows for running of a coroutine.
    //Executed over multiple frames.
    private IEnumerator Start()
    {
        //Async asset bundle request.
        AssetBundleCreateRequest asyncBundleRequest = AssetBundle.LoadFromFileAsync(Path.Combine(Application.streamingAssetsPath, bundleName));
        yield return asyncBundleRequest;

        //Get the local asset bundle.
        AssetBundle localAssetBundle = asyncBundleRequest.assetBundle;

        //Check if asset bundle actually exists.
        if (localAssetBundle == null)
        {
            Debug.Log("Failed to load asset bundle " + localAssetBundle.name);
            yield break;
        }


    }
}
