using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class Turret : MonoBehaviour
{
    [SerializeField]
    Transform target;
    [SerializeField]
    Transform barrel;
    [SerializeField]
    Transform canon;

    [SerializeField]
    float range;

    [SerializeField]
    [Range(0, 90)]
    float maxAngleUp;

    [SerializeField]
    [Range(0, 90)]
    float maxAngleDown;


    [SerializeField]
    [Range(0, 180)]
    float maxAngleLeft;

    [SerializeField]
    [Range(0, 180)]
    float maxAngleRight;

    [SerializeField]
    [Range(0, 1)]
    float turnSpeed;

    [SerializeField]
    BulletData bulletData;



    Gun gun;

    Rigidbody targetRb;
    private void Start()
    {
        targetRb = target.GetComponent<Rigidbody>();
        gun = GetComponentInChildren<Gun>();
        gun.Fire = true;

        GetComponent<Health>().OnDead += () =>
        {
            gun.Fire = false;
            enabled = false;
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {

            return;
        }
        if (Vector3.Distance(target.position, transform.position) < range)
        {
            TargetingMath.ComputeImpact(target.position, targetRb.velocity, transform.position, gun.BulletSpeed, out Vector3 location, out float _);

            Vector3 direction = location - barrel.position;
            Quaternion rotation = Quaternion.LookRotation(direction.normalized);

            Debug.DrawRay(transform.position, direction * 10000, Color.yellow);

            float startRotationY = transform.rotation.eulerAngles.y;
            float xRotation = ClampUpDown(rotation.eulerAngles.x, maxAngleUp, maxAngleDown);
            float yRotation = ClampLeftRight(rotation.eulerAngles.y, maxAngleLeft, maxAngleRight);

            float xRotationLeft = barrel.localRotation.eulerAngles.x - xRotation;
            float yRotationLeft = barrel.localRotation.eulerAngles.y - yRotation;

            float evenX = Mathf.Abs(xRotation / xRotationLeft);
            float evenY = Mathf.Abs(yRotation / yRotationLeft);

            xRotation = Mathf.LerpAngle(barrel.localRotation.eulerAngles.x, xRotation, turnSpeed * evenX);
            yRotation = Mathf.LerpAngle(canon.localRotation.eulerAngles.y, yRotation, turnSpeed * evenY);



            Quaternion barrelRotation = Quaternion.Euler(xRotation, 0, 0);
            Quaternion canonRotation = Quaternion.Euler(0, yRotation, 0);

            barrel.localRotation = barrelRotation;
            canon.localRotation = canonRotation;

            Quaternion fullRotation = Quaternion.Euler(xRotation, yRotation, 0);

            float rotationCloseness = Vector3.Dot(fullRotation.eulerAngles.normalized, rotation.eulerAngles.normalized);

            Debug.Log(rotationCloseness);
            if (rotationCloseness >= 1f - 0.001f)
            {
                gun.Fire = true;
            }
            else
            {
                gun.Fire = false;
            }

        }
        else
        {
            gun.Fire = false;
        }
    }



    private float ClampLeftRight(float yRotation, float maxLeft, float maxRight)
    {
        if (yRotation >= 0 && yRotation <= 180) // right side
        {
            yRotation = Mathf.Clamp(yRotation, 0, maxRight);
        }
        else if (yRotation >= 180 && yRotation <= 360) // left side
        {
            yRotation = Mathf.Clamp(yRotation, 360 - maxLeft, 360);
        }

        return yRotation;
    }

    private float ClampUpDown(float xRotation, float maxUp, float maxDown)
    {
        if (xRotation >= 270 && xRotation <= 360) // up
        {
            xRotation = Mathf.Clamp(xRotation, 360 - maxUp, 360);
        }
        else if (xRotation >= 0 && xRotation <= 90) // down
        {
            xRotation = Mathf.Clamp(xRotation, 0, maxDown);
        }

        return xRotation;
    }

    bool freezeDebugRays = false;
    float startRotationRad;

    Vector3 debugDirectionUp;
    Vector3 debugDirectionDown;
    Vector3 debugDirectionLeft;
    Vector3 debugDirectionRight;

    Quaternion lastRotation;
    Vector3 lastPosition;

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            if (lastRotation != transform.rotation)
            {
                freezeDebugRays = false;
                lastRotation = transform.rotation;
                float startRotationY = barrel.rotation.eulerAngles.y;
                startRotationRad = (startRotationY - 90) / 180 * Mathf.PI;
            }
            else if (lastPosition != transform.position)
            {
                freezeDebugRays = false;
                lastPosition = transform.position;
            }
        }

        float rayLength = 10000;

        float angleUp = maxAngleUp / 180f * Mathf.PI;
        float angleDown = maxAngleDown / 180f * Mathf.PI;
        float angleLeft = maxAngleLeft / 180f * Mathf.PI;
        float angleRight = maxAngleRight / 180f * Mathf.PI;

        if (!freezeDebugRays)
        {
            Vector3 start = new Vector3(Mathf.Cos(-startRotationRad), 0, Mathf.Sin(-startRotationRad));
            //Gizmos.DrawRay(barrel.position, start * rayLength);
            debugDirectionUp = new Vector3(Mathf.Cos(angleUp) * start.x, Mathf.Sin(angleUp), Mathf.Cos(angleUp) * start.z);
            debugDirectionDown = new Vector3(Mathf.Cos(-angleDown) * start.x, Mathf.Sin(-angleDown), Mathf.Cos(angleDown) * start.z);
            debugDirectionLeft = new Vector3(Mathf.Cos(angleLeft - startRotationRad), 0, Mathf.Sin(angleLeft - startRotationRad));
            debugDirectionRight = new Vector3(Mathf.Cos(-angleRight - startRotationRad), 0, Mathf.Sin(-angleRight - startRotationRad));
        }

        Gizmos.DrawRay(barrel.position, debugDirectionUp * rayLength);
        Gizmos.DrawRay(barrel.position, debugDirectionDown * rayLength);
        Gizmos.DrawRay(barrel.position, debugDirectionLeft * rayLength);
        Gizmos.DrawRay(barrel.position, debugDirectionRight * rayLength);

        freezeDebugRays = true;
    }

    private void OnValidate()
    {
        freezeDebugRays = false;
        float startRotationY = barrel.rotation.eulerAngles.y;
        startRotationRad = (startRotationY - 90) / 180 * Mathf.PI;
    }
}
