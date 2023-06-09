using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class TrailPoolManager : MonoBehaviour
{
    private ObjectPool<TrailRenderer> trailPool;
    private TrailRenderer defaultTrail;

    private static TrailPoolManager instance; //singleton bad?
    public static TrailPoolManager Instance => instance;

    // Start is called before the first frame update
    void Awake()
    {

        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }

        trailPool = new ObjectPool<TrailRenderer>(OnCreate, OnGet, OnRelease, ActionOnDestroy, true, 100);
        transform.parent = null;
        transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity); // just incase som lunatic changes it
        transform.localScale = Vector3.one;

        defaultTrail = new GameObject("Trail").AddComponent<TrailRenderer>();
        defaultTrail.transform.SetParent(transform);
        defaultTrail.gameObject.SetActive(false);
        defaultTrail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        
    }


    //OnDestroy was taken...
    private void ActionOnDestroy(TrailRenderer trail)
    {
        Destroy(trail, trail.time);
    }

    private TrailRenderer OnCreate()
    {
        //TrailRenderer trail = 
        TrailRenderer trail = Instantiate(defaultTrail, transform);
        return trail;
    }

    private void OnGet(TrailRenderer trail)
    {
        //trail.Clear();
        trail.gameObject.SetActive(true);
    }
    private void OnRelease(TrailRenderer trail)
    {
        //trail.Clear();
        trail.gameObject.SetActive(false);
    }

    /// <summary>
    /// Gets a trail defined by specified config and paranted to specified transform
    /// </summary>
    /// <param name="config"></param>
    /// <param name="transform"></param>
    /// <returns></returns>
    public TrailRenderer GetTrail(TrailConfig config, Transform transform)
    {
        TrailRenderer trail = trailPool.Get();
        trail.time = config.time;
        trail.material = config.material;
        trail.colorGradient = config.color;
        trail.widthCurve = config.width;
        trail.minVertexDistance = config.minVertDistance;
        trail.numCapVertices = config.endCapVerices;
        trail.numCornerVertices = config.cornerVerices;
        trail.shadowCastingMode = config.castShadow;
        trail.gameObject.transform.SetPositionAndRotation(transform.position, transform.rotation);
        trail.gameObject.transform.SetParent(transform);
        trail.Clear();
        trail.emitting = true;
        return trail;
    }

    /// <summary>
    /// Returns the trail to this pool, will wait before releasing the trail to allow it to finish rendering. Optionally it can be removed immediately
    /// </summary>
    /// <param name="trail">Trail to be returned</param>
    /// <param name="immediate">if true the TrailRenderer will be deactivated</param>
    public void ReturnTrail(TrailRenderer trail, bool immediate = false)
    {
        trail.emitting = false;
        trail.gameObject.transform.SetParent(transform);
        StartCoroutine(ReleaseTrail(trail, immediate ? 0 : trail.time));
    }

    private IEnumerator ReleaseTrail(TrailRenderer trail, float t)
    {
        yield return new WaitForSeconds(t);
        trailPool.Release(trail);
    }


}
