using System;

using QFramework;
using UnityEngine;
using UnityEngine.InputSystem;



public class InputSystem : AbstractSystem
{
    private MainControls mInputActions;

    protected override void OnInit()
    {
        mInputActions = new MainControls();
        mInputActions.MainActionMap.MoveView.performed += StartMoveView;
        mInputActions.MainActionMap.MoveView.canceled += StopMoveView;

        mInputActions.MainActionMap.RotateViewClockwise.performed += RotateViewClockwise;
        mInputActions.MainActionMap.RotateViewCounterclockwise.performed += RotateViewCounterclockwise;

        mInputActions.MainActionMap.Zoom.performed += OnZoom;
        
        mInputActions.MainActionMap.Enable();
    }

    private void OnZoom(InputAction.CallbackContext context)
    {
        // 1. 读取 Vector2 值
        Vector2 scrollValue = context.ReadValue<Vector2>();

        // 2. 我们只关心垂直滚动的 y 轴
        float scrollY = scrollValue.y;

        // 3. 发送带有滚动量的事件
        //    不需要在这里判断 > 0 还是 < 0
        //    让表现层的 ViewController 自己去判断
        if (scrollY != 0)
        {
            this.SendEvent(new OnViewZoom() { ZoomAmount = scrollY });
        }
    }

    private void StopMoveView(InputAction.CallbackContext context)
    {
        this.SendEvent<OnStopMoveView>();
    }

    private void RotateViewCounterclockwise(InputAction.CallbackContext context)
    {
        this.SendEvent<OnRotateViewCounterclockwise>();
    }

    private void RotateViewClockwise(InputAction.CallbackContext context)
    {
        this.SendEvent<OnRotateViewClockwise>();
    }

    private void StartMoveView(InputAction.CallbackContext context)
    {
        this.SendEvent<OnStartMoveView>();
    }

    protected override void OnDeinit()
    {
        mInputActions.MainActionMap.MoveView.performed -= StartMoveView;
        mInputActions.MainActionMap.MoveView.canceled -= StopMoveView;
        mInputActions.MainActionMap.RotateViewClockwise.performed -= RotateViewClockwise;
        mInputActions.MainActionMap.RotateViewCounterclockwise.performed -= RotateViewCounterclockwise;
        mInputActions.MainActionMap.Zoom.performed -= OnZoom;
        
        mInputActions.MainActionMap.Disable();
        mInputActions.Dispose();
    }
}