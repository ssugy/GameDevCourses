using System;
using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;
//using LitJson;

namespace My
{
    // gree webview
    // https://github.com/gree/unity-webview
    // android 9������ AndroidManifest.xml�� android:usesCleartextTraffic="true"�� �߰� ����. 
    // AndroidManifest.xml : ������Ʈ ���� -> project/Temp/StagingArea/ ���� ���� Assets/Plugins/Android/�� ����.
    // <application android:theme="@style/UnityThemeSelector" android:icon="@mipmap/app_icon" android:label="@string/app_name" android:usesCleartextTraffic="true">

    // Ȥ�� 
    // Unity Define �Է�â��
    //    UNITYWEBVIEW_ANDROID_USES_CLEARTEXT_TRAFFIC
    // �� �Է� �ϰ� ����.

    public class MyWebView : MonoBehaviour
    {
        public Button CloseButton;

        public float LeftMargin = 300;
        public float TopMargin = 50;
        public float RightMargin = 300;
        public float BottomMargin = 50;
        public bool IsRelativeMargin = true;

        [HideInInspector]
        public string URL;

        private WebViewObject webViewObject;

        void Start()
        {
            AlignCloseButton();

            // for Test
            Show("https://www.google.com/");
        }

        // Update is called once per frame
        void Update()
        {
            //if (Application.platform == RuntimePlatform.Android) {
            if (Input.GetKey(KeyCode.Escape))
            {
                // �� �� �� ��, esc �� ư 
                if (webViewObject)
                {
                    if (webViewObject.gameObject.activeInHierarchy)
                    {
                        // webViewObject.GoBack();
                    }
                }
                Hide();
                return;
            }
            //}
        }

        private void OnDestroy()
        {
            DestroyWebView();
        }

        void DestroyWebView()
        {
            if (webViewObject)
            {
                GameObject.Destroy(webViewObject.gameObject);
                webViewObject = null;
            }
        }

        void AlignCloseButton()
        {
            if (CloseButton == null)
            {
                return;
            }

            float defaultScreenHeight = 1080;
            float top = CloseButton.GetComponent<RectTransform>().rect.height * Screen.height / defaultScreenHeight;

            TopMargin = top;
        }

        public void Show(string url)
        {
            gameObject.SetActive(true);

            URL = url;

            StartWebView();
        }

        public void Hide()
        {
            // �� �� �� ��, esc �� ư 
            URL = string.Empty;

            if (webViewObject != null)
            {
                webViewObject.SetVisibility(false);

                //webViewObject.ClearCache(true);
                //webViewObject.ClearCookies();                
            }

            DestroyWebView();

            gameObject.SetActive(false);
        }

        public void StartWebView()
        {
            string strUrl = URL;  //"https://www.naver.com/";            

            try
            {
                if (webViewObject == null)
                {
                    webViewObject = (new GameObject("WebViewObject")).AddComponent<WebViewObject>();

                    webViewObject.Init(
                        cb: OnResultWebView,
                        err: (msg) => { Debug.Log($"WebView Error : {msg}"); },
                        httpErr: (msg) => { Debug.Log($"WebView HttpError : {msg}"); },
                        started: (msg) => { Debug.Log($"WebView Started : {msg}"); },
                        hooked: (msg) => { Debug.Log($"WebView Hooked : {msg}"); },

                        ld: (msg) =>
                        {
                            Debug.Log($"WebView Loaded : {msg}");
                            //webViewObject.EvaluateJS(@"Unity.call('ua=' + navigator.userAgent)");                    
                        }
                        , androidForceDarkMode: 1  // 0: follow system setting, 1: force dark off, 2: force dark on

#if UNITY_EDITOR
                        , separated: true
#endif

                    );
                }

                webViewObject.LoadURL(strUrl);
                webViewObject.SetVisibility(true);
                webViewObject.SetMargins((int)LeftMargin, (int)TopMargin, (int)RightMargin, (int)BottomMargin, IsRelativeMargin);
            }
            catch (System.Exception e)
            {
                print($"WebView Error : {e}");
            }

        }

        void OnResultWebView(string resultData)
        {
            print($"WebView CallFromJS : {resultData}");
            /*
             {
                result : string,
                data : string or json string
            }
            */

            //try
            //{
            //    JsonData json = JsonMapper.ToObject(resultData);

            //    if ((string)json["result"] == "success")
            //    {
            //        JsonData data = json["data"]["response"];
            //        long birthdayTick = (long)(data["birth"].IsLong ? (long)data["birth"] : (int)data["birth"]);
            //        string birthday = (string)data["birthday"];
            //        string unique_key = (string)data["unique_key"];

            //        // success
            //    }
            //    else if ((string)json["result"] == "failed")
            //    {
            //        Hide();

            //        // failed
            //    }
            //}
            //catch (Exception e)
            //{
            //    print("�� ���� ���� ������ �ֽ��ϴ�.");
            //}
        }
    }
}