using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCircle : MonoBehaviour
{
    LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        DrawCircle1(25, 40);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DrawCircle1(int steps, float radius)
    {
        lineRenderer.positionCount = steps;
        lineRenderer.loop = true;
        for (int i = 0; i < steps; i++)
        {
            float circumferenceProgress = (float)i / steps;

            float currentRadian = circumferenceProgress * Mathf.PI * 2;

            float x = Mathf.Cos(currentRadian) * radius;
            float y = Mathf.Sin(currentRadian) * radius;

            Vector3 currentPos = new(x, y, 0);
            lineRenderer.SetPosition(i, currentPos);
        }
    }
}
