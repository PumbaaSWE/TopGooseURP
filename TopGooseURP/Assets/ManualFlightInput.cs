using UnityEngine;

public class ManualFlightInput : MonoBehaviour
{

    /**
     * https://github.com/brihernandez/MouseFlight
     * This is based on Brian Hernandez MouseFlightController published under MIT Licence, see link
     */

    /*
     * Game objects should be:
     * HUD
     * Rig:
     * Rig>CameraRig:
     * Rig>CameraRig>Camera
     * Rig>MouseAim
     * Aircraft
     * 
     * Rig, CameraRig, and MouseAim should have zeroed transforms!
     * Camera should be positionen as you want it (ex. above and behind) in relation to the Aircraft
     * when it also is located at origin and forward is forward.
     * 
     * Aircraft can later be positioned anywhere in the scene as the Rig will follow with this script
     * 
     *** The game objects may have additional children, not required for basic function
     * 
     * Scripts:
     * Rig should have this script!
     * Aircraft should have FlightController and Autopilot scripts (as it is now when they control thing they attach to)
     * 
     *** Note, Rig will unparent itself to not accidentally inherit some transform, don't panic!
     */


    [Header("Components")]
    [Tooltip("Transform of the aircraft the rig follows and references")]
    [SerializeField] private Transform aircraft;
    [Tooltip("Transform of the object the mouse rotates to generate MouseAim position")]
    [SerializeField] private Transform mouseAim;
    [Tooltip("Transform of the object on the rig which the camera is attached to")]
    [SerializeField] private Transform cameraRig;
    [Tooltip("Transform of the camera itself")]
    [SerializeField] private Transform cam;
    [Tooltip("The Flight controller used")]
    [SerializeField] private FlightController controller;
    [Tooltip("The autopilot used")]
    [SerializeField] private Autopilot pilot;

    [Header("Options")]
    [Tooltip("Follow aircraft using fixed update loop")]
    [SerializeField] private bool useFixed = true;

    [Tooltip("How quickly the camera tracks the mouse aim point.")]
    [SerializeField] private float camSmoothSpeed = 5f;

    //Sensitivity is 0.15f to match old system input
    [Tooltip("Mouse sensitivity for the mouse flight target")]
    [SerializeField][Range(0.01f,1f)] private float mouseSensitivity = 0.15f;

    [Tooltip("How far the boresight and mouse flight are from the aircraft")]
    [SerializeField] private float aimDistance = 500f;

    [Tooltip("Deadzone like, not too relevant for keyboard")]
    [SerializeField][Range(0,1)] private float inputThreshold = .25f;

    [Tooltip("When looking up or down stop aligning with horizon and align with local up")]
    [SerializeField][Range(0, 1)] private float camAlignThreshold = .9f;

    [Space]
    [Tooltip("Draw Debug info in Scene view")]
    [SerializeField] private bool showDebugInfo = false;
    [Tooltip("Use the advanced auto pilot in autopilot")]
    [SerializeField] private bool useAdvancedAutopilot = false;

    //[SerializeField] private float mouseXsens = 100.0f;
    //[SerializeField] private float mouseYsens = 100.0f;

    [Space]
    [SerializeField] private GameInput gameInput;

    private Vector3 frozenDirection = Vector3.forward;

    private bool isMouseAimFrozen = false;


    /// <summary>
    /// Get a point along the aircraft's boresight projected out to aimDistance meters.
    /// Useful for drawing a crosshair to aim fixed forward guns with, or to indicate what
    /// direction the aircraft is pointed.
    /// </summary>
    public Vector3 BoresightPos
    {
        get
        {
            return aircraft == null
                 ? transform.forward * aimDistance
                 : (aircraft.transform.forward * aimDistance) + aircraft.transform.position;
        }
    }

    /// <summary>
    /// Get the position that the mouse is indicating the aircraft should fly, projected
    /// out to aimDistance meters. Also meant to be used to draw a mouse cursor.
    /// </summary>
    public Vector3 MouseAimPos
    {
        get
        {
            if (mouseAim != null)
            {
                return isMouseAimFrozen
                    ? GetFrozenMouseAimPos()
                    : mouseAim.position + (mouseAim.forward * aimDistance);
            }
            else
            {
                return transform.forward * aimDistance;
            }
        }
    }
    /// <summary>
    /// Get the direction that the mouse is looking relative to the aircraft
    /// </summary>
    public Vector3 MouseAimDirection
    {
        get
        {
            if (mouseAim != null)
            {
                return isMouseAimFrozen
                    ? transform.forward
                    : mouseAim.forward;
            }
            else
            {
                return transform.forward;
            }
        }
    }

    private void Awake()
    {
        if (aircraft == null)
            Debug.LogError(name + "ManualFlightInput - No aircraft transform assigned!");
        if (mouseAim == null)
            Debug.LogError(name + "ManualFlightInput - No mouse aim transform assigned!");
        if (cameraRig == null)
            Debug.LogError(name + "ManualFlightInput - No camera rig transform assigned!");
        if (cam == null)
            Debug.LogError(name + "ManualFlightInput - No camera transform assigned!");
        if (controller == null)
            Debug.LogError(name + "ManualFlightInput - No FlightController assigned!");

        // To work correctly, the entire rig must not be parented to anything.
        // When parented to something (such as an aircraft) it will inherit those
        // rotations causing unintended rotations as it gets dragged around.
        transform.parent = null;
    }

