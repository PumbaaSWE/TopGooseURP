using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnForFun : MonoBehaviour
{
    [SerializeField]
    Transform spawn;
    [SerializeField]
    float delay;
    float counter;
    [SerializeField]
    int maxAmount;

    private void Start()
    {
        counter = delay;
    }

    // Update is called once per frame
    void Update()
    {
        counter -= Time.deltaTime;

        if (counter > 0) return;
        if (transform.childCount > maxAmount) return;

        float x = Random.Range(-200, 200);
        float y = 200;
        float z = Random.Range(-200, 200);

        counter = delay;
        Instantiate(spawn, transform).transform.position = new Vector3(x,y,z);

    }
}
