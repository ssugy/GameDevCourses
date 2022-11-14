using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

/**
 * 패치정보는 웹에서 원래 받아야 한다.
 * 그래서 지금은 파일로 받아온다.
 * 
 * 1. 게임 실행 시 패치 정보의 문서를 다운로드.
 */
public class Patch : MonoBehaviour
{
    double currentVersion;
    double latestVersion;
    Dictionary<double, string> patchInfo;   // double : 버전, string : 패치 목록.

    private IEnumerator Start()
    {
        patchInfo = new Dictionary<double, string>();
        currentVersion = double.Parse(PlayerPrefs.GetString("Version","1.0"));
        yield return StartCoroutine(DownloadBundle("GamePatchInfo.csv"));   // 패치 받아야 할 파일 리스트 받고
        yield return StartCoroutine(VersionPatch());    // 패치 받기.
    }

    /**
     * 인간 : hp:300, att:10, ... 데이터 저장
     * 
     * 1. 하드코딩 : 코드에다가 직접 박는 것.
     * 2. csv, json, xml이런 파일의 형식으로 저장하는 것. -> 게임을 실행 할 때 해당 파일에 접근해서 데이터를 가져오는 것.
     * 3. 서버에서 데이터를 가져오는 것.
     *  - 서버에는 어떻게 저장하냐? RDB : Relation DataBase : 관계형(데이터들 끼리의 관계) 데이터베이스 ex) 데이터들이 관계있는 테이블로 이루어짐.
     *  사용자는 쿼리를 통해서 질문을 하고 답변을 받음.
     * 4. 클라이언트 내부에 저장하는 것. - PlayerPrefabs라는 클래스를 사용 -> 작은데이터들을 쉽게 저장 할 수 있음. -> 10Mb이상 저장안됨.
     */

    IEnumerator VersionPatch()
    {
        if (latestVersion == currentVersion)
            yield break;

        // 패치 받아야 할 버전별 파일을 하나씩 읽어서 파일을 다운로드.
        foreach (KeyValuePair<double,string> item in patchInfo)
        {
            currentVersion = item.Key;
            yield return null;  // 한번 업데이트 돌리고 아래 실행
            yield return StartCoroutine(ReadVersionPatch(item.Value));
            Debug.Log(item.Key);
        }
        yield return null;
    }

    IEnumerator ReadVersionPatch(string _fileName)
    {
        // 게임패치 파일을 내려받고 파일의 내용을 읽어서 패치를 진행
        string url = $"file:///{Application.dataPath}/PatchInfo/{_fileName}"; //==> 웹서버로 변경되면, 실제 패치받는것과 동일 행동.

        using (StreamReader sr = new StreamReader(_fileName))
        {
            string line = string.Empty;
            while ((line = sr.ReadLine()) != null)
            {
                // 패치 받아야 할 에셋 번들 내용
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
            Debug.Log(www.error);  // 파일 받기 실패
        }
        else
        {
            string downloadPath = Application.dataPath + "/PatchDownLoad/" + _name;
            Debug.Log(downloadPath);
            byte[] file = www.downloadHandler.data; // 이런 원시적인 방법으로는 접근이 안된다고 에러뜸(핸들러 이용하면 괜찮음)
            File.WriteAllBytes(downloadPath, file);   // file을 해당경로에다가 write하라는 의미

            using (StreamReader sr = new StreamReader(downloadPath))
            {
                string line = string.Empty;
                while ((line = sr.ReadLine()) != null)
                {
                    Debug.Log(line);
                    string[] verInfo = sr.ReadLine().Split(',');
                    Debug.Log($"버전 = {verInfo[0]}");
                    Debug.Log($"패치 내용 파일 = {verInfo[1]}");
                    latestVersion = double.Parse(verInfo[0]);
                    patchInfo.Add(latestVersion, verInfo[1]);
                }
                sr.Close(); // 이거안써도됨.
            }

            // 이 다음에는 뭐해야되나?
            // 패치해줘야됩니다!!!
            // 어디에있는 내용의 패치?
            //patchInfo의 내용을 실제로 패치 작업을 해야된다. -> 오늘까지는 하지 않음. 

            //var myLoadAssetbundle = AssetBundle.LoadFromFile(downloadPath);  // 저장한 바이트배열을 에셋번들 형태로 load

            //GameObject[] prefabs = myLoadAssetbundle.LoadAllAssets<GameObject>();
            //foreach (GameObject item in prefabs)
            //{
            //    GameObject obj = Instantiate<GameObject>(item);
            //    obj.transform.position = Vector3.zero;
            //    obj.name = item.name;
            //}

            //myLoadAssetbundle.Unload(false);    // 에셋을 free로 만듬
            Resources.UnloadUnusedAssets();    // 사용하지 않는 에셋의 메모리 반환
        }

    }
}
