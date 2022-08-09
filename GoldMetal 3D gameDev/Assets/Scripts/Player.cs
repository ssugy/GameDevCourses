using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    float hAxis;
    float vAxis;

    Vector3 moveVec;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    
    void Update()
    {
        hAxis = Input.GetAxisRaw("Horizontal");    // GetAxis는 -1 ~ 1사이 부드러운 이동
        vAxis = Input.GetAxisRaw("Vertical");   // GetAxisRaw  -1 0 1 

        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        transform.position += moveVec * speed * Time.deltaTime;
    }
}
