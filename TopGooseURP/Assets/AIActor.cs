using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class AIActor : MonoBehaviour
{
    public CombatCoordinator.Role Role { get; private set; }
    public CombatCoordinator.Role role = CombatCoordinator.Role.Chaser;
    public CombatCoordinator CombatCoordinator;
    public List<IUtility> utilities = new List<IUtility>();
    private int previusUtil = 0;
    private int currentUtil = 0;


    [Space]
    [Header("Ground Avoiding")]
    [SerializeField] private float radius = 3;
    [SerializeField] private float bumbModifier = 2;
    [SerializeField] private float rangeModifier = 2;
    [SerializeField] private LayerMask groundLayer;

    //[Space]
    //[Header("Shooting")]
    //[SerializeField][Range(0.0f, 1.0f)] float ramming = 0;
    //[SerializeField][Range(0.0f, 1.0f)] float guns = 0;
    //[SerializeField][Range(0.0f, 45.0f)] float gunsConeToFire = 1.0f;
    //private float gunsAlignToFire;
    //[SerializeField][Range(0.0f, 1.0f)] float minHeat = 0.3f;
    //[SerializeField][Range(0.0f, 1.0f)] float maxHeat = 0.9f;
    //private bool gunsOverheat;
    //[SerializeField] private LayerMask targetLayer;
    //private Vector3 ramTarget; // vector to ram target
    //private Vector3 gunSolutionTarget; // vector to get guns on target target
    //[SerializeField] private float bulletSpeed = 800; //to be allocated from Guns attached later


    private Autopilot autopilot;
    private FlightController controller;
    private Vector3 flyTarget = Vector3.zero; // actual fly towards target

    void Start()
    {

        autopilot = GetComponent<Autopilot>();
        controller = GetComponent<FlightController>();
        controller.SetThrottleInput(1.0f);

        //auto fill list based on attached components implementing IUtility?
        Component[] utilities = GetComponents(typeof(IUtility));

        for (int i = 0; i < utilities.Length; i++)
        {
            this.utilities.Add(utilities[i] as IUtility);
            //utilities[i].
        }

        if(TryGetComponent(out Health health))
        {
            health.OnDead += OnDeathCallback;
        }
        Role = role;
    }

    void FixedUpdate()
    {
        if (utilities.Count == 0) return;

        //evaluate every frame needed?
        EvaluateUtils();

        utilities[currentUtil].Execute();

        UpdateAutopilot();
    }

    /// <summary>
    /// Run every frame to see which of attached Utilities score the hightest to then execute that one.
    /// Also tell the current util running to exit if another utils now score higher
    /// </summary>
    private void EvaluateUtils()
    {
        //Evaluating every frame is too often?
        float maxScore = -1;
        for (int i = 0; i < utilities.Count; i++)
        {
            float score = utilities[i].Evaluate();
            if (score > maxScore)
            {
                currentUtil = i;
                maxScore = score;
            }
        }
        if (currentUtil != previusUtil)
        {
            //possible cleanup/startup calls?
            utilities[currentUtil].Exit();
            previusUtil = currentUtil;
        }
    }

    private void OnDeathCallback() {
        enabled = false;
    }

    /// <summary>
    /// If the combat cordinator has a spot for some role this AI will accept it if its "better" than current role. //This is not optimal...
    /// 
    /// </summary>
    /// <param name="role"></param>
    /// <returns>true if role is accepted otherwise false</returns>
    public bool OfferRole(CombatCoordinator.Role role)
    {
        if(role < Role)
        {
            Role = role;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Returns role to combat coordinator, to be called by utils requiring a certain role to execute some behavior, like chasing
    /// </summary>
    public void ReturnRole()
    {
        if(CombatCoordinator != null)
            CombatCoordinator.ReturnRole(Role);
        Role = CombatCoordinator.Role.Idler;
    }

    private void OnDisable()
    {
        ReturnRole();
    }

    private void OnDestroy()
    {
        ReturnRole();
    }
    /// <summary>
    /// To be used by the utilities to say where they want to go, target will changed based on ground avoidance settings before outputted to the autopilot
    /// </summary>
    /// <param name="flyTarget"></param>
    public void SetFlyTarget(Vector3 flyTarget)
    {
        this.flyTarget = flyTarget;
    }

    /// <summary>
    /// Target set by utils if thay want to fly anywhere with ground avoidance checks
    /// </summary>
    private void UpdateAutopilot()
    {
        if (Physics.SphereCast(transform.position, radius, transform.forward, out RaycastHit hit, controller.LocalVelocity.z * rangeModifier, groundLayer))
        {
            flyTarget = transform.position + transform.forward + (hit.normal + Vector3.up) * bumbModifier;
        }
        else if (Physics.SphereCast(transform.position, radius, flyTarget - transform.position, out hit, controller.LocalVelocity.z * rangeModifier, groundLayer))
        //else if (Physics.Linecast(transform.position, flyTarget, out hit, groundLayer))
        {
            flyTarget = hit.point + (hit.normal + Vector3.up) * bumbModifier;
        }
        if (Physics.CheckSphere(transform.position + transform.forward + Vector3.down, radius, groundLayer))
        {
            flyTarget = transform.position + transform.forward + Vector3.up * radius;
        }


        if (Physics.Raycast(flyTarget + Vector3.up * 100, Vector3.down, out hit, 200, groundLayer))
        {
            if (hit.point.y > flyTarget.y)
            {
                flyTarget.y = hit.point.y + radius;
            }
        }


        autopilot.RunAutopilot(flyTarget, out float pitch, out float yaw, out float roll);
        Vector3 input = new(pitch, yaw, roll);
        controller.SetControlInput(input);
    }

}
