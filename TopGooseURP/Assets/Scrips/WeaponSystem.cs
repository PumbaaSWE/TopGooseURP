using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    [SerializeField] private List<Weapon> weapons;
    [SerializeField] private Weapon primary;
    [SerializeField] private int secondarySlots = 2;
    private int currentWeapon = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        primary.Activate();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            CycleNext();
        }

        if (Input.GetMouseButton(0))
        {
            primary.Deploy();
        }
        if (Input.GetMouseButton(1))
        {
            weapons[currentWeapon].Deploy();
        }
    }

    public void AddWeapon(Weapon weapon)
    {
        if(weapons.Count < secondarySlots)
        {
            weapons.Add(weapon);
        }
        else
        {
            weapons[currentWeapon].Deactivate();
            weapons[currentWeapon] = weapon;
        }
    }

    public void CycleNext()
    {
        Cycle(++currentWeapon);
    }
    public void CyclePrevius()
    {
        Cycle(--currentWeapon);
    }

    public void Cycle(int i)
    {
        weapons[currentWeapon].Deactivate();
        currentWeapon = i % weapons.Count;
        weapons[currentWeapon].Activate();
    }

    public void Deploy()
    {
        weapons[currentWeapon].Deploy();
    }
}
