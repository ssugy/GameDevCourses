using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class KwonYongHoon_Patch : MonoBehaviour
{
    // ��Ʈ�������� ������ ���� üũ�϶�� �̾߱Ⱑ ���� ������ �������� ������ ���� ���׽��ϴ�.
    Dictionary<int, string> patchInfo;   // int : �ε���, string : ��ġ ���.

    private IEnumerator Start()
    {
        patchInfo = new Dictionary<int, string>();
        yield return StartCoroutine(DownloadBundle("DownLoadList.csv"));   // D://Game/PatchInfo ���� ���Ϲޱ�(������ ����)
        yield return StartCoroutine(VersionPatch());    // ����� ������ ��ġ ���� �ޱ�.
    }

    // �ܰ� 1. ���������� ��ġ ����� ������ ���� ��, �޾ƾߵǴ� ������ ��ġ ������ ��ųʸ��� �����Ѵ�.
    IEnumerator DownloadBundle(string _name)
    {
        string url = "file:///D:/Game/PatchInfo/" + _name;
        Debug.Log(url);
        UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(url);
        www.downloadHandler = new DownloadHandlerBuffer();
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("�ٿ�ε� ����Ʈ ���� �ٿ� ���� : " + www.error);  // ���� �ޱ� ����
        }
        else
        {
            // �ٿ�ε� ������ ������ ������ ������ݴϴ�.
            if (!System.IO.Directory.Exists("D:/Game/DownLoad"))
            {
                System.IO.Directory.CreateDirectory("D:/Game/DownLoad");
            }
            string downloadPath = "D:/Game/DownLoad/" + _name;
            Debug.Log(downloadPath);
            byte[] file = www.downloadHandler.data;   // �ڵ鷯�� �̿��ؼ� ������ ���� 
            File.WriteAllBytes(downloadPath, file);   // ���� ����

            using (StreamReader sr = new StreamReader(downloadPath))
            {
                string line = string.Empty;
                int lineCount = 0;
                // ù��° ���� Colum�� ������ ǥ�����ִ� ���Դϴ�.(�׷��� ��ġ ��Ͽ��� ���ܽ��׽��ϴ�)
                // �ι�° �ٺ��� ������ �ٱ��� �����ؼ� Dictionary�� ����.
                while ((line = sr.ReadLine()) != null)
                {
                    lineCount++;
                    if (lineCount >= 2)
                    {
                        string[] verInfo = line.Split(',');
                        Debug.Log($"�ε��� = {verInfo[0]}");
                        Debug.Log($"ī�װ� = {verInfo[1]}");
                        Debug.Log($"�����̸� = {verInfo[2]}");
                    
                        // Ȯ���� �������� �߰��� �� Dictionary�� �߰��Ͽ����ϴ�.
                        if (verInfo[2].Equals("CharacterInfo"))
                        {
                            patchInfo.Add(int.Parse(verInfo[0]), verInfo[2] + ".csv");
                        }
                        else
                        {
                            patchInfo.Add(int.Parse(verInfo[0]), verInfo[2] + ".txt");
                        }
                    }
                }
                sr.Close();
            }
            Resources.UnloadUnusedAssets();    // ������� �ʴ� ������ �޸� ��ȯ
        }
    }


    // �ܰ� 2. ��ųʸ��� ����� ��ġ ������ ����� �ϳ��ϳ� ���������� �ٿ��� �޴´�.
    IEnumerator VersionPatch()
    {
        // ��ġ �޾ƾ� �� ������ ��ųʸ����� �ϳ��� �о ������ �ٿ�ε�.
        foreach (KeyValuePair<int, string> item in patchInfo)
        {
            yield return null;  // �ѹ� ������Ʈ ������ ����
            yield return StartCoroutine(DownloadPatchFile(item.Value));
        }
        yield return null;
    }

    IEnumerator DownloadPatchFile(string _fileName)
    {
        // ������ġ ������ �����ް� ������ ������ �о ��ġ�� ����
        string url = "file:///D:/Game/PatchInfo/Source/" + _fileName; // ������ ������ �޴� ����� ����
        UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(url);
        www.downloadHandler = new DownloadHandlerBuffer();
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("��ġ���� �ٿ�ε� ���� :" + www.error);  // ���� �ޱ� ����
        }
        else
        {
            // ��ġ ������ ������ �ƾ� ���ٸ� null������ �߻��մϴ�. �ش� ������ ó���ϱ� ���� ����ó�������Դϴ�.
            try
            {
                // ������ ������ ������ ����� �ݴϴ�.
                if (!System.IO.Directory.Exists("D:/Game/DownLoad/Patch"))
                {
                    System.IO.Directory.CreateDirectory("D:/Game/DownLoad/Patch");
                }
                string downloadPath = "D:/Game/DownLoad/Patch/" + _fileName;
                Debug.Log(downloadPath);
                byte[] file = www.downloadHandler.data;
                File.WriteAllBytes(downloadPath, file);   // ������ �ش� ��ġ�� ����
            }
            catch (ArgumentNullException e)
            {
                Debug.Log($"{_fileName}�� ���� ���ο��� �ƹ��� ���� �����ϴ�." + e.Message);
                // �ƹ��� ���� ������ �� ���̶� �־��ְ� ������ �ٿ�ε� �ް� �մϴ�.
                byte[] emptyFile = new byte[0] { };
                File.WriteAllBytes("D:/Game/DownLoad/Patch/" + _fileName, emptyFile);
            }
        }
        yield return null;
    }
}
