using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trail : MonoBehaviour
{
    
    [SerializeField]private TrailConfig config;

    private TrailPoolManager trailPool;

    private TrailRenderer trailRenderer;

    void Start()
    {
        trailPool = TrailPoolManager.Instance;
        if(trailPool == null)
        {
            //enabled = false;
            new GameObject("TrailPoolManager", typeof(TrailPoolManager));
            trailPool = TrailPoolManager.Instance;
            trailRenderer = trailPool.GetTrail(config, transform);
        }
        else
        {
            trailRenderer = trailPool.GetTrail(config, transform);
            //enabled = true;
        }
           
        
    }

    public void Emitting(bool emitting)
    {
        trailRenderer.emitting = emitting;
    }

    // Update is called once per frame
    void Update()
    {
        //trailRenderer.gameObject.transform.SetPositionAndRotation(transform.position, transform.rotation);
    }

    private void OnDestroy()
    {
        if (trailPool) trailPool.ReturnTrail(trailRenderer);
        //trailRenderer = null;
        //trailPool = null;
    }
}
