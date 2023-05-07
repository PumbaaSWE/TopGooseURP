using UnityEngine;

[CreateAssetMenu(fileName = "NewObjectiveEvent", menuName = "ScriptableObject/ObjectiveEvent")]

public class ObjectiveEvent : ScriptableObject
{
    public ObjectiveEventManager objectiveEventManager;
    public Objective objectivePrefab;
    private Objective objective;

    public void OnEnable()
    {
        //if(objective)
        objective = Instantiate(objectivePrefab);
    }

    public void CountOne()
    {
        Count(1);
    }

    public void Count(int value)
    {
        objective.Count(value);
    }
}
