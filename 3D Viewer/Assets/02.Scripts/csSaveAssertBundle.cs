#if UNITY_EDITOR
using UnityEditor;
#endif


public class DoCreateAssetBundles
{
#if UNITY_EDITOR
    [MenuItem("Assets/Build Asset Bundles")]
    static void CreateBundles()
    {
        //빌드 타겟의 경우 로더가 어디서 불러오느냐에 따라 바꿔주어야 하며 동적으로 변경가능하다 ​
        BuildPipeline.BuildAssetBundles("Assets/AssetBundles", BuildAssetBundleOptions.None, BuildTarget.Android);

    }
#endif
}
