using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class TestHealthConnection : MonoBehaviour
{
    private Health health;
    [SerializeField]
    bool removeWhenDead;
    Color startColor;

    // Start is called before the first frame update
    void Start()
    {
        health = GetComponent<Health>();

        if(removeWhenDead)
        health.OnDead += RemoveOnDead;
        health.OnChangeHealth += OnChangeHealth;

        startColor = GetComponent<Renderer>().material.color;
    }

    public void RemoveOnDead()
    {
        Destroy(gameObject);
    }

    public void OnChangeHealth(float change, ChangeHealthType changeHealthType)
    {
        if(change < 0)
        {
            // make it red?
            Invoke("RedBlink", 0f);
            Invoke("TurnBackBlink", 0.05f);
        }
    }

    public void RedBlink()
    {
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.color = Color.red;
    }
    public void TurnBackBlink()
    {
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.color = startColor;
    }

}
