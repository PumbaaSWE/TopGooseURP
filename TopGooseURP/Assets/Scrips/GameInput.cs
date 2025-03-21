using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    /*
     [X] MouseMove
     [X] KeyboardMove
     [X] FireMain
     [X] FireSecondary
     [X] ChangeSpeed
     [X] Freelook
     [X] Yaw?
     [X] IngameMenu
     [X] SwitchWapon
     */
    public event EventHandler FireMainAction, FireMainCanceled, FireSecondaryAction, FireSecondaryCanceled, SwitchWeaponAction, InGameMenuAction, FreeLookStart, FreeLookCancel;
    private PlayerInputAction playerInputAction;

    private void Awake()
    {
        playerInputAction = new PlayerInputAction();
        playerInputAction.PlayerControls.Enable();

        //Mouse
        playerInputAction.PlayerControls.FireMain.started += FireMain_performed;
        playerInputAction.PlayerControls.FireMain.canceled += FireMain_canceled;
        playerInputAction.PlayerControls.FireSecondary.started += FireSecondary_performed;
        playerInputAction.PlayerControls.FireSecondary.canceled += FireSecondary_canceled;
        //Keyboard
        playerInputAction.PlayerControls.SwitchWeapon.performed += SwitchWeapon_performed;
        playerInputAction.PlayerControls.IngameMenu.performed += IngameMenu_performed;
        playerInputAction.PlayerControls.Freelook.started += Freelook_started;
        playerInputAction.PlayerControls.Freelook.canceled += Freelook_canceled;

        
    }    

    #region Methods
    public float ThrottleChangeActionNormalized()
    {
        return playerInputAction.PlayerControls.ChangeSpeed.ReadValue<Vector2>().y;
    }
    public Vector2 MouseAxis()
    {
        return playerInputAction.PlayerControls.MouseMove.ReadValue<Vector2>();
    }
    public Vector2 KeyboardMovement()
    {
        Vector2 inputVec = playerInputAction.PlayerControls.KeyboardMove.ReadValue<Vector2>();
        return inputVec;
    }
    public float YawActionNormalized()
    {
        return playerInputAction.PlayerControls.Yaw.ReadValue<Vector2>().x;
    }
    #endregion

    #region Events
    private void IngameMenu_performed(InputAction.CallbackContext obj)
    {
        InGameMenuAction?.Invoke(this, EventArgs.Empty);
    }
    private void SwitchWeapon_performed(InputAction.CallbackContext obj)
    {
        SwitchWeaponAction?.Invoke(this, EventArgs.Empty);
    }
    private void FireSecondary_performed(InputAction.CallbackContext obj)
    {
        FireSecondaryAction?.Invoke(this, EventArgs.Empty);
    }
    private void FireSecondary_canceled(InputAction.CallbackContext obj)
    {
        FireSecondaryCanceled?.Invoke(this, EventArgs.Empty);
    }
    private void FireMain_performed(InputAction.CallbackContext obj)
    {        
        FireMainAction?.Invoke(this, EventArgs.Empty);
    }
    private void FireMain_canceled(InputAction.CallbackContext obj)
    {
        FireMainCanceled?.Invoke(this, EventArgs.Empty);
    }
    private void Freelook_started(InputAction.CallbackContext obj)
    {
        FreeLookStart?.Invoke(this, EventArgs.Empty);
    }
    private void Freelook_canceled(InputAction.CallbackContext obj)
    {
        FreeLookCancel?.Invoke(this, EventArgs.Empty);
    }
    #endregion


    #region Pause
    public void GamePause()
    {
        playerInputAction.Disable();
    }

    public void GameUnPause()
    {
        playerInputAction.Enable();
    }
    #endregion
}
