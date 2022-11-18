using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Networking;

public class AssetBundleLoad : MonoBehaviour
{
    private void Start()
    {
        //StartCoroutine(LoadBundle());   // ���1 - file�������� �̿�
        StartCoroutine(DownLoadBundle("cube_0"));   // ���2 ���������� ��ġ �޴� ���� ����
    }

    // 1. ���¹��� �ε��ϴ� ��� - file�̿��� �ε�
    IEnumerator LoadBundle()
    {
        string url = "file:///" + Application.dataPath + "/AssetBundles/" + "cube_0.assetbd";   // 1. ���� ����� �����͵��� �ִ���ġ.
        UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(url);
        yield return www.SendWebRequest();
        // ���� �Ʒ� ���ʹ� www.SendWebRequest�Ϸ�� ����
        AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);
        // ���¹����� �����°�. ����? �޸𸮻����� �����Դ�.

        var prefab = bundle.LoadAsset<GameObject>("Cube");
        //var prefab = bundle.LoadAllAssets<GameObject>();

        GameObject go = GameObject.Instantiate(prefab);
        go.transform.position = Vector3.zero;

        bundle.Unload(false);    // �ش� ���¹����� �����͸� free�� ����
        Resources.UnloadUnusedAssets();    // ������� �ʴ� ������ �޸� ��ȯ
    }

    // 2. ����Ʈ�� ���� ����
    // �������� �ִ� ������ �������� ��. �������� ������ �ٽ� �ε��ؼ� �ҷ���
    IEnumerator DownLoadBundle(string _name)
    {
        // ���� : �ƴ�.. �������� �ִ� ������ �����޴°Ŷ�� �ϴµ�, �� ���⼭�� file:///�̵��� ������?
        // �츮�� �������� �ȹ�����. => ��������� �׳� �����սô�. �ƽð��� ������? �̷��� ����.
        string url = "file:///" + Application.dataPath + "/AssetBundles/" + _name + ".assetbd";
        UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(url);
        www.downloadHandler = new DownloadHandlerBuffer();  // �̰ſ־�? -> .data�� �����ؼ� ���� �������� ���ؼ���.
        yield return www.SendWebRequest();  // ����

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);  // ���� �ޱ� ����
        }
        else
        {
            string fullPath = Application.dataPath + "/DownLoadBundle/" + _name + ".assetbd";
            byte[] file = www.downloadHandler.data; // �̷� �������� ������δ� ������ �ȵȴٰ� ������
            File.WriteAllBytes(fullPath, file);   // ���� ������ ����� ����.

            var myLoadAssetbundle = AssetBundle.LoadFromFile(fullPath);  // ������ ����Ʈ�迭�� ���¹��� ���·� load

            GameObject[] prefabs = myLoadAssetbundle.LoadAllAssets<GameObject>();
            //GameObject prefabs2 = myLoadAssetbundle.LoadAsset<GameObject>("cube");
            foreach (GameObject item in prefabs)
            {
                GameObject obj = Instantiate<GameObject>(item);
                obj.transform.position = Vector3.zero;
                obj.name = item.name;
            }

            myLoadAssetbundle.Unload(false);    // ������ free�� ����
            Resources.UnloadUnusedAssets();    // ������� �ʴ� ������ �޸� ��ȯ
        }
    }
}
