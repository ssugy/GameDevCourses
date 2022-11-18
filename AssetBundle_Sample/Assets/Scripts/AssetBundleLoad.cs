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
        //StartCoroutine(LoadBundle());   // 방법1 - file프로토콜 이용
        StartCoroutine(DownLoadBundle("cube_0"));   // 방법2 웹서버에서 패치 받는 것을 묘사
    }

    // 1. 에셋번들 로드하는 방법 - file이용한 로드
    IEnumerator LoadBundle()
    {
        string url = "file:///" + Application.dataPath + "/AssetBundles/" + "cube_0.assetbd";   // 1. 실제 저장된 데이터들이 있는위치.
        UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(url);
        yield return www.SendWebRequest();
        // 여기 아래 부터는 www.SendWebRequest완료된 상태
        AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);
        // 에셋번들을 가져온것. 어디로? 메모리상으로 가져왔다.

        var prefab = bundle.LoadAsset<GameObject>("Cube");
        //var prefab = bundle.LoadAllAssets<GameObject>();

        GameObject go = GameObject.Instantiate(prefab);
        go.transform.position = Vector3.zero;

        bundle.Unload(false);    // 해당 에셋번들의 데이터를 free로 만듬
        Resources.UnloadUnusedAssets();    // 사용하지 않는 에셋의 메모리 반환
    }

    // 2. 바이트로 파일 저장
    // 웹서버에 있는 파일을 내려받은 것. 내려받은 파일을 다시 로드해서 불러옴
    IEnumerator DownLoadBundle(string _name)
    {
        // 질문 : 아니.. 웹서버에 있는 파일을 내려받는거라고 하는데, 왜 여기서도 file:///이딴거 쓰나요?
        // 우리는 웹서버를 안배웠어요. => 웹서버라고 그냥 생각합시다. 아시겠죠 여러분? 이렇게 진행.
        string url = "file:///" + Application.dataPath + "/AssetBundles/" + _name + ".assetbd";
        UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(url);
        www.downloadHandler = new DownloadHandlerBuffer();  // 이거왜씀? -> .data에 접근해서 값을 가져오기 위해서임.
        yield return www.SendWebRequest();  // 실행

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);  // 파일 받기 실패
        }
        else
        {
            string fullPath = Application.dataPath + "/DownLoadBundle/" + _name + ".assetbd";
            byte[] file = www.downloadHandler.data; // 이런 원시적인 방법으로는 접근이 안된다고 에러뜸
            File.WriteAllBytes(fullPath, file);   // 실제 파일을 만드는 행위.

            var myLoadAssetbundle = AssetBundle.LoadFromFile(fullPath);  // 저장한 바이트배열을 에셋번들 형태로 load

            GameObject[] prefabs = myLoadAssetbundle.LoadAllAssets<GameObject>();
            //GameObject prefabs2 = myLoadAssetbundle.LoadAsset<GameObject>("cube");
            foreach (GameObject item in prefabs)
            {
                GameObject obj = Instantiate<GameObject>(item);
                obj.transform.position = Vector3.zero;
                obj.name = item.name;
            }

            myLoadAssetbundle.Unload(false);    // 에셋을 free로 만듬
            Resources.UnloadUnusedAssets();    // 사용하지 않는 에셋의 메모리 반환
        }
    }
}
