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

    /// <summary>
    /// Returns the heat of this gun, between 0 and 1 where 1 is overheated and will not fire
    /// </summary>
    public float Heat { get { return Mathf.Clamp(heat, 0, 1); } }

    /// <summary>
    /// Set to true to begin shooying, false to stop
    /// </summary>
    public bool Fire { get; set; }

    /// <summary>
    /// Returns the bullet speed of this gun in m/s
    /// </summary>
    public float BulletSpeed => bulletData.speed;

    public float FireRate { get { return fireRate; } }

    public AudioClip[] pews;
    public AudioSource source;

    private TeamMember owner;
    // Start is called before the first frame update
    void Start()
    {
        muzzleFlash = GetComponentInChildren<ParticleSystem>();
        muzzleFlash.Stop();
        fireRate = 1 / (shotsPerMinute / 60);
        source = GetComponent<AudioSource>();
        owner = GetComponentInParent<TeamMember>();
    }

    // Update is called once per frame
    void Update()
    {
        
        fireTime += Time.deltaTime;
        if(fireRate < fireTime && Fire && heat < 1.0f)
        {
            fireTime = 0;
            FireBullet();
            if(pews.Length > 0)source.PlayOneShot(pews[Random.Range(0, pews.Length)]);
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

    /// <summary>
    /// Spawns a bullet and computes a random spread (based on a cube, not very elegant...)
    /// </summary>
    void FireBullet()
    {
        float randomNumberX = Random.Range(-spread, spread);
        float randomNumberY = Random.Range(-spread, spread);
        float randomNumberZ = Random.Range(-spread, spread);
        Quaternion rotation = Quaternion.Euler(randomNumberX, randomNumberY, randomNumberZ);

        BulletManager.Instance.SpawnBullet(bulletData, owner, bulletSpawn.position, bulletSpawn.rotation * rotation);
        //Bullet bullet = BulletManager.Instance.SpawnBullet();
        //bullet.transform.Rotate(randomNumberX, randomNumberY, randomNumberZ);
        //bullet.Init(bulletData, bulletSpawn.position, bulletSpawn.rotation * rotation);
    }


    /// <summary>
    /// Change the direction this gun is aiming at without changing the models direction...
    /// </summary>
    /// <param name="aimPoint"></param>
    public void ZeroAt(Vector3 aimPoint)
    {
        bulletSpawn.LookAt(aimPoint); //world space? that will fail...
    }
}
