using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileLauncher : MonoBehaviour
{

    public SeekerHead missile;
    public Vector3 initVel = new Vector3(0, 0, 10);
    public float maxSpeed = 25;

    [Space]
    [Tooltip("Draw Debug info in Scene view")]
    [SerializeField] private bool showDebugInfo = false;



    [SerializeField] private ManualFlightInput flightInput;

    [SerializeField] private Transform[] hardPoints;
    private HardPointData[] hardPointData;
    private int selectedHardpoint = -1;

    public AudioClip[] tones = new AudioClip[4];
    AudioSource audioSource;

    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        hardPointData = new HardPointData[hardPoints.Length];
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        for (int i = 0; i < tones.Length; i++)
        {
            tones[i].LoadAudioData();
            
        }
        audioSource.clip = tones[0];
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            GameObject go = Instantiate(missile.gameObject, transform.position - transform.up, transform.rotation);

            //if(go.TryGetComponent(out Rigidbody rigidbody))
            //{
            //    rigidbody.velocity = rb.velocity;
            //}
            //else
            //{
            //    Debug.LogWarning("MissileLauncher - cannot access the rigidbody component of the missile");
            //}

            if (go.TryGetComponent(out SeekerHead seeker))
            {
                seeker.Launch(rb.velocity);
            }
            else
            {
                Debug.LogWarning("MissileLauncher - cannot access the SeekerHead component of the missile");
            }

            //if (go.TryGetComponent(out SimpleFlight simpleFlight))
            //{
            //    simpleFlight.SetMaxSpeed(maxSpeed);
            //}
            //else
            //{
            //    Debug.LogWarning("MissileLauncher - cannot access the SimpleFlight component of the missile");
            //}

            Destroy(go, 5.0f);
        }

        float dt = Time.deltaTime;
        HandleHardpoints(dt);

        if (selectedHardpoint < 0) return;





        //Debug.Log("selectedHardpoint = " + selectedHardpoint);

        SeekerHead selectedMissile = hardPointData[selectedHardpoint].seekerHead;
        //Debug.Log("Tone = " + selectedMissile.Tone);

        if(flightInput != null)
        {
            selectedMissile.Track(flightInput.MouseAimPos - transform.position);
        }
        else
        {
            selectedMissile.Track(transform.forward);

        }

        //selectedMissile.debugDraw = true;
        switch (selectedMissile.Tone)
        {
            case SeekerHead.SeekerTone.Active:
                audioSource.clip = tones[0];
                break;
            case SeekerHead.SeekerTone.InView:
                audioSource.clip = tones[1];
                break;
            case SeekerHead.SeekerTone.InViewOffBore:
                audioSource.clip = tones[2];
                break;
            case SeekerHead.SeekerTone.Locked:
                audioSource.clip = tones[3];
                break;
        }
        


        if (Input.GetKeyDown(KeyCode.T))
        {
            audioSource.Stop();
            hardPointData[selectedHardpoint].Launch(rb.velocity, 5);
            //Debug.Log("Hardpoint " + selectedHardpoint + " launched, " + hardPointData[selectedHardpoint].IsLoaded);
            selectedHardpoint = NextHardpoint();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            selectedMissile.Uncage(true);
        }
    }


    private int NextHardpoint() {
        for (int i = 0; i < hardPointData.Length; i++)
        {
            if (hardPointData[i].IsLoaded) return i;
        }
        return -1;
    }

    private void HandleHardpoints(float dt)
    {
        for (int i = 0; i < hardPointData.Length; i++)
        {

            if (hardPointData[i].IsLoaded) continue;

            hardPointData[i].respawnTime -= dt;
            if (hardPointData[i].respawnTime <= 0)
            {
                SpawnMissileOnHardPoint(i);
                if (selectedHardpoint < 0) selectedHardpoint = i;
            }
        }
    }

    private void SpawnMissileOnHardPoint(int i)
    {
        SeekerHead seekerHead = Instantiate(missile, hardPoints[i].position, hardPoints[i].rotation);
        seekerHead.transform.parent = hardPoints[i]; //do it after to no fukk up scaling
        if (seekerHead.TryGetComponent(out Rigidbody rigidbody))
        {
            rigidbody.isKinematic = true;
        }
        else
        {
            Debug.LogWarning("MissileLauncher - SpawnMissileOnHardPoint - cannot access the rigidbody component of the missile");
        }

        //if (seekerHead.gameObject.TryGetComponent(out FlappyWings flappyWings))
        //{
        //    flappyWings.Flap(false);
        //}
        //else
        //{
        //    Debug.LogWarning("MissileLauncher - SpawnMissileOnHardPoint - cannot access the rigidbody component of the missile");
        //}

        //if (seekerHead.gameObject.TryGetComponent(out Trail trail))
        //{
        //    trail.Emitting(false);
        //}
        //else
        //{
        //    Debug.LogWarning("MissileLauncher - SpawnMissileOnHardPoint - cannot access the rigidbody component of the missile");
        //}
        hardPointData[i].seekerHead = seekerHead;
        //hardPointData[i].trail = trail;
    }

    private void LaunchHardPoint(int i)
    {
        hardPointData[i].seekerHead.transform.parent = null;
        //hardPointData[i].seekerHead.Launch(rb.velocity);
        if (hardPointData[i].seekerHead.TryGetComponent(out Rigidbody rigidbody))
        {
            rigidbody.isKinematic = false;
        }
        hardPointData[i].seekerHead = null;
    }


    private void OnDrawGizmos()
    {
        if (showDebugInfo)
        {
            Color oldColor = Gizmos.color;

            

            Gizmos.color = oldColor;
        }
    }

    internal Vector3 SeekerViewPositon()
    {
        if (selectedHardpoint < 0) return Vector3.zero;// transform.position+transform.forward*500;
        return hardPointData[selectedHardpoint].seekerHead.TargetPosition;
    }

    internal SeekerHead GetSeekerHead()
    {
        if (selectedHardpoint < 0) return null;// transform.position+transform.forward*500;
        return hardPointData[selectedHardpoint].seekerHead;
    }

    internal bool NoMissile => selectedHardpoint < 0;

    private struct HardPointData
    {
        public float respawnTime;
        public SeekerHead seekerHead;
        public Rigidbody rigidbody;
        public SimpleFlight simpleFlight;
        internal Trail trail;

        public bool IsLoaded => seekerHead != null;

        public void Launch( Vector3 velocity, float t)
        {
            respawnTime = t;
            seekerHead.Launch(velocity);
            seekerHead = null;
            rigidbody = null;
            simpleFlight = null;
            //trail.Emitting(true);
            //trail = null;
        }
    }
}
