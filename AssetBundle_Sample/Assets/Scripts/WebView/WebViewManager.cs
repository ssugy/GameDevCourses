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
        Debug.Log("입력받은 글자 : " + str);
    }

    public void OnEndEdit2(TextMeshProUGUI str2)
    {
        Debug.Log("입력받은 글자 : " + str2.text);
    }

    public void NEHappy()
    {
        Debug.Log("헷갈리는 나은님");  // 버튼을 누르면, 이거를 실행하고 싶다 이거에요.
    }
}
