using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{

    public event EventHandler FireMainAction, FireSecondaryAction, SwitchWeaponAction, InGameMenuAction;
    private PlayerInputAction playerInputAction;

    private void Awake()
    {
        playerInputAction = new PlayerInputAction();
        playerInputAction.PlayerControls.Enable();

        //Mouse
        playerInputAction.PlayerControls.FireMain.performed += FireMain_performed;
        playerInputAction.PlayerControls.FireSecondary.performed += FireSecondary_performed;

        //Keyboard
        playerInputAction.PlayerControls.SwitchWeapon.performed += SwitchWeapon_performed;
        playerInputAction.PlayerControls.IngameMenu.performed += IngameMenu_performed;
        playerInputAction.PlayerControls.Freelook.performed += Freelook_performed;
        
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
    private void FireMain_performed(InputAction.CallbackContext obj)
    {        
        FireMainAction?.Invoke(this, EventArgs.Empty);
    }
    private void Freelook_performed(InputAction.CallbackContext obj)
    {
        throw new NotImplementedException();
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
