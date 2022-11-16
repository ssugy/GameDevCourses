using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;

/**
 * ���� ������� ����ؼ�
 * D:/Test/�ִ� ������ E:/Test�� ����
 */
public class SystemIO : MonoBehaviour
{
    public Text fileNameTxt;
    public Text fileProgressTxt;
    public Image progressBar;

    private float progress;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartCoroutine(MoveAllFiles(StartURI, EndURI));
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            StartCoroutine(MoveAllFiles(EndURI, StartURI));
        }
    }

    string StartURI = "D://Test/";
    string EndURI = "C://Test/";

    private IEnumerator MoveAllFiles(string StartURI, String DestURI)
    {
        System.IO.DirectoryInfo di = new DirectoryInfo(StartURI);
        FileInfo[] fileList = di.GetFiles();
        for (int i = 0; i < fileList.Length; i++)
        {
            fileList[i].MoveTo(DestURI + fileList[i].Name);
            
            fileNameTxt.text = $"���ϸ� : {fileList[i].Name}";
            fileProgressTxt.text = $"({(i + 1)}/{fileList.Length})";
            progress = (i+1.0f) / fileList.Length;
            Debug.Log(progress);
            progressBar.fillAmount = progress;
            yield return new WaitForSeconds(1);
        }
        
    }
}
