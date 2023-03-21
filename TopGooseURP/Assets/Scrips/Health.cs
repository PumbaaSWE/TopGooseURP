using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    public delegate void OnDeadEvent();
    OnDeadEvent OnDead;

    public delegate void OnChangeHealthEvent(float change);
    OnChangeHealthEvent OnChangeHealth;

    //[NonSerialized]
    public float health;

    [NonSerialized]
    public bool dead;

    public float startHealth;



    public void AddDeathEvent(OnDeadEvent deadEvent)
    {
        OnDead += deadEvent;
    }
    public void RemoveDeathEvent(OnDeadEvent deadEvent)
    {
        OnDead -= deadEvent;
    }
    public void AddChangeHealthEvent(OnChangeHealthEvent changeHealthEvent)
    {
        OnChangeHealth += changeHealthEvent;
    }
    public void RemoveChangeHealthEvent(OnChangeHealthEvent changeHealthEvent)
    {
        OnChangeHealth -= changeHealthEvent;
    }

    public void ClearDeathEvents()
    {
        OnDead = null;
    }

    public void Refresh()
    {
        health = startHealth;
        dead = false;
    }

    public void ChangeHealth(float change)
    {
        health += change;
        OnChangeHealth?.Invoke(change);

        if (health > startHealth) health = startHealth;
    }

    private void Awake()
    {
        health = startHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if(health <= 0 && !dead)
        {
            OnDead?.Invoke();
            dead = true;
        }
    }
}
