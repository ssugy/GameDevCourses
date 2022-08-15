using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * 스크립트를 3개를 쓰라고 했고,
 * 전체 관리자 : GameManger.cs
 * 카메라 : CameraManager.cs
 * 캐릭터 : Character.cs
 * 
 * 1. 시작시 리소스로드하여 화면에 배치 ( 높낮이에 상관없는 몬스터 배치 ) - GameManager.cs (해결됨)
 * 2. 마우스 픽킹(Picking)에 의한 캐릭터 이동/회전 구현 - GameManager.cs // Character.cs
 * 3. 마우스로 캐릭터 선택시 콘솔뷰에 캐릭터 이름 출력 - GameManager.cs (해결됨)
 * 4. 캐릭터가 이동시 카메라는 초기의 거리를 유지한 채로 캐릭터를 따라서 이동한다. 
 * 단 ,캐릭터의 회전은 카메라에 적용되지 않는다.
 */
public class GameManager_DevYH : MonoBehaviour
{
    Vector3 targetPos;
    Charactor_DevYH charactor;
    CameraManager_DevYH cameraManager;

    void Start()
    {
        // 1. 시작시 리소스로드하여 화면에 배치
        GameObject go = Resources.Load<GameObject>("TrollGiant");
        GameObject player = Instantiate(go, Vector3.zero, Quaternion.identity);

        player.AddComponent<Charactor_DevYH>();       // 캐릭터 스크립트를 달았다.
        
        // 캐릭터 스크립트 가져오기
        charactor = player.GetComponent<Charactor_DevYH>();

        // 카메라에 카메라 스크립트 적용
        cameraManager = Camera.main.gameObject.AddComponent<CameraManager_DevYH>();

    }

    void Update()
    {
        // 2. 마우스를 클릭한다.0
        // 마우스 클릭한 지점의 충돌을 체크한다.0
        // 그 위치로 이동한다.
        // - 1) 충돌 위치를 저장한다
        // - 2) 해당 위치로 이동한다.
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity))
            {
                if (hitInfo.collider.CompareTag("Player"))
                {
                    //3. 이름출력
                    Debug.Log(hitInfo.collider.name);
                }
                else
                {
                    targetPos = hitInfo.point;  // 목표지점 설정
                }
            }
        }

        // 2. 케릭터 이동
        charactor.MoveCharactor(targetPos);
        // 2. 케릭터 회전
        charactor.RotCharactor(targetPos);

        // 4. 카메라 이동을 원한다.
        cameraManager.TracePlayer(charactor.transform.position);
    }
}
