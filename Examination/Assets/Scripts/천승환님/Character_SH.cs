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
                    vEnd = hitInfo.point;   // ������ ��ǥ
                }
                else if(hitInfo.collider.CompareTag("Player"))
                {
                    // 3�� �̸� �Ϸ�
                    Debug.Log(hitInfo.collider.gameObject.name);
                }
            }
        }
        // �̵��ڵ� �ۼ� - ������ġ�� �ʿ���, �� �����̸� �̵����Ѿߵ�
        transform.position = Vector3.MoveTowards(transform.position, vEnd, Time.deltaTime * 5 );

        // ȸ���ڵ� �ۼ�
        // 1. ���⺤�� ���ϱ�
        // 2. Vector3.RotateToWards;
        // 3. ���ʹϾ�.������̼�
        Vector3 dir = vEnd - transform.position;    // �������� ���ϴ� ���⺤��
        Vector3 rotVec = Vector3.RotateTowards(transform.forward, dir, Time.deltaTime * 6, 0);  // ���������� �������� �߰� �ɰ� ������ ����
        transform.rotation = Quaternion.LookRotation(rotVec.normalized);    //���Ϸ�����(vec3) -> ���ʹϾ����� �����ؾߵ�
    }
}
