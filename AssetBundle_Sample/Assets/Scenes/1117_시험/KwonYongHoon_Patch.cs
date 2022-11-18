using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class KwonYongHoon_Patch : MonoBehaviour
{
    // 포트폴리오는 버전을 따로 체크하라는 이야기가 없기 때문에 버전관련 내용은 제외 시켰습니다.
    Dictionary<int, string> patchInfo;   // int : 인덱스, string : 패치 목록.

    private IEnumerator Start()
    {
        patchInfo = new Dictionary<int, string>();
        yield return StartCoroutine(DownloadBundle("DownLoadList.csv"));   // D://Game/PatchInfo 에서 파일받기(웹서버 묘사)
        yield return StartCoroutine(VersionPatch());    // 목록의 각각의 패치 파일 받기.
    }

    // 단계 1. 웹서버에서 패치 목록의 파일을 받은 뒤, 받아야되는 각각의 패치 파일을 딕셔너리에 정리한다.
    IEnumerator DownloadBundle(string _name)
    {
        string url = "file:///D:/Game/PatchInfo/" + _name;
        Debug.Log(url);
        UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(url);
        www.downloadHandler = new DownloadHandlerBuffer();
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("다운로드 리스트 파일 다운 실패 : " + www.error);  // 파일 받기 실패
        }
        else
        {
            // 다운로드 폴더가 없으면 폴더를 만들어줍니다.
            if (!System.IO.Directory.Exists("D:/Game/DownLoad"))
            {
                System.IO.Directory.CreateDirectory("D:/Game/DownLoad");
            }
            string downloadPath = "D:/Game/DownLoad/" + _name;
            Debug.Log(downloadPath);
            byte[] file = www.downloadHandler.data;   // 핸들러를 이용해서 데이터 저장 
            File.WriteAllBytes(downloadPath, file);   // 파일 저장

            using (StreamReader sr = new StreamReader(downloadPath))
            {
                string line = string.Empty;
                int lineCount = 0;
                // 첫번째 줄은 Colum의 정보를 표시해주는 줄입니다.(그래서 패치 목록에는 제외시켰습니다)
                // 두번째 줄부터 마지막 줄까지 정리해서 Dictionary에 저장.
                while ((line = sr.ReadLine()) != null)
                {
                    lineCount++;
                    if (lineCount >= 2)
                    {
                        string[] verInfo = line.Split(',');
                        Debug.Log($"인덱스 = {verInfo[0]}");
                        Debug.Log($"카테고리 = {verInfo[1]}");
                        Debug.Log($"파일이름 = {verInfo[2]}");
                    
                        // 확장자 수동으로 추가한 뒤 Dictionary에 추가하였습니다.
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
            Resources.UnloadUnusedAssets();    // 사용하지 않는 에셋의 메모리 반환
        }
    }


    // 단계 2. 딕셔너리에 저장된 패치 파일의 목록을 하나하나 웹서버에서 다운을 받는다.
    IEnumerator VersionPatch()
    {
        // 패치 받아야 할 파일을 딕셔너리에서 하나씩 읽어서 파일을 다운로드.
        foreach (KeyValuePair<int, string> item in patchInfo)
        {
            yield return null;  // 한번 업데이트 돌리고 실행
            yield return StartCoroutine(DownloadPatchFile(item.Value));
        }
        yield return null;
    }

    IEnumerator DownloadPatchFile(string _fileName)
    {
        // 게임패치 파일을 내려받고 파일의 내용을 읽어서 패치를 진행
        string url = "file:///D:/Game/PatchInfo/Source/" + _fileName; // 웹서버 파일을 받는 모습을 묘사
        UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(url);
        www.downloadHandler = new DownloadHandlerBuffer();
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("패치파일 다운로드 실패 :" + www.error);  // 파일 받기 실패
        }
        else
        {
            // 패치 파일의 내용이 아애 없다면 null에러가 발생합니다. 해당 에러를 처리하기 위한 예외처리구문입니다.
            try
            {
                // 폴더가 없으면 폴더를 만들어 줍니다.
                if (!System.IO.Directory.Exists("D:/Game/DownLoad/Patch"))
                {
                    System.IO.Directory.CreateDirectory("D:/Game/DownLoad/Patch");
                }
                string downloadPath = "D:/Game/DownLoad/Patch/" + _fileName;
                Debug.Log(downloadPath);
                byte[] file = www.downloadHandler.data;
                File.WriteAllBytes(downloadPath, file);   // 파일을 해당 위치에 저장
            }
            catch (ArgumentNullException e)
            {
                Debug.Log($"{_fileName}의 파일 내부에는 아무런 값이 없습니다." + e.Message);
                // 아무런 값이 없으면 빈 값이라도 넣어주고 파일을 다운로드 받게 합니다.
                byte[] emptyFile = new byte[0] { };
                File.WriteAllBytes("D:/Game/DownLoad/Patch/" + _fileName, emptyFile);
            }
        }
        yield return null;
    }
}
