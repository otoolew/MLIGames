using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthPanel : MonoBehaviour
{
    #region Components
    [SerializeField] private HealthComponent healthComp;
    public HealthComponent HealthComp { get => healthComp; set => healthComp = value; }

    [SerializeField] private Slider healthSlider;
    public Slider HealthSlider { get => healthSlider; set => healthSlider = value; }
    #endregion

    public void RegisterHealthComponent(HealthComponent health)
    {
        healthComp = health;
        healthSlider.minValue = 0;
        healthSlider.maxValue = healthComp.MaxHealth;
        healthComp.OnHealthChange.AddListener(ApplyHealthChange);
    }

    private void ApplyHealthChange(float value)
    {
        healthSlider.value = healthComp.CurrentHealth;
    }

}
