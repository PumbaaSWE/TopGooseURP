using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class DissolveGameObject : MonoBehaviour
{
    [SerializeField][Range(0.0f, 1.0f)] float time;
    [SerializeField] float dissolveSpeed;
    [SerializeField] Material dissolveMaterial;
    List<Renderer> dissolveThese = new List<Renderer>();
    bool doOnce;

    Health health;

    // Start is called before the first frame update
    void Start()
    {
        health = GetComponent<Health>();
        doOnce = true;
    }

    public void Update()
    {
        if(health.Amount <= 0)
        {
            if(doOnce)
            {
                Renderer[] renderers = GetComponentsInChildren<Renderer>();
                for (int i = 0; i < renderers.Length; i++)
                {
                    renderers[i].sharedMaterial = dissolveMaterial;
                    if (renderers[i].material.shader.name.Contains("Dissolve")) dissolveThese.Add(renderers[i]);
                }
                doOnce = true;
            }

            time += dissolveSpeed * Time.deltaTime;
            for (int i = 0; i < dissolveThese.Count; i++)
                dissolveThese[i].material.SetFloat("_T", time);

            if (time >= 1)
                Destroy(gameObject);
        }
    }
}
