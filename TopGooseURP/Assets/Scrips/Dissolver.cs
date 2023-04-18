using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolver : MonoBehaviour
{
    [SerializeField] [Range(0.0f, 1.0f)] float time;
    [SerializeField] Renderer renderer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        renderer.material.SetFloat("_T", time);
    }
}
