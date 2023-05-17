using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatMissileLauncher : MonoBehaviour
{
    [SerializeField]
    GameObject boatMissile;

    [SerializeField]
    Transform hardPoint;

    ParticleSystem particleSystem;

    private void Start()
    {
        particleSystem = hardPoint.GetComponent<ParticleSystem>();
    }

    public void Fire(Transform target)
    {
        GameObject missile = Instantiate(boatMissile);

        particleSystem.Play();
        missile.transform.position = hardPoint.position;
        missile.GetComponent<BoatMissile>().SetTarget(target);
        missile.SetActive(true);
    }
}
