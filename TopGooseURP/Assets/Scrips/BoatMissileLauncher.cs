using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatMissileLauncher : MonoBehaviour
{
    [SerializeField]
    GameObject boatMissile;

    [SerializeField]
    Transform hardPoint;

    public void Fire(Transform target)
    {
        GameObject missile = Instantiate(boatMissile);

        //Change later
        missile.transform.position = hardPoint.position;
        missile.GetComponent<BoatMissile>().SetTarget(target);
        missile.SetActive(true);
    }
}
