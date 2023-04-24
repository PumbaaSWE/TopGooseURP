using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveGameObject : MonoBehaviour
{
    [SerializeField][Range(0.0f, 1.0f)] float time;
    [SerializeField] float dissolveSpeed;
    List<Renderer> dissolveThese = new List<Renderer>();

    [SerializeField]
    Health health;

    // Start is called before the first frame update
    void Start()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].material.shader.name.Contains("Dissolve")) dissolveThese.Add(renderers[i]);
        }
    }

    public void Update()
    {
        if(health.Amount <= 0)
        {
            time += dissolveSpeed * Time.deltaTime;
            for (int i = 0; i < dissolveThese.Count; i++)
                dissolveThese[i].material.SetFloat("_T", time);

            if (time >= 1)
                Destroy(gameObject);
        }
    }
}
