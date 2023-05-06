using UnityEngine;
using UnityEngine.UI;

public class TargetHUDAim : MonoBehaviour
{

    [Tooltip("Used for all")][SerializeField] private Color tintColor = Color.white;
    [SerializeField] private Image aimPoint;
    [SerializeField] private Image dot1;
    [SerializeField] private Image dot2;
    [SerializeField] private Image dot3;
    bool noDots = false;
    bool display = false;

    /**
     * THESE NEEDS TO BE DYNAMIACALLY SET IN THE FUTURE!!!!
     */
    public TargetHUD owner;
    [Tooltip("FIX THIS")][SerializeField] private float bulletSpeed = 100;
    [Tooltip("FIX THIS")][SerializeField] private float flyRange = 500;

    Rigidbody targetBody;
    public Transform targetTransform;
    public string targetName;

    Camera playerCam;

    public Transform TrackedTransform => targetTransform;

    // Start is called before the first frame update
    void Start()
    {
        playerCam = Camera.main;
        if (playerCam == null)
            Debug.LogWarning(name + ": TargetHUDAim - No camera found on Camera.main!");

        if (dot1 == null || dot2 == null || dot3 == null)
        {
            noDots = true;
        }

        aimPoint.color = tintColor;
        if (!noDots)
        {
            dot1.color = tintColor;
            dot2.color = tintColor;
            dot3.color = tintColor;
        }

    }

    public void SetTracker(TargetHUD tracker, float bulletSpeed = 800, float flyRange = 500)
    {
        owner = tracker;
        this.bulletSpeed = bulletSpeed;
        this.flyRange = flyRange;
    }

    public void ActivateTracking(Transform targetTransform, Rigidbody targetBody)
    {
        this.targetBody = targetBody;
        this.targetTransform = targetTransform;
        targetName = targetTransform.name;
        //enabled = false;
    }

    public void DeactivateTracking()
    {
        //enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        Vector3 tVel = Vector3.zero;
        if(targetBody != null) tVel = targetBody.velocity;
        if(targetTransform == null)
        {
            owner.ReturnAndRemove(transform);
            return;
        }
        Vector3 tPos = targetTransform.position;

        Vector3 position = owner.transform.position;
        if (TargetingMath.ComputeImpact(tPos, tVel, position, bulletSpeed, out Vector3 impact, out float _))
        {

            Vector3 toImpact = (impact - position).normalized;
            Vector3 toAimPoint = toImpact * flyRange;
            float a = Vector3.Dot(toImpact, owner.transform.forward);

            if (a > .9f) a = 1.0f;
            else a = Mathf.InverseLerp(.6f, .9f, a);

            Color color = tintColor;
            color.a = a;

            //Debug.DrawLine(position, impact, Color.blue);

            aimPoint.color = color;

            display = true;


            toAimPoint = playerCam.WorldToScreenPoint(toAimPoint + owner.transform.position);
            aimPoint.transform.position = toAimPoint;

            Vector3 targetScreenPos = playerCam.WorldToScreenPoint(tPos);

            if (!noDots)
            {
                dot2.transform.position = (toAimPoint + targetScreenPos) / 2;
                dot1.transform.position = (dot2.transform.position + targetScreenPos) / 2 ;
                dot3.transform.position = (toAimPoint + dot2.transform.position) / 2;

                dot1.color = color;
                dot2.color = color;
                dot3.color = color;
            }


        }
        else
        {
            display = false;
        }

        aimPoint.gameObject.SetActive(display);
        if (!noDots)
        {
            dot1.gameObject.SetActive(display);
            dot2.gameObject.SetActive(display);
            dot3.gameObject.SetActive(display);
        }
    }
}
