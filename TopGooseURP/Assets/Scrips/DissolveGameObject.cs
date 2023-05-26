using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class DissolveGameObject : MonoBehaviour
{
    [SerializeField] float aliveTime;
    [SerializeField] float dissolveSpeed;
    [SerializeField] Material dissolveMaterial;
    List<Renderer> dissolveThese = new List<Renderer>();
    float dissolveTime;
    Health health;

    // Start is called before the first frame update
    void Start()
    {
        health = GetComponent<Health>();
        health.OnDead += OnDead;
        enabled = false;
    }

    private void OnDead()
    {
        StartCoroutine(StartDissolve());
    }

    private IEnumerator StartDissolve()
    {
        yield return new WaitForSeconds(aliveTime);
        enabled = true;
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].sharedMaterial = dissolveMaterial;
            if (renderers[i].material.shader.name.Contains("Dissolve")) dissolveThese.Add(renderers[i]);
        }
    }

    public void Update()
    {
        if(health.Amount <= 0)
        {
            dissolveTime += dissolveSpeed * Time.deltaTime;
            for (int i = 0; i < dissolveThese.Count; i++)
                dissolveThese[i].material.SetFloat("_T", dissolveTime);

            if (dissolveTime >= 1)
                Destroy(gameObject);
        }
    }
}
