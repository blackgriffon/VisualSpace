
using System.Collections;
using System.Collections.Generic;
using UnityEditor;




public class csSaveAssertBundle
{

    [MenuItem("Assets/TESTT/Build AssetBundles")]
    public static void makeAssetBundles()
    {

        BuildPipeline.BuildAssetBundles("Assets/AssetBundles", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);

    }


    [MenuItem("Assets/TESTT/Build AssetBundles WebGl")]
    public static void makeAssetBundlesWebGl()
    {

        BuildPipeline.BuildAssetBundles("Assets/AssetBundles", BuildAssetBundleOptions.None, BuildTarget.WebGL);

    }

}
