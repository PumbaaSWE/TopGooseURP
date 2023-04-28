using System;
using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    [SerializeField] private GameInput gameInput;

    public ManualFlightInput flightInput;

    //    public GunInput GunInput;
    public Gun[] guns;
    public MissileLauncher MissileLauncher;
    public BombBay BombBay; // THIS IS DR BOMBAY!!

    //delegate for events to the UI
    public delegate void BombSelected(bool selected);
    public BombSelected OnBombSelected;

    private bool bombs; //the switch :P'
    private bool fireMain;
    private bool fireSecondary;

    void Start()
    {
        guns = GetComponentsInChildren<Gun>();
        if (guns == null)
        {
            Debug.LogWarning("WeaponSystem - Missing Gun script on this game object children!");
        }
        if (!TryGetComponent(out MissileLauncher))
        {
            Debug.LogWarning("WeaponSystem - Missing MissileLauncher script on this game object!");
        }
        if (!TryGetComponent(out BombBay))
        {
            Debug.LogWarning("WeaponSystem - Missing BombBay script on this game object!");
        }
        OnSecondarySwitch();

        gameInput.FireMainAction += GameInput_FireMainAction;
        gameInput.FireSecondaryAction += GameInput_FireSecondaryAction;
        gameInput.SwitchWeaponAction += GameInput_SwitchWeaponAction;
    }

    private void GameInput_SwitchWeaponAction(object sender, EventArgs e)
    {
        OnSecondarySwitch();
    }

    private void GameInput_FireSecondaryAction(object sender, EventArgs e)
    {
        if (bombs)
        {
            fireSecondary = !fireSecondary;
            BombBay.DropBombs(fireSecondary);
        }
        else
        {
            MissileLauncher.LaunchMissile();
        }
    }

    private void GameInput_FireMainAction(object sender, EventArgs e)
    {
        fireMain = !fireMain;
        FireAllGuns(fireMain);
    }

    void Update()
    {
        if (bombs)
        {
            //HandleBombBay();
        }
        else
        {
            HandleMissileLauncher();
        }
    }

    private void HandleMissileLauncher()
    {
        if (!MissileLauncher.HasMissile) return;
        MissileLauncher.SetCageDirection(flightInput.MouseAimDirection);
    }

    public void OnPrimaryFire()
    {
        FireAllGuns(true);
    }
    public void OnPrimaryStop()
    {
        FireAllGuns(false);
    }
    public void OnSecondaryFire()
    {
        if (bombs)
        {
            BombBay.DropBombs(true);
        }
        else
        {
            MissileLauncher.LaunchMissile();
        }
    }
    public void OnSecondaryStop()
    {
        if (bombs)
        {
            BombBay.DropBombs(false);
        }
    }
    public void OnSecondarySwitch()
    {
        bombs = !bombs;
        //We dont diable the scrpts/components as we want recharging/reloading to keep going!
        BombBay.Activate(bombs);
        MissileLauncher.Activate(!bombs);
        OnBombSelected?.Invoke(bombs);
    }

    #region GUNS
    public void FireAllGuns(bool fire)
    {
        for (int i = 0; i < guns.Length; i++)
        {
            guns[i].Fire = fire;
        }
    }
    public void ZeroGunsAtPoint(Vector3 aimPoint)
    {
        for (int i = 0; i < guns.Length; i++)
        {
            guns[i].ZeroAt(aimPoint);
        }
    }
    public void ZeroGunsAtRange(float range)
    {
        ZeroGunsAtPoint(transform.position + transform.forward * range);
    }
    #endregion
}
