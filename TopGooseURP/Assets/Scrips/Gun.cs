using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private float shotsPerMinute = 560;
    [SerializeField] private float fireRate = .1f;
    [SerializeField] private BulletData bulletData;
    [SerializeField] private float spread = 1f;
    [SerializeField] private Transform bulletSpawn;

    private ParticleSystem muzzleFlash;
    private float fireTime;

    [SerializeField] private float heatGainPerBullet = .1f;
    [SerializeField] private float heatLossPerSec = .1f;
    private float heat;

    public float Heat { get { return Mathf.Clamp(heat, 0, 1); } }

    public bool Fire { get; set; }

    public float FireRate { get { return fireRate; } }

    // Start is called before the first frame update
    void Start()
    {
        muzzleFlash = GetComponentInChildren<ParticleSystem>();
        //fireRate = shotsPerMinute;
    }

    // Update is called once per frame
    void Update()
    {
        
        fireTime += Time.deltaTime;
        if(fireRate < fireTime && Fire && heat < 1.0f)
        {
            fireTime = 0;
            FireBullet();
            muzzleFlash.Emit(1);
            heat += heatGainPerBullet;
            if (heat > 1) heat = 2; //if shooting until full overheat -> punish
        }

        if(heat > 0)
        {
            heat -= heatLossPerSec * Time.deltaTime;
            if (heat < 0) heat = 0;
        }
    }

    void FireBullet()
    {
        float randomNumberX = Random.Range(-spread, spread);
        float randomNumberY = Random.Range(-spread, spread);
        float randomNumberZ = Random.Range(-spread, spread);
        Quaternion rotation = Quaternion.Euler(randomNumberX, randomNumberY, randomNumberZ);

        BulletManager.Instance.SpawnBullet(bulletData, bulletSpawn.position, bulletSpawn.rotation * rotation);
        //Bullet bullet = BulletManager.Instance.SpawnBullet();
        //bullet.transform.Rotate(randomNumberX, randomNumberY, randomNumberZ);
        //bullet.Init(bulletData, bulletSpawn.position, bulletSpawn.rotation * rotation);
    }
}
