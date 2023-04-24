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
    Color[] startColor;

    // Start is called before the first frame update
    void Start()
    {
        health = GetComponent<Health>();

        if(removeWhenDead)
        health.OnDead += RemoveOnDead;
        health.OnChangeHealth += OnChangeHealth;
        health.OnDamage += OnDamageHealth;

        Renderer[] renderer = GetComponentsInChildren<Renderer>();

        startColor = new Color[renderer.Length];
        for (int i = 0; i < renderer.Length; i++)
        {
            startColor[i] = renderer[i].material.color;
        }
    }

    public void RemoveOnDead()
    {
        Destroy(gameObject);
    }

    public void OnChangeHealth(float change, ChangeHealthType changeHealthType, TeamMember _)
    {
        if(change < 0)
        {
            // make it red?
            Invoke("RedBlink", 0f);
            Invoke("TurnBackBlink", 0.05f);
        }
    }

    public void OnDamageHealth(DamageInfo info, float actualDamage)
    {
        if (actualDamage < 0)
        {
            // make it red?
            Invoke("RedBlink", 0f);
            Invoke("TurnBackBlink", 0.05f);
        }
    }

    public void RedBlink()
    {
        Renderer[] renderer = GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderer.Length; i++)
        {
            renderer[i].material.color = Color.red;
        }
    }
    public void TurnBackBlink()
    {
        Renderer[] renderer = GetComponentsInChildren<Renderer>();
        for(int i = 0; i < renderer.Length; i++)
        {
            renderer[i].material.color = startColor[i];
        }
    }

}
