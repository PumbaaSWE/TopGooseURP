using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Health health;
    [Space]
    [SerializeField] private Image circleHp;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private float lerpTime;

    private float maxHealth, currentHP, lerpSpeed;

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
    }

    private void Update()
    {
        lerpSpeed = lerpTime * Time.deltaTime; 
        circleHp.fillAmount = Mathf.Lerp(circleHp.fillAmount, currentHP / maxHealth, lerpSpeed);
        hpText.text = (circleHp.fillAmount * 100).ToString("n1") + "%";
    }
}
