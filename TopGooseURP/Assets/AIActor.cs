using System.Collections.Generic;
using UnityEngine;

public class AIActor : MonoBehaviour
{

    [SerializeField]private readonly List<IUtility> utilities = new();
    private int previusUtil = 0;
    private int currentUtil = 0;

    void Start()
    {
        //auto fill list based on attached components implementing IUtility?
        for (int i = 0; i < utilities.Count; i++)
        {
            utilities[i].AddGameObject(gameObject);
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
}
