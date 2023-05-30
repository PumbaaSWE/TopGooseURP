using UnityEngine;

public class BoatMissileLauncher : MonoBehaviour
{
    [SerializeField]
    GameObject boatMissile;

    [SerializeField]
    Transform hardPoint;

    ParticleSystem particles;

    private void Start()
    {
        particles = hardPoint.GetComponent<ParticleSystem>();
    }

    public void Fire(Transform target)
    {
        GameObject missile = Instantiate(boatMissile);

        particles.Play();
        missile.transform.position = hardPoint.position;
        missile.GetComponent<BoatMissile>().SetTarget(target);
        missile.SetActive(true);
    }
}
