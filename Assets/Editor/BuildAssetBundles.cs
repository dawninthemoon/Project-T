#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO; 

public class BuildAsssetBundles { 
     [MenuItem("Bundles/Build AssetBundles")] 
     static void BuildAllAssetBundles() { 
        string assetBundleDirectory = "Assets/AssetBundles";
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
    } 
}

