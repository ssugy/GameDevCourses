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
            print("���콺 �������� : " + Input.mousePosition);
            print("ScreenPointToRay ��ǥ : " + Camera.main.ScreenPointToRay(Input.mousePosition));
            print("ScreenToViewportPoint ��ǥ : " + Camera.main.ScreenToViewportPoint(Input.mousePosition));
            print("ScreenToWorldPoint ��ǥ : " + Camera.main.ScreenToWorldPoint(Input.mousePosition));

        }   
    }
}
