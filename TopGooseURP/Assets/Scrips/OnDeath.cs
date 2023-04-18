using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class OnDeath : MonoBehaviour
{
    [SerializeField] Renderer renderer;
    [SerializeField] RagdollHandler ragdollHandler;
    [SerializeField] float dissolveSpeed;
    [SerializeField] float startDissolvingAfter;

    private void OnEnable() => ragdollHandler.onRagdollEnable += OnRagdoll;
    private void OnDisable() => ragdollHandler.onRagdollEnable -= OnRagdoll;

    float t;
    bool dissolve, ragdoll;

    void Update()
    {
        if (!ragdoll)
            return;
        else if (!dissolve)
        {
            startDissolvingAfter -= Time.deltaTime;

            if (startDissolvingAfter < 0)
                dissolve = true;
        }
        else if (t < 1)
        {
            t += dissolveSpeed * Time.deltaTime;
            renderer.material.SetFloat("_T", t);
        }
    }

    private void OnRagdoll()
    {
        ragdoll = true;
    }
}
