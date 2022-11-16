using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class FileCopyExam_Teacher : MonoBehaviour
{
    string sourcesDir;
    string destDir;

    public Text fileNameTxt;
    public Text CountTxt;
    public Image bar;

    private void Start()
    {
        sourcesDir = "D:\\Test\\";
        destDir = "C:\\Test\\";

        StartCoroutine(CopyFileTo());
    }

    IEnumerator CopyFileTo()
    {
        // 폴더가 존재하지 않으면 만들어주고
        if (!System.IO.Directory.Exists(destDir))
        {
            System.IO.Directory.CreateDirectory(destDir);
        }
        string[] fileName = System.IO.Directory.GetFiles(sourcesDir);

        for (int i = 0; i < fileName.Length; i++)
        {
            var file = new FileInfo(fileName[i]);
            string destFileName = destDir + file.Name;

            //글자 표시,진행도 표시
            fileNameTxt.text = $"{destFileName}";
            CountTxt.text = $"{i + 1} / {fileName.Length}";
            bar.fillAmount = (float)(i + 1) / (float)fileName.Length;
            yield return null;

            File.Copy(fileName[i], destFileName);
            yield return null;
        }
    }
}
