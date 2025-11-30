using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

public class CameraViewController : MonoBehaviour, IController
{
    [Header("Pan Settings (平移)")]
    // 【修改1】添加平移平滑时间
    [Tooltip("平移平滑时间 (越小越快)")]
    public float panSmoothTime = 0.1f;
    [Tooltip("平移灵敏度")]
    public float panSensitivity = 0.03f;

    // 【修改2】修正 min/max 语义。Min 应该是较小的值 (-30)，Max 是较大的值 (-5)
    [Tooltip("X轴最小位置")]
    [SerializeField] private float minPanX = -30f; 
    [Tooltip("X轴最大位置")]
    [SerializeField] private float maxPanX = -5f;
    [Tooltip("Y轴最小位置")]
    [SerializeField] private float minPanY = -30f;
    [Tooltip("Y轴最大位置")]
    [SerializeField] private float maxPanY = -5f;

    [Header("Zoom Settings (缩放)")]
    [Tooltip("缩放灵敏度")]
    public float zoomSensitivity = 0.01f;
    [Tooltip("缩放平滑时间 (越小越快)")]
    public float zoomSmoothTime = 0.15f;
    [Tooltip("最近Z轴位置")]
    [SerializeField] private float minZoomZ = -5f;
    [Tooltip("最远Z轴位置")]
    [SerializeField] private float maxZoomZ = -30f;

    [Header("Rotation Settings (旋转)")]
    [Tooltip("旋转动画的持续时间(秒)")]
    public float rotationDuration = 0.25f;

    [Header("References")]
    [SerializeField] private GameObject pCam;
    [Tooltip("相机")]
    [SerializeField] private Camera mCamera;

    private Vector3 m_TargetLocalPosition;
    private Vector3 m_ZoomVelocity = Vector3.zero;
    
    // 【修改3】平移(Pan)的目标位置和速度
    private Vector3 m_TargetRigPosition; // Rig(pCam)的目标位置
    private Vector3 m_PanVelocity = Vector3.zero; // Pan的SmoothDamp速度

    private Vector3 mLastMousePosition;

    private bool isMouse3Hold = false;
    private bool isRotating = false;

    public IArchitecture GetArchitecture()
    {
        return ShennongAlmanac.Interface;
    }

    void Start()
    {
        if (mCamera == null)
        {
            mCamera = pCam.GetComponentInChildren<Camera>();
        }
        
        // 【修改4】初始化 Rig 和 Camera 的目标位置
        m_TargetRigPosition = pCam.transform.position;
        m_TargetLocalPosition = mCamera.transform.localPosition;

        this.RegisterEvent<OnStartMoveView>(e =>
        {
            isMouse3Hold = true;
            mLastMousePosition = Input.mousePosition;
            
            // 【修改5】开始拖拽时，立即停止任何惯性
            m_TargetRigPosition = pCam.transform.position; 
            m_PanVelocity = Vector3.zero;

        }).UnRegisterWhenGameObjectDestroyed(gameObject);

        this.RegisterEvent<OnStopMoveView>(e =>
        {
            isMouse3Hold = false;
        }).UnRegisterWhenGameObjectDestroyed(gameObject);

        this.RegisterEvent<OnRotateViewCounterclockwise>(e => RotateViewCounterclockwise())
            .UnRegisterWhenGameObjectDestroyed(gameObject);

        this.RegisterEvent<OnRotateViewClockwise>(e => RotateViewClockwise())
            .UnRegisterWhenGameObjectDestroyed(gameObject);

        this.RegisterEvent<OnViewZoom>(e => OnCameraZoom(e.ZoomAmount))
            .UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    // OnCameraZoom 逻辑是正确的 (不变)
    private void OnCameraZoom(float zoomAmount)
    {
        float move = zoomAmount * zoomSensitivity;
        m_TargetLocalPosition.z += move;
        m_TargetLocalPosition.z = Mathf.Clamp(m_TargetLocalPosition.z, maxZoomZ, minZoomZ);
    }

    void Update()
    {
        // 1. (计算) 如果正在拖拽，更新平移目标
        if (isMouse3Hold && !isRotating)
        {
            MoveView();
        }
        SmoothMove();

        CameraZoom();
    }

    private void SmoothMove()
    {
        if(pCam.transform.position != m_TargetRigPosition)
        {           
            pCam.transform.position = Vector3.SmoothDamp(
                        pCam.transform.position,
                        m_TargetRigPosition,
                        ref m_PanVelocity,
                        panSmoothTime
                    );
        }
    }

    private void MoveView()
    {
        Vector3 currentMousePosition = Input.mousePosition;
        Vector3 mousePixelDelta = currentMousePosition - mLastMousePosition;

        Vector3 worldMoveDelta = (pCam.transform.right * -mousePixelDelta.x +
                                  pCam.transform.up * -mousePixelDelta.y) * panSensitivity;

        m_TargetRigPosition += worldMoveDelta;

        m_TargetRigPosition.x = Mathf.Clamp(m_TargetRigPosition.x, minPanX, maxPanX);
        m_TargetRigPosition.y = Mathf.Clamp(m_TargetRigPosition.y, minPanY, maxPanY);

        mLastMousePosition = currentMousePosition;       
    }

    
    // CameraZoom 逻辑是正确的 (不变)
    private void CameraZoom()
    {
        mCamera.transform.localPosition = Vector3.SmoothDamp(
            mCamera.transform.localPosition,
            m_TargetLocalPosition,
            ref m_ZoomVelocity,
            zoomSmoothTime
        );
    }

    // 旋转逻辑是正确的 (不变)
    private void RotateViewClockwise()
    {
        if (!isRotating)
            StartCoroutine(RotateAround(45, rotationDuration));
    }

    private void RotateViewCounterclockwise()
    {
        if (!isRotating)
            StartCoroutine(RotateAround(-45, rotationDuration));
    }

    IEnumerator RotateAround(float angle, float duration)
    {
        isRotating = true;
        Quaternion startRotation = pCam.transform.rotation;
        Quaternion targetRotation = pCam.transform.rotation * Quaternion.Euler(0, 0, angle);
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            t = t * t * (3f - 2f * t);

            pCam.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        pCam.transform.rotation = targetRotation;
        isRotating = false;
    }
}