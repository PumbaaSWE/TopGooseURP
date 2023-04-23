using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;
using Random = UnityEngine.Random;

public class BombBay : MonoBehaviour
{
    [SerializeField] private Bomb bombPrefab;
    [SerializeField] private float spread = .1f;
    [SerializeField] private int magazineSize = 10;
    [SerializeField] private float  reloadTime = 5.5f;
    [SerializeField] private float dropRate = 0.2f;

    public int MagazineSize { get { return magazineSize; } set { magazineSize = value; } }
    public int Magazine { get; private set; }
    public float ReloadTimer { get; private set; }
    public float ReloadTime { get { return reloadTime; } set { reloadTime = value; } }

    public delegate void OnActivated(bool selected);
    public OnActivated OnActivationChange;

    private float bombTime;
    private bool drop = false;
    private Rigidbody rb;

    private TeamMember owner;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        owner = GetComponentInParent<TeamMember>();
    }

    void Update()
    {
        float dt = Time.deltaTime;
        bombTime += dt;
        if (Magazine <= 0)
        {
            HandleReload(dt);
        }
        else if(drop && bombTime >= dropRate)
        {  
            DropBomb();
        }
  
    }

    private void HandleReload(float dt)
    {
        ReloadTimer += dt;
        if (ReloadTimer >= reloadTime)
        {
            ReloadTimer = 0;
            Magazine = magazineSize;
        }
    }

    public void DropBombs(bool drop)
    {
        this.drop = drop && Magazine > 0;
    }

    private void DropBomb()
    {
        bombTime = 0;
        --Magazine;
        Bomb bomb = Instantiate(bombPrefab, transform.position, transform.rotation);
        bomb.SetOwner(owner);

        float randomNumberX = Random.Range(-spread, spread);
        //float randomNumberY = Random.Range(-spread, spread);
        float randomNumberZ = Random.Range(-spread, spread);


        bomb.GetComponent<Rigidbody>().velocity = rb.velocity + new Vector3(randomNumberX, 0, randomNumberZ);
    }

    public void Activate(bool active)
    {
        OnActivationChange?.Invoke(active);
        if (!active)
        {
            drop = false;
        }
    }
}
