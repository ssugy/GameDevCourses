using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;



public class JsonExample_1 : MonoBehaviour
{
    List<Mob> list;

    void Start()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("HumanInfo");   // 확장자를 안씀
        JSONNode root = JSON.Parse(textAsset.text);
        Debug.Log(root["Name"].Value);
        Debug.Log(int.Parse(root["Age"].Value));
        Debug.Log(bool.Parse(root["Man"].Value));

        JSONNode JOBINFO = root["JobInfo"];
        Debug.Log(JOBINFO["class"].Value);
        Debug.Log(JOBINFO["Job"].Value);

        // Json을 List에 저장하고, List를 Json으로 저장하려면 Serialize를 해야된다.
        // list -> json으로 변환
        list = new List<Mob>();
        for (int i = 0; i < 3; i++)
        {
            Mob tmp = new Mob();
            tmp.INDEX = 100;
            tmp.NAME = "가나다" + i.ToString();
            list.Add(tmp);
        }
        string jsonData = JsonUtility.ToJson(new Serialization<Mob>(list)) ;// 여기 들어가는 매개변수는 serial된 데이터만 가능하다.(중요)
        Debug.Log(jsonData);

        // 제이슨 -> 리스트로 변환
        List<Mob> mobList = JsonUtility.FromJson<Serialization<Mob>>(jsonData).ToList();
        for (int i = 0; i < mobList.Count; i++)
        {
            Debug.Log(mobList[i].INDEX);
            Debug.Log(mobList[i].NAME);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
