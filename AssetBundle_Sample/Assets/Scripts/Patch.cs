using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

/**
 * ��ġ������ ������ ���� �޾ƾ� �Ѵ�.
 * �׷��� ������ ���Ϸ� �޾ƿ´�.
 * 
 * 1. ���� ���� �� ��ġ ������ ������ �ٿ�ε�.
 */
public class Patch : MonoBehaviour
{
    double currentVersion;
    double latestVersion;
    Dictionary<double, string> patchInfo;   // double : ����, string : ��ġ ���.

    private IEnumerator Start()
    {
        patchInfo = new Dictionary<double, string>();
        currentVersion = double.Parse(PlayerPrefs.GetString("Version","1.0"));
        yield return StartCoroutine(DownloadBundle("GamePatchInfo.csv"));   // ��ġ �޾ƾ� �� ���� ����Ʈ �ް�
        yield return StartCoroutine(VersionPatch());    // ��ġ �ޱ�.
    }

    /**
     * �ΰ� : hp:300, att:10, ... ������ ����
     * 
     * 1. �ϵ��ڵ� : �ڵ忡�ٰ� ���� �ڴ� ��.
     * 2. csv, json, xml�̷� ������ �������� �����ϴ� ��. -> ������ ���� �� �� �ش� ���Ͽ� �����ؼ� �����͸� �������� ��.
     * 3. �������� �����͸� �������� ��.
     *  - �������� ��� �����ϳ�? RDB : Relation DataBase : ������(�����͵� ������ ����) �����ͺ��̽� ex) �����͵��� �����ִ� ���̺�� �̷����.
     *  ����ڴ� ������ ���ؼ� ������ �ϰ� �亯�� ����.
     * 4. Ŭ���̾�Ʈ ���ο� �����ϴ� ��. - PlayerPrefabs��� Ŭ������ ��� -> ���������͵��� ���� ���� �� �� ����. -> 10Mb�̻� ����ȵ�.
     */

    IEnumerator VersionPatch()
    {
        if (latestVersion == currentVersion)
            yield break;

        // ��ġ �޾ƾ� �� ������ ������ �ϳ��� �о ������ �ٿ�ε�.
        foreach (KeyValuePair<double,string> item in patchInfo)
        {
            currentVersion = item.Key;
            yield return null;  // �ѹ� ������Ʈ ������ �Ʒ� ����
            yield return StartCoroutine(ReadVersionPatch(item.Value));
            Debug.Log(item.Key);
        }
        yield return null;
    }

    IEnumerator ReadVersionPatch(string _fileName)
    {
        // ������ġ ������ �����ް� ������ ������ �о ��ġ�� ����
        string url = $"file:///{Application.dataPath}/PatchInfo/{_fileName}"; //==> �������� ����Ǹ�, ���� ��ġ�޴°Ͱ� ���� �ൿ.

        using (StreamReader sr = new StreamReader(_fileName))
        {
            string line = string.Empty;
            while ((line = sr.ReadLine()) != null)
            {
                // ��ġ �޾ƾ� �� ���� ���� ����
            }
            sr.Close();
        }
        yield return null;
    }

    IEnumerator DownloadBundle(string _name)
    {
        string url = $"file:///{Application.dataPath}/PatchInfo/{_name}";
        Debug.Log(url);
        UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(url);
        www.downloadHandler = new DownloadHandlerBuffer();
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);  // ���� �ޱ� ����
        }
        else
        {
            string downloadPath = Application.dataPath + "/PatchDownLoad/" + _name;
            Debug.Log(downloadPath);
            byte[] file = www.downloadHandler.data; // �̷� �������� ������δ� ������ �ȵȴٰ� ������(�ڵ鷯 �̿��ϸ� ������)
            File.WriteAllBytes(downloadPath, file);   // file�� �ش��ο��ٰ� write�϶�� �ǹ�

            using (StreamReader sr = new StreamReader(downloadPath))
            {
                string line = string.Empty;
                while ((line = sr.ReadLine()) != null)
                {
                    Debug.Log(line);
                    string[] verInfo = sr.ReadLine().Split(',');
                    Debug.Log($"���� = {verInfo[0]}");
                    Debug.Log($"��ġ ���� ���� = {verInfo[1]}");
                    latestVersion = double.Parse(verInfo[0]);
                    patchInfo.Add(latestVersion, verInfo[1]);
                }
                sr.Close(); // �̰žȽᵵ��.
            }

            // �� �������� ���ؾߵǳ�?
            // ��ġ����ߵ˴ϴ�!!!
            // ����ִ� ������ ��ġ?
            //patchInfo�� ������ ������ ��ġ �۾��� �ؾߵȴ�. -> ���ñ����� ���� ����. 

            //var myLoadAssetbundle = AssetBundle.LoadFromFile(downloadPath);  // ������ ����Ʈ�迭�� ���¹��� ���·� load

            //GameObject[] prefabs = myLoadAssetbundle.LoadAllAssets<GameObject>();
            //foreach (GameObject item in prefabs)
            //{
            //    GameObject obj = Instantiate<GameObject>(item);
            //    obj.transform.position = Vector3.zero;
            //    obj.name = item.name;
            //}

            //myLoadAssetbundle.Unload(false);    // ������ free�� ����
            Resources.UnloadUnusedAssets();    // ������� �ʴ� ������ �޸� ��ȯ
        }

    }
}
