using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class OnDeath : MonoBehaviour
{
    
    RagdollHandler ragdollHandler;
    FlightController flightController;
    Health health;
    Rigidbody rigidBody;
    List<Renderer> dissolveThese = new List<Renderer>();

    [SerializeField]
    GameObject feathers;

    [SerializeField]
    float velocityCheck;

    [SerializeField]
    int velocityCountTime;

    [SerializeField]
    float dissolveSpeed;

    [SerializeField]
    float startDissolvingAfter;

    [SerializeField]
    bool destroyGameObject;

    [Space]
    //[SerializeField] private InGameMenu inGameMenu;

    float t, spin, spinPreviousUpdate;
    bool dissolve, ragdoll, dead, counterClockWise;

    private void Start()
    {
        ragdollHandler = GetComponent<RagdollHandler>();
        flightController = GetComponent<FlightController>();
        health = GetComponent<Health>();
        rigidBody = GetComponent<Rigidbody>();

        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].material.shader.name.Contains("Dissolve")) dissolveThese.Add(renderers[i]);
        }

        ragdollHandler.onRagdollEnable += OnRagdoll;
        health.OnDead += OnDeathDo;
    }

    void FixedUpdate()
    {
        //When not dead, keep track of the previous spin in order to determine which direction to roll on death
        if(health.Amount > 0)
        {
            spinPreviousUpdate = spin;
            spin = transform.rotation.eulerAngles.z;
        }

        if (rigidBody.velocity.magnitude < velocityCheck) StartCoroutine(CheckVelocity(velocityCountTime));

        //If you haven't died yet, you shall not pass!
        if (!dead) return;

        //If you haven't ragdolled yet, roll!
        if (!ragdoll)
        {
            if (rigidBody.velocity.magnitude < velocityCheck)
            {
                ragdollHandler.EnableRagdoll();
                return;
            }

            spin += (counterClockWise) ? Time.deltaTime * 180 : Time.deltaTime * -180;
            transform.forward = rigidBody.velocity;
            transform.Rotate(Vector3.forward * spin, Space.Self);
        }

        //If not dissolving yet, count down time until dissolve
        else if (!dissolve)
        {
            startDissolvingAfter -= Time.deltaTime;

            if (startDissolvingAfter < 0)
                dissolve = true;
        }

        //If dissolving and not dissolved yet, add to "t"-value
        else if (t < 1)
        {
            t += dissolveSpeed * Time.deltaTime;
            for (int i = 0; i < dissolveThese.Count; i++)
            {
                dissolveThese[i].material.SetFloat("_T", t);
            }
        }

        //When fully dissolved, remove gameObject from the scene
        else
        {
            if (destroyGameObject) Destroy(gameObject);

            else
            {
                gameObject.SetActive(false);
                //if(inGameMenu != null)
                //{
                //    inGameMenu.EndScreen();
                //}
            }
        }
    }

    private void OnRagdoll()
    {
        dead = true;
        ragdoll = true;
    }

    private void OnDeathDo()
    {
        rigidBody.AddRelativeTorque(new Vector3(0, 0, 0.2f), ForceMode.Impulse);
        dead = true;
        flightController.enabled = false;

        if (spin > spinPreviousUpdate)
            counterClockWise = true;

        var feathersInstance = Instantiate(feathers, gameObject.transform.position, Quaternion.identity);
        feathersInstance.transform.parent = gameObject.transform;
    }

    private IEnumerator CheckVelocity(int t)
    {
        yield return new WaitForSeconds(t);

        if(rigidBody.velocity.magnitude < velocityCheck) health.ChangeHealth(-99999, ChangeHealthType.bullet, null);
    }
}
