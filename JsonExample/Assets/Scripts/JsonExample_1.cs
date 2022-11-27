using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;



public class JsonExample_1 : MonoBehaviour
{
    List<Mob> list;

    void Start()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("HumanInfo");   // Ȯ���ڸ� �Ⱦ�
        JSONNode root = JSON.Parse(textAsset.text);
        Debug.Log(root["Name"].Value);
        Debug.Log(int.Parse(root["Age"].Value));
        Debug.Log(bool.Parse(root["Man"].Value));

        JSONNode JOBINFO = root["JobInfo"];
        Debug.Log(JOBINFO["class"].Value);
        Debug.Log(JOBINFO["Job"].Value);

        // Json�� List�� �����ϰ�, List�� Json���� �����Ϸ��� Serialize�� �ؾߵȴ�.
        // list -> json���� ��ȯ
        list = new List<Mob>();
        for (int i = 0; i < 3; i++)
        {
            Mob tmp = new Mob();
            tmp.INDEX = 100;
            tmp.NAME = "������" + i.ToString();
            list.Add(tmp);
        }
        string jsonData = JsonUtility.ToJson(new Serialization<Mob>(list)) ;// ���� ���� �Ű������� serial�� �����͸� �����ϴ�.(�߿�)
        Debug.Log(jsonData);

        // ���̽� -> ����Ʈ�� ��ȯ
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
