using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunInput : MonoBehaviour
{
    public Gun[] guns;
    // Start is called before the first frame update
    float bombTime;

    public GameObject bombBrefab;
    [SerializeField] private float spread = .1f;

    Rigidbody rb;

    void Start()
    {
        guns = GetComponentsInChildren<Gun>();
        if(guns == null) gameObject.SetActive(false);
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        for (int i = 0; i < guns.Length; i++)
        {
            guns[i].Fire = Input.GetMouseButton(0);
            
        }

        bombTime += Time.deltaTime;
        if (Input.GetMouseButton(1) && bombTime>.3f)
        {
            bombTime = 0;
            GameObject bomb = Instantiate(bombBrefab, transform.position+Vector3.down, transform.rotation);

            float randomNumberX = Random.Range(-spread, spread);
            float randomNumberY = Random.Range(-spread, spread);
            float randomNumberZ = Random.Range(-spread, spread);
            Quaternion rotation = Quaternion.Euler(randomNumberX, randomNumberY, randomNumberZ);


            bomb.GetComponent<Rigidbody>().velocity = rb.velocity;
            bomb.transform.rotation *= rotation;
        }


    }
}
