using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthScript : MonoBehaviour
{

    public Slider healthBar;
    public Gradient HP;
    public Image fillColor;
    public void setHealth(int health)
    {
        healthBar.value = health;
        fillColor.color = HP.Evaluate(healthBar.normalizedValue);
    }

    public void maxHealth(int health)
    {
        healthBar.maxValue = health;
        healthBar.value = health;
        fillColor.color = HP.Evaluate(1f);
    }
}
