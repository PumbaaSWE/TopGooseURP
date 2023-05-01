using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ThreatDisplay : MonoBehaviour
{
    [SerializeField]
    GameObject arrow;

    [SerializeField]
    SphereCollider sphereCollider;

    [SerializeField]
    float detectionRadius;

    [SerializeField]
    LayerMask detectLayer;

    [SerializeField]
    string detectLayerName;

    [SerializeField]
    Transform rig;

    List<Arrow> arrows = new List<Arrow>();

    private void Start()
    {
        sphereCollider.radius = detectionRadius;
    }

    private void OnTriggerEnter(Collider other)
    {
        //Vill göra
        //if (other.gameObject.layer == detectLayer)
        //men det går inte av någon anledning? "Layer index out of bounds"

        if (other.gameObject.layer == LayerMask.NameToLayer(detectLayerName))
        {
            for (int i = 0; i < arrows.Count; i++)
            {
                if (arrows[i].target == other.gameObject.transform)
                    return;
            }

            var arrowObject = Instantiate(this.arrow);
            arrowObject.transform.parent = rig;

            var arrow = arrowObject.GetComponent<Arrow>();
            arrow.target = other.gameObject.transform;

            arrows.Add(arrow);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        for (int i = 0; i < arrows.Count; i++)
        {
            if (arrows[i].target == other.gameObject.transform)
            {
                Destroy(arrows[i].gameObject);
                arrows.RemoveAt(i);
            }
        }
    }
}
