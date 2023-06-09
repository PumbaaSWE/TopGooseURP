using UnityEngine;
/// <summary>
/// A component to draw trail after an object, if the object is destroyed the trail will persist until its done before returning to an object pool 
/// </summary>
public class Trail : MonoBehaviour
{
    
    [SerializeField]private TrailConfig config;

    private TrailPoolManager trailPool;

    private TrailRenderer trailRenderer;

    void Awake()
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

        //Emitting(false);
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
