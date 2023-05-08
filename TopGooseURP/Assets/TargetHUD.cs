using System.Collections.Generic;
using UnityEngine;

public class TargetHUD : MonoBehaviour
{

    public CapsuleCollider targetVolume; //change to private after debugging
    
    public TargetHUDPool pool;
    public List<TargetHUDAim> targets = new();

    private Dictionary<TargetHUDAim, int> targetsMap = new();

    private TeamMember team;

    [SerializeField] private float range = 300;
    [SerializeField] private float radius = 60;
    [Tooltip("What layer is searched for")][SerializeField] private LayerMask searchLayer; //does this work?

    /**
     * THESE NEEDS TO BE DYNAMIACALLY SET IN THE FUTURE!!!!
     */
    [Tooltip("FIX THIS")][SerializeField] private float bulletSpeed = 100;
    [Tooltip("FIX THIS")][SerializeField] private float flyRange = 500;

    // Start is called before the first frame update
    void Start()
    {
        if(targetVolume == null) //needs some local thingy ma thing
        {
            targetVolume = gameObject.AddComponent<CapsuleCollider>();
            targetVolume.isTrigger = true;
            targetVolume.radius = radius;
            targetVolume.height = range;
            targetVolume.center = transform.position + (radius + range / 2) * transform.forward;
        }
        team = GetComponent<TeamMember>();
        //rb = GetComponent<Rigidbody>();
        //targetVolume.
        pool.SetTracker(this, bulletSpeed, flyRange);
    }

    // Update is called once per frame
    void FixedUpdate()
    {


    }



    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out TeamMember teamMember))
        {
            return;
        }
        if (teamMember.SameTeam(team))
        {
            return;
        }


        Rigidbody rigidbody = other.GetComponent<Rigidbody>();
        if (pool == null) return; // tydligen skapas den efter goose? Lägga den högre i sceen view?
        TargetHUDAim t = pool.Get();
        t.ActivateTracking(other.transform, rigidbody);
        if (!targets.Contains(t))
        {
            targets.Add(t);
            teamMember.OnDeathCallback += ReturnAndRemove;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        ReturnAndRemove(other.transform);
    }

    public void ReturnAndRemove(Transform other)
    {
        for (int i = 0; i < targets.Count; i++)
        {
            if (other == targets[i].TrackedTransform)
            {
                pool.Return(targets[i]);
                //targets[i].DeactivateTracking();
                targets.RemoveAt(i);
            }
        }
    }

    private void ReturnAndRemove(TeamMember teamMember)
    {
        teamMember.OnDeathCallback -= ReturnAndRemove;
        ReturnAndRemove(teamMember.transform);
    }
}
