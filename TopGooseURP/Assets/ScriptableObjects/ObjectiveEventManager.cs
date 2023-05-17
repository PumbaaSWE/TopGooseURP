using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "ObjectiveEventManager", menuName = "ScriptableObject/ObjectiveEventManager")]
public class ObjectiveEventManager : ScriptableObject
{

    public UnityEvent<Objective> ObjectiveCompleted = new();
    public UnityEvent<bool> AllPrimaryCompleted = new();
    public UnityEvent<bool> AllSecondaryCompleted = new();

    private readonly List<Objective> primaries = new();
    private int primaryCount;
    private readonly List<Objective> secondaries = new();
    private int secondaryCount;

    public int PrimaryCount => primaryCount;
    public int PrimaryTotal => primaries.Count;
    public int SecondaryCount => secondaryCount;
    public int SecondaryTotal => secondaries.Count;

    private readonly List<Objective> displayed = new();

    public void OnEnable()
    {
        //if(ObjectiveCompleted == null) ObjectiveCompleted = new UnityEvent<Objective>(); // ??= not supported according to Unity website
        //AllPrimaryCompleted ??= new UnityEvent<bool>();
        //AllSecondaryCompleted ??= new UnityEvent<bool>();
        primaries.Clear();
        secondaries.Clear();
        primaryCount = 0;
        secondaryCount = 0;
        displayed.Clear();
    }

    public void RegisterObjective(Objective objective)
    {
        if (objective.IsPrimary)
        {
            if (primaries.Contains(objective))
            {
                Debug.LogWarning("Trying to register alredy registered Primary Objective: " + objective.Title);
                return;
            }
            primaries.Add(objective);
            primaryCount++;
        }
        else
        {
            if (secondaries.Contains(objective))
            {
                Debug.LogWarning("Trying to register already registered Secondary Objective: " + objective.Title);
                return;
            }
            secondaries.Add(objective);
            secondaryCount++;
        }

        if (objective.ShownOnMap)
        {
            displayed.Add(objective);
        }
    }

    public void Completed(Objective objective)
    {
        if(ObjectiveCompleted != null)
            ObjectiveCompleted?.Invoke(objective);

        if (objective.IsPrimary)
        {
            if (!primaries.Contains(objective)) //not very fast.. keep flag in Objective?
            {
                Debug.LogWarning("Trying to Complete unregistered Primary Objective: " + objective.Title);
                return;
            }
            primaryCount--;
            if(primaryCount <= 0)
            {
                AllPrimaryCompleted?.Invoke(true);
            }
        }
        else
        {
            if (!secondaries.Contains(objective)) //not very fast..  keep flag in Objective?
            {
                Debug.LogWarning("Trying to Complete unregistered Secondary Objective: " + objective.Title);
                return;
            }

            secondaryCount--;
            if (secondaryCount <= 0)
            {
                AllSecondaryCompleted?.Invoke(true);
            }
        }
    }

    public List<Objective> GetDisplayed()
    {
        return displayed;
    }

    internal void Complete(ObjectiveEvent objectiveEvent)
    {
        
    }
}