    private void Start()
    {
        controller.SetThrottleInput(1);
    }

    private void Update()
    {
        //this should not be here
        if (Input.GetKeyDown(KeyCode.F1) || Input.GetKeyDown(KeyCode.P))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = !Cursor.visible;
            if (Cursor.visible)
            {
                Cursor.lockState = CursorLockMode.None;
            }
        }



    }
    private void LateUpdate()
    {
        if (useFixed == false)
            UpdateCameraPos();
        RotateRig();
    }



    private void FixedUpdate()
    {
        if (useFixed == true)
            UpdateCameraPos();
        HandleFlightInput(Time.fixedDeltaTime);
    }

    private void HandleFlightInput(float dt)
    {
        bool rollOverride = false;
        bool pitchOverride = false;

        float keyboardRoll = -Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(keyboardRoll) > inputThreshold)
        {
            rollOverride = true;
        }

        float keyboardPitch = Input.GetAxisRaw("Vertical");
        if (Mathf.Abs(keyboardPitch) > inputThreshold)
        {
            pitchOverride = true;
            rollOverride = true; // why?
        }

        float autoPitch;
        float autoYaw;
        float autoRoll;

        if (!useAdvancedAutopilot)
        {
            pilot.RunAutopilot(MouseAimPos, out autoPitch, out autoYaw, out autoRoll);
        }
        else
        {
            pilot.RunAdvancedAutopilot2(MouseAimPos, dt, out autoPitch, out autoYaw, out autoRoll);
        }

        float yaw = autoYaw;
        float pitch = (pitchOverride) ? keyboardPitch : autoPitch;
        float roll = (rollOverride) ? keyboardRoll : autoRoll;

        Vector3 input = new(pitch, yaw, roll);
        controller.SetControlInput(input);
    }

    private void RotateRig()
    {
        if (mouseAim == null || cam == null || cameraRig == null)
            return;

        // Freeze the mouse aim direction when the free look key is pressed.
        if (Input.GetKeyDown(KeyCode.C))
        {
            isMouseAimFrozen = true;
            frozenDirection = mouseAim.forward;
        }
        else if (Input.GetKeyUp(KeyCode.C))
        {
            isMouseAimFrozen = false;
            mouseAim.forward = frozenDirection;
        }
        //// Mouse input.
        //float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        //float mouseY = -Input.GetAxis("Mouse Y") * mouseSensitivity;
        
        Vector2 mouseAxis = gameInput.MouseAxis();
        float mouseX = mouseAxis.x * mouseSensitivity;
        float mouseY = -mouseAxis.y * mouseSensitivity;

        float scroll = gameInput.ThrottleChangeActionNormalized();
        controller.SetThrottleInput(controller.Throttle + scroll);
        
        // Rotate the aim target that the plane is meant to fly towards.
        // Use the camera's axes in world space so that mouse motion is intuitive.
        mouseAim.Rotate(cam.right, mouseY, Space.World);
        mouseAim.Rotate(cam.up, mouseX, Space.World);

        // The up vector of the camera normally is aligned to the horizon. However, when
        // looking straight up/down this can feel a bit weird. At those extremes, the camera
        // stops aligning to the horizon and instead aligns to itself.
        Vector3 upVec = (Mathf.Abs(mouseAim.forward.y) > camAlignThreshold) ? cameraRig.up : Vector3.up;

        // Smoothly rotate the camera to face the mouse aim.
        cameraRig.rotation = Damp(cameraRig.rotation, Quaternion.LookRotation(mouseAim.forward, upVec), camSmoothSpeed, Time.deltaTime);
    }

    private Vector3 GetFrozenMouseAimPos()
    {
        if (mouseAim != null)
            return mouseAim.position + (frozenDirection * aimDistance);
        else
            return transform.forward * aimDistance;
    }

    private void UpdateCameraPos()
    {
        if (aircraft != null)
        {
            // Move the whole rig to follow the aircraft.
            transform.position = aircraft.position;
        }
    }

    // Thanks to Rory Driscoll
    // http://www.rorydriscoll.com/2016/03/07/frame-rate-independent-damping-using-lerp/
    /// <summary>
    /// Creates dampened motion between a and b that is framerate independent.
    /// </summary>
    /// <param name="a">Initial parameter</param>
    /// <param name="b">Target parameter</param>
    /// <param name="lambda">Smoothing factor</param>
    /// <param name="dt">Time since last damp call</param>
    /// <returns></returns>
    private Quaternion Damp(Quaternion a, Quaternion b, float lambda, float dt)
    {
        return Quaternion.Slerp(a, b, 1 - Mathf.Exp(-lambda * dt));
    }

    private void OnDrawGizmos()
    {
        if (showDebugInfo)
        {
            Color oldColor = Gizmos.color;

            // Draw the boresight position.
            if (aircraft != null)
            {
                Gizmos.color = Color.white;
                Gizmos.DrawWireSphere(BoresightPos, 10f);
            }

            if (mouseAim != null)
            {
                // Draw the position of the mouse aim position.
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(MouseAimPos, 10f);

                // Draw axes for the mouse aim transform.
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(mouseAim.position, mouseAim.forward * 50f);
                Gizmos.color = Color.green;
                Gizmos.DrawRay(mouseAim.position, mouseAim.up * 50f);
                Gizmos.color = Color.red;
                Gizmos.DrawRay(mouseAim.position, mouseAim.right * 50f);
            }

            Gizmos.color = oldColor;
        }
    }
}
