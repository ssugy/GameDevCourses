using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class XmlDataExample : MonoBehaviour
{
    string fileName;
    private void Start()
    {
        fileName = "XML/xml_sample";
        LoadXml();
    }

    private void LoadXml()
    {
        // 방법1 : Resources폴더에서 텍스트 읽은 뒤, LoadXml로 Xml형식으로 다시 로드하기
        TextAsset txtAsset = Resources.Load<TextAsset>(fileName);
        XmlDocument xmlDoc = new XmlDocument();
        Debug.Log(txtAsset.text);
        xmlDoc.LoadXml(txtAsset.text);

        // 방법2 (이건 수업시간에 안함) : 절대경로로 Xml로드를 바로 함.
        XmlDocument xmlDoc2 = new XmlDocument();
        string abFilePath = Application.dataPath + "/Resources/" + fileName + ".xml";
        xmlDoc2.Load(abFilePath);
        Debug.Log(xmlDoc2.InnerXml);

        // 전체 아이템 가져오기
        XmlNodeList nodeList = xmlDoc.SelectNodes("dataroot/TestItem");
        foreach (XmlNode item in nodeList)
        {
            Debug.Log($"id : {item.SelectSingleNode("id").InnerText}");
            Debug.Log($"name : {item.SelectSingleNode("name").InnerText}");
            Debug.Log($"cost : {item.SelectSingleNode("cost").InnerText}");
        }

        XmlNode node = xmlDoc.SelectSingleNode("dataroot/DataItem");    // 이렇게 하던
        //XmlNode node = xmlDoc.SelectSingleNode("dataroot/DataItem/name");// 이렇게 하던 결과가 같은데, 그 이유가 이너텍스트는 하나밖에 없어서 이다.
        Debug.Log($"싱글노드 이너텍스트 : {node.InnerText}");
        //Debug.Log($"싱글노드 이너태그: {node.InnerXml}");
    }
}
