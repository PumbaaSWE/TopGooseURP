using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Health health;
    [Space]
    [SerializeField] private Image circleHp;
    [SerializeField] private TextMeshProUGUI hpText;

    private float maxHealth, currentHP;

    private void Start()
    {
        SetMaxHealth(health.Amount);
        health.OnChangeHealth += SetHealth;
    }
    public void SetMaxHealth(float health)
    {
        maxHealth = health;
        currentHP = health;
        circleHp.fillAmount = currentHP / maxHealth;
    }

    public void SetHealth(float change, ChangeHealthType damageType, TeamMember damager)
    {
        currentHP += change;

        circleHp.fillAmount = currentHP / maxHealth;
    }

    private void Update()
    {
        hpText.text = (circleHp.fillAmount * 100).ToString("n1") + "%";
    }
}
