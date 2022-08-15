using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_SH : MonoBehaviour
{
    Vector3 vEnd;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if(Physics.Raycast(ray, out hitInfo, Mathf.Infinity))
            {
                if(hitInfo.collider.CompareTag("Terrain"))
                {
                    vEnd = hitInfo.point;   // 목적지 좌표
                }
                else if(hitInfo.collider.CompareTag("Player"))
                {
                    // 3번 이름 완료
                    Debug.Log(hitInfo.collider.gameObject.name);
                }
            }
        }
        // 이동코드 작성 - 목적위치가 필요함, 내 몸뚱이를 이동시켜야됨
        transform.position = Vector3.MoveTowards(transform.position, vEnd, Time.deltaTime * 5 );

        // 회전코드 작성
        // 1. 방향벡터 구하기
        // 2. Vector3.RotateToWards;
        // 3. 쿼터니언.룩로테이션
        Vector3 dir = vEnd - transform.position;    // 목적지로 향하는 방향벡터
        Vector3 rotVec = Vector3.RotateTowards(transform.forward, dir, Time.deltaTime * 6, 0);  // 목적지까지 가기위해 잘게 쪼갠 각각의 방향
        transform.rotation = Quaternion.LookRotation(rotVec.normalized);    //오일러각도(vec3) -> 쿼터니언으로 변경해야됨
    }
}
