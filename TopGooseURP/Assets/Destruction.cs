using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class Destruction : MonoBehaviour
{


    private Rigidbody[] rigidbodies;
    private float radius = 10; //for explosionForce, should be computed from bounds (kind of bounding sphere like)
    private bool destroyed;
    private Health health;


    [Tooltip("The rubble that is suppose to be left standing")][SerializeField] private GameObject foundation;
    [Tooltip("The new shiny house when not destroyed")][SerializeField] private GameObject model;
    [Space]
    [Tooltip("The explosion force applied to pieces when destruction starts")][SerializeField] private float force = 100;
    [Tooltip("Affect the pieces computed mass")][SerializeField] private float density = 1;
    [Tooltip("The pieces will compute mass based on bounds")][SerializeField] private bool calculateMass = true;
    [Tooltip("Minimum mass allowed")][SerializeField] private float minMass = 1.0f;
    [Tooltip("Maximum mass allowed")][SerializeField] private float maxMass = 100.0f;
    [Space]
    [Tooltip("Time before pieces start to sink into the ground")][SerializeField]private float sinkTime = 5;
    [Tooltip("The speed in m/s pieces sink into the ground")][SerializeField] private float sinkSpeed = 1;
    [Tooltip("Time before pieces are disabled from starting to sink")][SerializeField] private float removeTime = 5;
    [Space]
    [Tooltip("Set the smoke pillar when destryed (if null it will try children)")][SerializeField] private ParticleSystem smoke;
    //public bool destroyPieces = true;

    public ParticleSystem destructionSmoke;

    void Awake()
    {
        //foundation = 
        rigidbodies = GetComponentsInChildren<Rigidbody>(true);
        for (int i = 0; i < rigidbodies.Length; i++)
        {
            rigidbodies[i].gameObject.SetActive(false);
            if (calculateMass)
            {
                Vector3 size = rigidbodies[i].gameObject.GetComponent<MeshFilter>().mesh.bounds.size;
                float volume = size.x * size.y * size.z;
                rigidbodies[i].mass = Mathf.Clamp(volume * density, minMass, maxMass);
            }

            //Renderer renderer = rigidbodies[i].gameObject.GetComponent<Renderer>();
            //if (renderer.material.shader.name.Contains("Dissolve")) dissolveThese.Add(renderer);

        }
        if (foundation != null)
            foundation.SetActive(false);
        radius = model.GetComponent<Collider>().bounds.extents.magnitude; // does not necessarily bound everything, revisit this!!!!

        if (smoke == null)
            smoke = GetComponentInChildren<ParticleSystem>(true);
        if (smoke != null)
        {
            smoke.Stop();
            smoke.Clear();
        }

        health = GetComponent<Health>();
        health.OnDead += StartDestruction;
        enabled = false;

        if (destructionSmoke != null) { 
            destructionSmoke = Instantiate(destructionSmoke, transform);
            ParticleSystem.ShapeModule sm = destructionSmoke.shape;
            //sm.enabled = true;
            sm.meshRenderer = model.GetComponent<MeshRenderer>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //test
        //if (Input.GetKeyDown(KeyCode.Y))
        //{
        //    Debug.Log("dfgsdfghsfghsftrhgjsrftjhsrfjhsdrfj");
        //    StartDestruction();
        //}
        //if (!destroyed) return;
        //test

        removeTime -= Time.deltaTime;
        for (int i = 0; i < rigidbodies.Length; i++)
        {
            rigidbodies[i].transform.position += sinkSpeed * Time.deltaTime * Vector3.down;
        }
        if (removeTime < 0)
        {
            enabled = false;
            for (int i = 0; i < rigidbodies.Length; i++)
            {
                rigidbodies[i].gameObject.SetActive(false);
                //destroy them?
            }
        }
    }

    /// <summary>
    /// Will disable the model, show destroyed foundation if any and enable all debree and apply some explosion force to them. also Plays smoke effects if any.
    /// </summary>
    private void StartDestruction()
    {
        if (destroyed) return;
        model.SetActive(false);
        if (foundation != null)
            foundation.SetActive(true);
        for (int i = 0; i < rigidbodies.Length; i++)
        {
            rigidbodies[i].gameObject.SetActive(true);
            if(force > float.Epsilon)
                rigidbodies[i].AddExplosionForce(force, transform.position, radius);
        }
        if(smoke != null)
            smoke.Play();
        if(destructionSmoke !=null)
            destructionSmoke.Play();
        StartCoroutine(StartSink(sinkTime));
    }

    /// <summary>
    /// COROUTINE: Starts the sinking process after t seconds
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    private IEnumerator StartSink(float t)
    {
        yield return new WaitForSeconds(t);
        destroyed = true;
        enabled = true;
        for (int i = 0; i < rigidbodies.Length; i++)
        {
            rigidbodies[i].isKinematic = true;
        }

    }
}
