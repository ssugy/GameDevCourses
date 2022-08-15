using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * ��ũ��Ʈ�� 3���� ����� �߰�,
 * ��ü ������ : GameManger.cs
 * ī�޶� : CameraManager.cs
 * ĳ���� : Character.cs
 * 
 * 1. ���۽� ���ҽ��ε��Ͽ� ȭ�鿡 ��ġ ( �����̿� ������� ���� ��ġ ) - GameManager.cs (�ذ��)
 * 2. ���콺 ��ŷ(Picking)�� ���� ĳ���� �̵�/ȸ�� ���� - GameManager.cs // Character.cs
 * 3. ���콺�� ĳ���� ���ý� �ֺܼ信 ĳ���� �̸� ��� - GameManager.cs (�ذ��)
 * 4. ĳ���Ͱ� �̵��� ī�޶�� �ʱ��� �Ÿ��� ������ ä�� ĳ���͸� ���� �̵��Ѵ�. 
 * �� ,ĳ������ ȸ���� ī�޶� ������� �ʴ´�.
 */
public class GameManager_DevYH : MonoBehaviour
{
    Vector3 targetPos;
    Charactor_DevYH charactor;
    CameraManager_DevYH cameraManager;

    void Start()
    {
        // 1. ���۽� ���ҽ��ε��Ͽ� ȭ�鿡 ��ġ
        GameObject go = Resources.Load<GameObject>("TrollGiant");
        GameObject player = Instantiate(go, Vector3.zero, Quaternion.identity);

        player.AddComponent<Charactor_DevYH>();       // ĳ���� ��ũ��Ʈ�� �޾Ҵ�.
        
        // ĳ���� ��ũ��Ʈ ��������
        charactor = player.GetComponent<Charactor_DevYH>();

        // ī�޶� ī�޶� ��ũ��Ʈ ����
        cameraManager = Camera.main.gameObject.AddComponent<CameraManager_DevYH>();

    }

    void Update()
    {
        // 2. ���콺�� Ŭ���Ѵ�.0
        // ���콺 Ŭ���� ������ �浹�� üũ�Ѵ�.0
        // �� ��ġ�� �̵��Ѵ�.
        // - 1) �浹 ��ġ�� �����Ѵ�
        // - 2) �ش� ��ġ�� �̵��Ѵ�.
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity))
            {
                if (hitInfo.collider.CompareTag("Player"))
                {
                    //3. �̸����
                    Debug.Log(hitInfo.collider.name);
                }
                else
                {
                    targetPos = hitInfo.point;  // ��ǥ���� ����
                }
            }
        }

        // 2. �ɸ��� �̵�
        charactor.MoveCharactor(targetPos);
        // 2. �ɸ��� ȸ��
        charactor.RotCharactor(targetPos);

        // 4. ī�޶� �̵��� ���Ѵ�.
        cameraManager.TracePlayer(charactor.transform.position);
    }
}
