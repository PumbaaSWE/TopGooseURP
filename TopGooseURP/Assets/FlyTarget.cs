using UnityEngine;

public class FlyTarget : MonoBehaviour
{

    public delegate void TargetHitEvent(FlyTarget flyTarget);
    public event TargetHitEvent OnTargetHit;

    private Collider target;
    private bool hasBeenHit;
    private MeshRenderer mr;
    [SerializeField]private Color originalColor;
    //public Color highlightColor;
    //public Color hitColor;

    // Start is called before the first frame update
    void Start()
    {
        target = GetComponent<Collider>();
        target.isTrigger = true;

        mr = GetComponent<MeshRenderer>();
        mr.material.color = originalColor;
        //mr.material.shader.
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasBeenHit)
        {
            //hasBeenHit = true;
            OnTargetHit?.Invoke(this);
            //gameObject.SetActive(false);
        }
    }

    public void ConfirmHit()
    {
        enabled = true;
        hasBeenHit = true;
        target.enabled = false;
        gameObject.SetActive(false);
    }

    public void ResetColor()
    {
        mr.material.color = originalColor;
    }

    public void SetColor(Color color)
    {
        mr.material.color = color;
    }
}
