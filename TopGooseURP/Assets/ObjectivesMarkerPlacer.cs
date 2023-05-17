using System.Collections.Generic;
using UnityEngine;

public class ObjectivesMarkerPlacer : MonoBehaviour
{

    [SerializeField] ObjectiveEventManager eventManager;
    List<Objective> objectives = new List<Objective>();
    List<Objective> markers = new List<Objective>();

    [SerializeField] ObjectiveMarker objectiveMarkerPrefab;
    [SerializeField] Color primary = Color.red;
    [SerializeField] Color secondary = Color.yellow;

    [SerializeField] private float offset = 50;

    // Start is called before the first frame update
    void Start()
    {
        objectives = eventManager.GetDisplayed();
        for (int i = 0; i < objectives.Count; i++)
        {
            ObjectiveMarker marker = Instantiate(objectiveMarkerPrefab, objectives[i].transform.position + Vector3.up * offset, objectiveMarkerPrefab.transform.rotation);
            if (objectives[i].IsPrimary)
                marker.SetColor(primary);
            else
                marker.SetColor(secondary);
            objectives[i].OnComplete += marker.OnObjectiveComplete;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
