using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GnomePickUp : MonoBehaviour
{

    private GnomeScript gnomeScript;
    private bool carring = false;

    [SerializeField] GameObject gnomePrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (carring && Input.GetKeyDown(KeyCode.Space))
        {
            gnomeScript.Drop(GetComponentInParent<Rigidbody>().velocity);
            carring = false;
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            Instantiate(gnomePrefab);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        
        if (carring) return;
        if (other.TryGetComponent(out GnomeScript gs) && gs.CanBePickedUp)
        {
            gs.PickUp(transform);
            gnomeScript = gs;
            carring = true;
        }
    }
}
