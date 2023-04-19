using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class OnDeath : MonoBehaviour
{
    [SerializeField] Renderer renderer;
    [SerializeField] RagdollHandler ragdollHandler;
    [SerializeField] FlightController flightController;
    [SerializeField] ManualFlightInput manualFlightInput;
    [SerializeField] float dissolveSpeed;
    [SerializeField] float startDissolvingAfter;

    private void OnEnable() => ragdollHandler.onRagdollEnable += OnRagdoll;
    private void OnDisable() => ragdollHandler.onRagdollEnable -= OnRagdoll;

    float t;
    bool dissolve, ragdoll, dead;

    void Update()
    {
        //For now, die when pressing space
        if (Input.GetKeyDown(KeyCode.Space) && dead == false)
        {
            OnDeathDo();
        }

        //If you haven't ragdolled yet you shall not pass!
        if (!ragdoll)
            return;
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
            renderer.material.SetFloat("_T", t);
        }
        //When fully dissolved, remove gameObject from the scene
        else
            Destroy(gameObject);
    }

    private void OnRagdoll()
    {
        ragdoll = true;
    }

    private void OnDeathDo()
    {
        flightController.ResetInput();
        flightController.DisableInput = true;
        dead = true;
    }
}
