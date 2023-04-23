using System;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public delegate void OnDeadEvent();
    public OnDeadEvent OnDead;

    public delegate void OnChangeHealthEvent(float change, ChangeHealthType damageType, TeamMember damager);
    public OnChangeHealthEvent OnChangeHealth;

    public delegate void OnDamageEvent(DamageInfo info, float actualDamage);
    public OnDamageEvent OnDamage;

    public List<DamageModsPair> damageModifier = new();
    private readonly Dictionary<DamageType, float> damageMods = new(); 
    public float Amount { get; private set; }
    public bool Dead { get; private set; }

    [SerializeField]
    private float startHealth = 100;


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


    public void DealDamage(DamageInfo info)
    {
        if (Dead) return;
        float damage = info.amount;
        if (damageMods.TryGetValue(info.type, out float mod)){
            damage *= mod;
        }
        damage = Mathf.Min(damage, Amount); // deal no more damage than there is hp
        Amount -= damage;
        OnDamage?.Invoke(info, damage);
        OnChangeHealth?.Invoke(-info.amount, ChangeHealthType.bullet, info.dealer);
        if(Amount <= 0.0f)
        {
            OnDead?.Invoke();
            Dead = true;
        }
    }

    private void OnValidate()
    {
        damageMods.Clear();
        for (int i = 0; i < damageModifier.Count; i++)
        {
            if (damageModifier[i].type != null)
                damageMods.TryAdd(damageModifier[i].type, damageModifier[i].modifier);
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
        for (int i = 0; i < damageModifier.Count; i++)
        {
            if(damageModifier[i].type != null)
                damageMods.TryAdd(damageModifier[i].type, damageModifier[i].modifier);
        }
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

[Serializable]
public class DamageModsPair
{
    public DamageType type;
    public float modifier;
}