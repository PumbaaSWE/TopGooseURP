using UnityEngine;
using UnityEngine.Pool;
using static UnityEngine.UI.GridLayoutGroup;

public class TargetHUDPool : MonoBehaviour
{

    private ObjectPool<TargetHUDAim> pool;
    [SerializeField] private TargetHUDAim prefab;


    /**
     * THESE NEEDS TO BE DYNAMIACALLY SET IN THE FUTURE!!!!
     */
    public TargetHUD owner;

    private float bulletSpeed;
    private float flyRange;

    // Start is called before the first frame update
    void Awake()
    {
        pool = new(OnCreate, OnGet, OnReturn);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetTracker(TargetHUD tracker, float bulletSpeed = 800, float flyRange = 500)
    {
        owner = tracker;
        this.bulletSpeed = bulletSpeed;
        this.flyRange = flyRange;
    }

    private TargetHUDAim OnCreate(){
        TargetHUDAim t = Instantiate(prefab, transform);
        t.SetTracker(owner, bulletSpeed, flyRange);
        return t;
    }


    private void OnGet(TargetHUDAim targetHUDAim)
    {
        targetHUDAim.gameObject.SetActive(true);
    }

    private void OnReturn(TargetHUDAim targetHUDAim)
    {
        targetHUDAim.gameObject.SetActive(false);
    }

    public TargetHUDAim Get()
    {
        return pool.Get();
    }

    public void Return(TargetHUDAim targetHUDAim)
    {
        pool.Release(targetHUDAim);
        //targetHUDAim.gameObject.SetActive(false);
    }
}
