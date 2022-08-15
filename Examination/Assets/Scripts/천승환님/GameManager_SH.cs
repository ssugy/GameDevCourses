using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * 1. ���۽� ���ҽ��ε��Ͽ� ȭ�鿡 ��ġ ( �����̿� ������� ���� ��ġ ) - GameManager.cs
 * 2. ���콺 ��ŷ(Picking)�� ���� ĳ���� �̵�/ȸ�� ���� - GameManager.cs // Character.cs
 * 3. ���콺�� ĳ���� ���ý� �ֺܼ信 ĳ���� �̸� ��� - GameManager.cs
 * 4. ĳ���Ͱ� �̵��� ī�޶�� �ʱ��� �Ÿ��� ������ ä�� ĳ���͸� ���� �̵��Ѵ�. �� ,ĳ������ ȸ���� ī�޶� ������� �ʴ´�.
 */
public class GameManager_SH : MonoBehaviour
{
    GameObject rcCha;
    GameObject character;

    Character_SH test;
    void Awake()
    {
        // 1. ���� �� ���ҽ��ε��Ͽ� ȭ�鿡 ��ġ(�Ϸ�)
        rcCha = Resources.Load<GameObject>("Character/TrollGiant");
        character = GameObject.Instantiate<GameObject>(rcCha);


        // ������ ���ӿ������� ��ũ��Ʈ�� ���α׷� �ڵ�󿡼� �߰�
        //character.AddComponent<Character_SH>(); // 1
        //test = character.GetComponent<Character_SH>(); //2

        Character_SH chaScript = character.AddComponent<Character_SH>();

        // 4. ����ī�޶�, ��ũ��Ʈ �߰�
        // ī�޶���Ʈ�ѷ��� player�� character�� ����������Ѵ�.
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
