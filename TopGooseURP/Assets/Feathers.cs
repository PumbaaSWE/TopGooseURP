using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Feathers : MonoBehaviour
{
    [SerializeField] ParticleSystem particles;
    RagdollHandler ragdollHandler;

    private void Start()
    {
        ragdollHandler = GetComponentInParent<RagdollHandler>();

        ragdollHandler.onRagdollEnable += OnRagdoll;
    }

    private void OnRagdoll()
    {
        particles.enableEmission = false;

        Destroy(gameObject, 5);
    }
}
