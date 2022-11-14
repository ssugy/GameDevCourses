using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;

public class AssetBundleMenue
{
    // 이 메뉴를 누르면 에셋번들을 세이브하는 기능을 한다.
    [MenuItem("AssetBundle/Save")]
    static void SaveAssetBundle()
    {
        // 이 한줄은 에셋번들을 만드는 것.
        BuildPipeline.BuildAssetBundles("Assets/AssetBundles", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
    }

    /**
     * 에셋번들을 로드하기 위해서는
     * 1. 코루틴은 메뉴옵션에서 안됨
     * 2. 
     */
    //[MenuItem("AssetBundle/Load")]
    //static IEnumerator LoadAssetBundle()
    //{
    //    // 1. 에셋번들 로드하는 방법
    //    string url = "file:///" + Application.dataPath + "/AssetBundles/" + "cube_0.assetbd";
    //    UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(url);
    //    yield return www.SendWebRequest();
    //    AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);
    //    var prefab = bundle.LoadAsset<GameObject>("Cube");

    //    GameObject go = GameObject.Instantiate(prefab);
    //    go.transform.position = Vector3.zero;
    //}
}
