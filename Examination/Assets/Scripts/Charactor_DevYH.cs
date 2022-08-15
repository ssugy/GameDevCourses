using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charactor_DevYH : MonoBehaviour
{
    // �̵� + ȸ��
    private float moveSpeed = 10;
    private float rotSpeed = 5;

    //�̵��Լ�
    public void MoveCharactor(Vector3 destPos)
    {
        //�̵��ϴ� �۾� - MoveTowards ����.
        transform.position = Vector3.MoveTowards(transform.position
                                                    , destPos
                                                    , Time.deltaTime * moveSpeed);
    }

    // ȸ������
    public void RotCharactor(Vector3 vEnd)
    {
        // ���⺤�͸� ���Ѵ�
        // RotateTowards �޼��带 ����ϰ�
        // ȸ������ ���� ����.
        Vector3 dir = vEnd - transform.position;    //���⺤��
        Vector3 newDir = Vector3.RotateTowards(transform.forward    //���� �ٶ󺸴� ����
                                            , dir   // ���� ����
                                            , Time.deltaTime * rotSpeed // ȸ���ӵ�
                                            , 0);
        transform.rotation = Quaternion.LookRotation(newDir);
    }
}
