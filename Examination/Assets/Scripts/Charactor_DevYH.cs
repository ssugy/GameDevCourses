using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charactor_DevYH : MonoBehaviour
{
    // 이동 + 회전
    private float moveSpeed = 10;
    private float rotSpeed = 5;

    //이동함수
    public void MoveCharactor(Vector3 destPos)
    {
        //이동하는 작업 - MoveTowards 저는.
        transform.position = Vector3.MoveTowards(transform.position
                                                    , destPos
                                                    , Time.deltaTime * moveSpeed);
    }

    // 회전구현
    public void RotCharactor(Vector3 vEnd)
    {
        // 방향벡터를 구한다
        // RotateTowards 메서드를 사용하고
        // 회전값에 직접 대입.
        Vector3 dir = vEnd - transform.position;    //방향벡터
        Vector3 newDir = Vector3.RotateTowards(transform.forward    //현재 바라보는 방향
                                            , dir   // 목적 방향
                                            , Time.deltaTime * rotSpeed // 회전속도
                                            , 0);
        transform.rotation = Quaternion.LookRotation(newDir);
    }
}
