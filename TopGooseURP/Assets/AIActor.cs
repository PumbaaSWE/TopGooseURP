using System;
using System.Collections.Generic;
using UnityEngine;

public class AIActor : MonoBehaviour
{

    public List<IUtility> utilities = new List<IUtility>();
    private int previusUtil = 0;
    private int currentUtil = 0;

    void Start()
    {
        //auto fill list based on attached components implementing IUtility?
        Component[] utilities = GetComponents(typeof(IUtility));

        for (int i = 0; i < utilities.Length; i++)
        {
            this.utilities.Add(utilities[i] as IUtility);
        }
    }

    void FixedUpdate()
    {
        if (utilities.Count == 0) return;

        //evaluate every frame needed?
        EvaluateUtils();

        utilities[currentUtil].Execute();
    }

    private void EvaluateUtils()
    {
        //Evaluating every frame is too often?
        float maxScore = -1;
        for (int i = 0; i < utilities.Count; i++)
        {
            float score = utilities[i].Evaluate();
            if (score > maxScore)
            {
                currentUtil = i;
                maxScore = score;
            }
        }
        if (currentUtil != previusUtil)
        {
            //possible cleanup/startup calls?
            previusUtil = currentUtil;
        }
    }

    public bool OfferRole(CombatCoordinator.Role role)
    {
        throw new NotImplementedException();
    }
}
