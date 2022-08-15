using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager_DevYH : MonoBehaviour
{
    public void TracePlayer(Vector3 playerPos)
    {
        transform.position = playerPos + new Vector3(0, 2, -10);
    }
}
