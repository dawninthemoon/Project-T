#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO; 
using UnityEngine;

public class BuildAsssetBundles { 
    [MenuItem("Bundles/Build AssetBundles")] 
    public static void BuildAllAssetBundles() { 
        string assetBundleDirectory = Application.streamingAssetsPath + "/AssetBundles";
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
    }
}

