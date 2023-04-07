using System;
using UnityEngine;

public class WeaponSystem : MonoBehaviour
{

    public ManualFlightInput flightInput;

//    public GunInput GunInput;
    public Gun[] guns;
    public MissileLauncher MissileLauncher;
    public BombBay BombBay; // THIS IS DR BOMBAY!!

    //delegate for events to the UI
    public delegate void BombSelected(bool selected);
    public BombSelected OnBombSelected;

    private bool bombs; //the switch :P
    
    void Start()
    {
        guns = GetComponentsInChildren<Gun>();
        if (guns == null)
        {
            Debug.LogWarning("WeaponSystem - Missing Gun script on this game object children!");
        }
        if(!TryGetComponent(out MissileLauncher))
        {
            Debug.LogWarning("WeaponSystem - Missing MissileLauncher script on this game object!");
        }
        if (!TryGetComponent(out BombBay))
        {
            Debug.LogWarning("WeaponSystem - Missing BombBay script on this game object!");
        }
        OnSecondarySwitch();
    }

    void Update()
    {
        //FOR TESTING!!!! *****REMOVE*****
        if (Input.GetKeyDown(KeyCode.F))
        {
            OnSecondarySwitch();
        }

        if (Input.GetMouseButtonDown(0))
        {
            OnPrimaryFire();
        }
        if (Input.GetMouseButtonUp(0))
        {
            OnPrimaryStop();
        }

        if (Input.GetMouseButtonDown(1))
        {
            OnSecondaryFire();
        }
        if (Input.GetMouseButtonUp(1))
        {
            OnSecondaryStop();
        }
        //*****END OF REMOVE******




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
        if(MissileLauncher.NoMissile) return;
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
