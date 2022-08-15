using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            print("¸¶¿ì½º Æ÷Áö¼ÇÀº : " + Input.mousePosition);
            print("ScreenPointToRay ÁÂÇ¥ : " + Camera.main.ScreenPointToRay(Input.mousePosition));
            print("ScreenToViewportPoint ÁÂÇ¥ : " + Camera.main.ScreenToViewportPoint(Input.mousePosition));
            print("ScreenToWorldPoint ÁÂÇ¥ : " + Camera.main.ScreenToWorldPoint(Input.mousePosition));

        }   
    }
}
