using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * 1. 시작시 리소스로드하여 화면에 배치 ( 높낮이에 상관없는 몬스터 배치 ) - GameManager.cs
 * 2. 마우스 픽킹(Picking)에 의한 캐릭터 이동/회전 구현 - GameManager.cs // Character.cs
 * 3. 마우스로 캐릭터 선택시 콘솔뷰에 캐릭터 이름 출력 - GameManager.cs
 * 4. 캐릭터가 이동시 카메라는 초기의 거리를 유지한 채로 캐릭터를 따라서 이동한다. 단 ,캐릭터의 회전은 카메라에 적용되지 않는다.
 */
public class GameManager_SH : MonoBehaviour
{
    GameObject rcCha;
    GameObject character;

    Character_SH test;
    void Awake()
    {
        // 1. 시작 시 리소스로드하여 화면에 배치(완료)
        rcCha = Resources.Load<GameObject>("Character/TrollGiant");
        character = GameObject.Instantiate<GameObject>(rcCha);


        // 생성한 게임오브젝에 스크립트를 프로그램 코드상에서 추가
        //character.AddComponent<Character_SH>(); // 1
        //test = character.GetComponent<Character_SH>(); //2

        Character_SH chaScript = character.AddComponent<Character_SH>();

        // 4. 메인카메라에, 스크립트 추가
        // 카메라컨트롤러의 player와 character를 연결해줘야한다.
        CameraController_SH cameraController = Camera.main.gameObject.AddComponent<CameraController_SH>();
        cameraController.PLAYER = character;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    int TestFunc(string str, float f1)
    {
        return 0;
    }
}
