using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WebViewManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Application.OpenURL("https://www.naver.com");
    }

    public void OnEndEdit(string str)
    {
        Debug.Log("�Է¹��� ���� : " + str);
    }

    public void OnEndEdit2(TextMeshProUGUI str2)
    {
        Debug.Log("�Է¹��� ���� : " + str2.text);
    }

    public void NEHappy()
    {
        Debug.Log("�򰥸��� ������");  // ��ư�� ������, �̰Ÿ� �����ϰ� �ʹ� �̰ſ���.
    }
}
