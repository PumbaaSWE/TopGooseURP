using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class MissileLauncher : MonoBehaviour
{

    [SerializeField] private Missile missile;
    [SerializeField] private float reloadTime = 8;
    [SerializeField] private Transform[] hardpoints;

    [Space]
    [Tooltip("Draw Debug info in Scene view")]
    [SerializeField] private bool showDebugInfo = false;
    public float ReloadTime { get { return reloadTime; } set { reloadTime = value; } }
    public bool NoMissile => selectedHardpoint < 0;

    public bool Active { get; private set; }

    public delegate void OnActivated(bool selected);
    public OnActivated OnActivationChange;

    private HardpointData[] hardpointData;
    //private SeekerHead[] seekerHeads;
    private int selectedHardpoint = -1;


    private Rigidbody rb;
    private SeekerHead selectedMissile;

    private TeamMember owner;

    void Start()
    {
        TryGetComponent(out rb);
        hardpointData = new HardpointData[hardpoints.Length];
        owner = GetComponentInParent<TeamMember>();
        //seekerHeads = new SeekerHead[hardPoints.Length];
        //for (int i = 0; i < hardpoints.Length; i++)
        //{
        //    hardpoints[i].AddComponent<FixedJoint>();
        //}
    }

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;
        HandleHardpoints(dt);
    }

    void LateUpdate()
    {
        for (int i = 0; i < hardpointData.Length; i++)
        {
            if (hardpointData[i].IsLoaded)
            {
                hardpointData[i].missile.transform.SetPositionAndRotation(hardpoints[i].position, hardpoints[i].rotation);
            }
        }
    }

    /// <summary>
    /// Launch selected missile, missile will inherit velocity of any attatched rigidbody
    /// </summary>
    public void LaunchMissile()
    {
        if (selectedHardpoint < 0)
        {
            return;
        }
        Vector3 velocity = rb == null ? Vector3.zero : rb.velocity; 
        LaunchHardpoint(selectedHardpoint, velocity);
    }

    /// <summary>
    /// Manually uncage or recage the selected missile (DONT call if no missile!)
    /// </summary>
    /// <param name="uncage"></param>
    public void UncageMissile(bool uncage)
    {
        selectedMissile.Uncage(uncage);
    }

    /// <summary>
    /// Set cage direction of selected missile to aid aiming (DONT call if no missile!)
    /// </summary>
    /// <param name="cageDir"></param>
    public void SetCageDirection(Vector3 cageDir)
    {
        selectedMissile.Track(cageDir);
    }

    /// <summary>
    /// To change what missile prefab to spawn at reload
    /// </summary>
    /// <param name="seekerHead">The missile to use</param>
    public void SetSeekerHeadToUse(Missile missile)
    {
        this.missile = missile;
    }
    /// <summary>
    /// (UNTESTED!!!) To change hardpoints in code, to ease the use of ScriptableObjects, will destroy all current missiles
    /// </summary>
    /// <param name="hardPoints">New array of transforms to use, missiles spawn at .position looking .forward</param>
    public void SetHardpoints(Transform[] hardPoints)
    {
        this.hardpoints = hardPoints;
        for (int i = 0; i < hardpointData.Length; i++)
        {
            if (hardpointData[i].IsLoaded)
                Destroy(hardpointData[i].missile);
        }
        hardpointData = new HardpointData[hardPoints.Length];
        selectedHardpoint = -1;
        selectedMissile = null;
    }

    /// <summary>
    /// Will activate next availibe hardpoint or loop back to itself if that is the only loaded one
    /// </summary>
    /// <returns>Selected hardpoint or -1 if nothing is availibe</returns>
    private int SelectNextHardpoint() {

        //if(selectedHardpoint >= 0) selectedMissile.enabled = false; //disable current missile <- not needed if you cant manually switch (and method is private now)

        //BUG when the missile launcher "spawns" one missile will be activated and lock on even

        int index = selectedHardpoint + 1;
        for (int i = 0; i < hardpointData.Length; i++)
        {
            index = (index + i) % hardpointData.Length;

            if (hardpointData[index].IsLoaded)
            {
                selectedMissile = hardpointData[index].missile.SeekerHead;
                if(Active) selectedMissile.enabled = true;
                selectedHardpoint = index;
                return index;
            }
        }
        //KEEP THIS INCASE I BREAK SOMETHING
        //for (int i = 0; i < hardPointData.Length; i++)
        //{
        //    if (hardPointData[i].IsLoaded)
        //    {
        //        selectedMissile = hardPointData[i].seekerHead;
        //        return i;
        //    }
        //}
        selectedMissile = null;
        selectedHardpoint = -1;
        return -1;
    }

    //private Queue<(int hp, float time)> spawnQueue = new Queue<(int hp, float time)>(); //this works if all missiles have the same reload time, need sorted (priority) queue otherwise!

    private void HandleHardpoints(float dt)
    {
        //if (spawnQueue.Count == 0) return;
        //while(spawnQueue.Peek().time >= Time.time)
        //{
        //    SpawnMissileOnHardPoint(spawnQueue.Dequeue().hp);
        //}

        for (int i = 0; i < hardpointData.Length; i++)
        {

            if (hardpointData[i].IsLoaded)
            {
                //hardPointData[i].simpleFlight.transform.SetPositionAndRotation(hardPoints[i].position, hardPoints[i].rotation);
                continue;
            }

            hardpointData[i].respawnTime -= dt;
            if (hardpointData[i].respawnTime <= 0)
            {
                SpawnMissileOnHardpoint(i);
            }
        }
    }

    //This Instantiate an new missile and set important values and components, also parents the missile to this object
    //Would like not to parent to make movement more loose/less rigid maybe? flying birds after all...
    private void SpawnMissileOnHardpoint(int i)
    {

        Missile missile = Instantiate(this.missile, hardpoints[i].position, hardpoints[i].rotation);
        missile.Initialize(owner);
        hardpointData[i].missile = missile;

        //missile.Initialize();

        if (selectedHardpoint < 0)
        {
            SelectNextHardpoint();
        }
    }

    //Removes parent of missile, enables the rigidbody of the missile
    //Call hardpointData.Launch whitch does the exact same so some doubles here, but hardpointData had som trouble with rigidbody, so I think hardpointData should change
    //Important to call missile seekerHead.Launch though, now done in hardpointData
    //finally Select Next Hardpoint as current becomes unavalible
    private void LaunchHardpoint(int i, Vector3 initialVelocity)
    {
        hardpointData[i].Launch(initialVelocity, ReloadTime);
        //spawnQueue.Enqueue((i, Time.time));

        SelectNextHardpoint();
    }


    private void OnDrawGizmos()
    {
        if (showDebugInfo)
        {
            Color oldColor = Gizmos.color;
            Gizmos.color = oldColor;
        }
    }

    public Vector3 SeekerViewPositon()
    {
        if (selectedHardpoint < 0) return Vector3.zero;// transform.position+transform.forward*500;
        return hardpointData[selectedHardpoint].missile.SeekerHead.TargetPosition;
    }

    public SeekerHead GetSeekerHead()
    {
        if (selectedHardpoint < 0) return null;// transform.position+transform.forward*500;
        return hardpointData[selectedHardpoint].missile.SeekerHead;
    }

    public void Activate(bool active)
    {
        OnActivationChange?.Invoke(active);
        Active = active;
        if (selectedHardpoint < 0) return;
        selectedMissile.enabled = active;
        //selectedMissile.ClearTarget();
        //selectedMissile.Uncage(false);
        
    }

    //Clean this up! If  Rigidbody, SimpleFlight, Trail not really at home here remove them
    /// <summary>
    /// Keeps track of spawn time and the seekerHead of a hardpoint
    /// </summary>
    private struct HardpointData
    {
        internal float respawnTime;
        internal Missile missile;

        public bool IsLoaded => missile != null;

        public void Launch( Vector3 velocity, float t)
        {
            respawnTime = t;
            missile.Launch(velocity);
            missile = null;
        }
    }
}
