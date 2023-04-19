using UnityEngine;

public class FlappyWingsModel : MonoBehaviour
{
    [SerializeField] private Transform left;
    [SerializeField] private Transform right;
    [SerializeField] private float flapSpeed = 45;
    [SerializeField] private float maxFlap = 90;


    [SerializeField] private SimpleFlight controller;
    [SerializeField] private Transform tail;
    [SerializeField] private float strengthPitch = 45;
    [SerializeField] private float strengthRoll = 40;

    bool flap;
    float angle;

    private Vector3 leftOriginal;
    private Vector3 rightOriginal;
    private Vector3 tailOriginal;

    internal void Flap(bool flap)
    {
        this.flap = flap;
    }

    // Start is called before the first frame update
    void Start()
    {
        if(TryGetComponent(out controller))
        {
            //do smth
        }
        leftOriginal = left.localEulerAngles;
        rightOriginal = right.localEulerAngles;
        tailOriginal = tail.localEulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        if(controller.LocalVelocity.z <= 0) return;
        angle += flapSpeed * Time.deltaTime;

        if (angle > maxFlap / 2) 
        {
            flapSpeed = -flapSpeed; 
            angle = maxFlap / 2;
        }

        if (angle < -maxFlap / 2)
        {
            flapSpeed = -flapSpeed;
            angle = -maxFlap / 2;
        }
        left.localEulerAngles = leftOriginal + new Vector3 (0 , angle, 0);
        right.localEulerAngles = rightOriginal + new Vector3(0 , -angle, 0);

        float pitch = controller.Steering.x * strengthPitch;
        float roll = controller.Steering.z * strengthRoll;

        Vector3 steer = new(pitch, 0, roll);
        tail.localEulerAngles = steer;
    }
}
