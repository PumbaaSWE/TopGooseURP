using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatMissileLauncher : MonoBehaviour
{
    [SerializeField]
    GameObject boatMissile;

    public void Fire(Transform target)
    {
        GameObject missile = Instantiate(boatMissile);

        //Change later
        missile.transform.position = transform.position + Vector3.up * 5;
        missile.GetComponent<BoatMissile>().SetTarget(target);
        missile.SetActive(true);
    }
}
