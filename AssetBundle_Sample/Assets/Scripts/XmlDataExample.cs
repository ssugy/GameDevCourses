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
        // ���1 : Resources�������� �ؽ�Ʈ ���� ��, LoadXml�� Xml�������� �ٽ� �ε��ϱ�
        TextAsset txtAsset = Resources.Load<TextAsset>(fileName);
        XmlDocument xmlDoc = new XmlDocument();
        Debug.Log(txtAsset.text);
        xmlDoc.LoadXml(txtAsset.text);

        // ���2 (�̰� �����ð��� ����) : �����η� Xml�ε带 �ٷ� ��.
        XmlDocument xmlDoc2 = new XmlDocument();
        string abFilePath = Application.dataPath + "/Resources/" + fileName + ".xml";
        xmlDoc2.Load(abFilePath);
        Debug.Log(xmlDoc2.InnerXml);

        // ��ü ������ ��������
        XmlNodeList nodeList = xmlDoc.SelectNodes("dataroot/TestItem");
        foreach (XmlNode item in nodeList)
        {
            Debug.Log($"id : {item.SelectSingleNode("id").InnerText}");
            Debug.Log($"name : {item.SelectSingleNode("name").InnerText}");
            Debug.Log($"cost : {item.SelectSingleNode("cost").InnerText}");
        }

        XmlNode node = xmlDoc.SelectSingleNode("dataroot/DataItem");    // �̷��� �ϴ�
        //XmlNode node = xmlDoc.SelectSingleNode("dataroot/DataItem/name");// �̷��� �ϴ� ����� ������, �� ������ �̳��ؽ�Ʈ�� �ϳ��ۿ� ��� �̴�.
        Debug.Log($"�̱۳�� �̳��ؽ�Ʈ : {node.InnerText}");
        //Debug.Log($"�̱۳�� �̳��±�: {node.InnerXml}");
    }
}
