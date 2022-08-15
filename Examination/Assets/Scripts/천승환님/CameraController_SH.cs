using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 카메라가 플레이어 쫓아가기
public class CameraController_SH : MonoBehaviour
{
    private GameObject player;
    private Vector3 offsetVec = new Vector3(0, 2, -10);

    public GameObject PLAYER
    {
        set { player = value; }
    }

    private void Update()
    {
        MoveCamera();
    }

    public void MoveCamera()
    {
        transform.position = player.transform.position + offsetVec;
    }
}
