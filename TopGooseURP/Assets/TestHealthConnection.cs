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
    Renderer[] renderers;

    // Start is called before the first frame update
    void Start()
    {
        health = GetComponent<Health>();

        if (removeWhenDead)
            health.OnDead += RemoveOnDead;
        health.OnDead += TurnBackBlinkOnDead;
        health.OnChangeHealth += OnChangeHealth;
        health.OnDamage += OnDamageHealth;

        renderers = GetComponentsInChildren<Renderer>();

        startColor = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            startColor[i] = renderers[i].material.color;
        }
    }

    private void TurnBackBlinkOnDead()
    {
        Invoke("TurnBackBlink", 0f);
    }

    public void RemoveOnDead()
    {
        CancelInvoke(nameof(RedBlink));
        CancelInvoke(nameof(TurnBackBlink));
        Destroy(gameObject);
    }

    public void OnChangeHealth(float change, ChangeHealthType changeHealthType, TeamMember _)
    {
        if (change < 0)
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
        //renderers = GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = Color.red;
        }
    }
    public void TurnBackBlink()
    {
        //renderers = GetComponentsInChildren<Renderer>();
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = startColor[i];
        }
    }

}