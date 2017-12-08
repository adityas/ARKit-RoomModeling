using UnityEditor;

public class CreateAssetBundles
{
    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        AssetDatabase.ImportAsset("Assets/model/model.obj", ImportAssetOptions.Default);

        AssetBundleBuild[] build = new AssetBundleBuild[1];
        build[0].assetBundleName = "modelios";
        string[] assetNames = new string[1];
        assetNames[0] = "Assets/model/model.obj";
        build[0].assetNames = assetNames;

		BuildPipeline.BuildAssetBundles("Assets/AssetBundles", build, BuildAssetBundleOptions.None, BuildTarget.iOS);
    }
}