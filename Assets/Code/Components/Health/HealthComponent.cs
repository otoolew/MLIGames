using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthComponent : MonoBehaviour
{
    #region Components

    #endregion

    [SerializeField] private float maxHealth;
    public float MaxHealth { get => maxHealth; set => maxHealth = value; }

    [SerializeField] private float currentHealth;
    public float CurrentHealth { get => currentHealth; set => currentHealth = value; }

    [SerializeField] private bool isDead;
    public bool IsDead { get => isDead; set => isDead = value; }

    public UnityAction DeathAction;
    public UnityEvent<float> OnHealthChange;
    // Start is called before the first frame update
    void Start()
    {
        //if(OnHealthChange == null)
        //{
        //    OnHealthChange = new UnityEvent<float>();
        //}
        //OnHealthChange = new UnityEvent<float>();
    }

    public void ApplyHealthChange(float value)
    {
        //Debug.Log(transform.root.name + " Apply Health Hit");
        if (isDead)
        {
            return;
        }

        OnHealthChange?.Invoke(value);

        currentHealth += value;
        if (CurrentHealth <= 0)
        {
            isDead = true;
            DeathAction?.Invoke();
        }
    }

    public void ResetHealth()
    {
        currentHealth = MaxHealth;
    }
}
