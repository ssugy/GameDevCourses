using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;

public class AssetBundleMenue
{
    // �� �޴��� ������ ���¹����� ���̺��ϴ� ����� �Ѵ�.
    [MenuItem("AssetBundle/Save")]
    static void SaveAssetBundle()
    {
        // �� ������ ���¹����� ����� ��.
        BuildPipeline.BuildAssetBundles("Assets/AssetBundles", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
    }

    /**
     * ���¹����� �ε��ϱ� ���ؼ���
     * 1. �ڷ�ƾ�� �޴��ɼǿ��� �ȵ�
     * 2. 
     */
    //[MenuItem("AssetBundle/Load")]
    //static IEnumerator LoadAssetBundle()
    //{
    //    // 1. ���¹��� �ε��ϴ� ���
    //    string url = "file:///" + Application.dataPath + "/AssetBundles/" + "cube_0.assetbd";
    //    UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(url);
    //    yield return www.SendWebRequest();
    //    AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);
    //    var prefab = bundle.LoadAsset<GameObject>("Cube");

    //    GameObject go = GameObject.Instantiate(prefab);
    //    go.transform.position = Vector3.zero;
    //}
}
