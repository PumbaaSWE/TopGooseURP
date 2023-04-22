using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    public delegate void OnDeadEvent();
    public OnDeadEvent OnDead;

    public delegate void OnChangeHealthEvent(float change, ChangeHealthType damageType, TeamMember damager);
    public OnChangeHealthEvent OnChangeHealth;

    public float Amount { get; private set; }
    public bool Dead { get; private set; }

    [SerializeField]
    private float startHealth;


    public void ChangeHealth(float change, ChangeHealthType damageType , TeamMember damager)
    {
        Amount += change;
        OnChangeHealth?.Invoke(change, damageType, damager);

        if (Amount > startHealth) Amount = startHealth;

        if (Amount <= 0 && !Dead)
        {
            OnDead?.Invoke();
            Dead = true;
        }
    }

    public void Reset()
    {
        Amount = startHealth;
        Dead = false;
    }

    private void Awake()
    {
        Amount = startHealth;
    }
}

public enum ChangeHealthType
{
    bullet,
    fire,
    explosion,
    impact,
    regeneration
}