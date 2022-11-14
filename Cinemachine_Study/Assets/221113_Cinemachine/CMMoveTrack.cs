using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

/*
 * 제가 하려는게 머죠?
 * CinemachineVirtualCamera 컴포넌트 안에 있는 body안에 있는 position에 접근하기.
 */
public class CMMoveTrack : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;    // 버추얼카메라 가져오기
    [SerializeField] private float cycleTime = 10.0f;
    private CinemachineTrackedDolly dolly;
    private float pathPositionMax;
    private float pathPositionMin;
    private void Start()
    {
        // Cancel if no virtual camera is set
        if (this.virtualCamera == null)
        {
            this.enabled = false;
            return;
        }
        // abort if dolly component cannot be obtained
        this.dolly = this.virtualCamera.GetCinemachineComponent<CinemachineTrackedDolly>();
        if (this.dolly == null)
        {
            this.enabled = false;
            return;
        }
        // Set the unit of Position to be based on the waypoint number on the track
        this.dolly.m_PositionUnits = CinemachinePathBase.PositionUnits.PathUnits;
        // Get the maximum and minimum number of waypoints
        this.pathPositionMax = this.dolly.m_Path.MaxPos;
        this.pathPositionMin = this.dolly.m_Path.MinPos;
    }
    private void Update()
    {
        // reciprocate on track over cycleTime seconds
        var t = 0.5f - (0.5f * Mathf.Cos((Time.time * 2.0f * Mathf.PI) / this.cycleTime));
        this.dolly.m_PathPosition = Mathf.Lerp(this.pathPositionMin, this.pathPositionMax, t);
    }
}
