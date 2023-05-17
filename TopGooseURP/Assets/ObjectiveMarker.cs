using UnityEngine;

public class ObjectiveMarker : MonoBehaviour
{

    Vector3 startPos;
    Vector3 endPos;
    [SerializeField] float bobDist = 5;
    [SerializeField] float bobSpeed = 5;

    float time = 0;


    // Start is called before the first frame update
    void Start()
    {
        //enabled = false;
        startPos = transform.position + bobDist / 2 * Vector3.down;
        endPos = transform.position + bobDist / 2 * Vector3.up;
    }

    // Update is called once per frame
    public void SetColor(Color color)
    {
        GetComponent<MeshRenderer>().material.color = color;
    }

    public void OnObjectiveComplete()
    {
        Destroy(gameObject);
    }

    void Update()
    {
        transform.position = Vector3.Lerp(startPos, endPos, (Mathf.Sin(Time.time)+1)*0.5f);
    }
}
