using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    [SerializeField] private Slider slider;
    [Space]
    [SerializeField] private TextMeshProUGUI healthText;


    private float maxHealth;

    public void SetMaxHealth(float health)
    {
        slider.maxValue = health;
        slider.value = health;
        maxHealth = health;
    }

    public void SetHealth(float health)
    {
        slider.value = health;
    }

    private void Update()
    {
        healthText.text = slider.value + "/" + maxHealth;
    }
}
