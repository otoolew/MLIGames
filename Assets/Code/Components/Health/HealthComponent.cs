using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthComponent : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    public float MaxHealth { get => maxHealth; set => maxHealth = value; }

    [SerializeField] private float currentHealth;
    public float CurrentHealth { get => currentHealth; set => currentHealth = value; }

	public float NormalisedHealth
	{
		get
		{
			if (Math.Abs(maxHealth) <= Mathf.Epsilon)
			{
				Debug.LogError("Max Health is 0");
				maxHealth = 1f;
			}
			return currentHealth / maxHealth;
		}
	}

	public bool IsAtMaxHealth
	{
		get { return Mathf.Approximately(currentHealth, maxHealth); }
	}

	public bool IsDead { get { return currentHealth <= 0f; } }

    public UnityAction DeathAction;
    public UnityEvent<float> OnHealthChange;

	public UnityAction ReachedMaxHealth;
	public UnityAction<HealthChangeInfo> HealthChanged;
	public UnityAction<HealthChangeInfo> Damaged;
	public UnityAction<HealthChangeInfo> Healed;
	public UnityAction<HealthChangeInfo> Died;

	// Start is called before the first frame update
	void Start()
    {
		currentHealth = maxHealth;
    }

    //public void ApplyHealthChange(float value)
    //{
    //    //Debug.Log(transform.root.name + " Apply Health Hit");
    //    if (IsDead)
    //    {
    //        return;
    //    }

    //    OnHealthChange?.Invoke(value);

    //    currentHealth += value;
    //    if (IsDead)
    //    {
    //        DeathAction?.Invoke();
    //    }
    //}
	/// <summary>
	/// Sets the max health and starting health to the same value
	/// </summary>
	public void SetMaxHealth(float health)
	{
		if (health <= 0)
		{
			return;
		}
		maxHealth = currentHealth = health;
	}
	/// <summary>
	/// Sets the max health and starting health separately
	/// </summary>
	public void SetMaxHealth(float health, float startingHealth)
	{
		if (health <= 0)
		{
			return;
		}
		maxHealth = health;
		this.currentHealth = startingHealth;
	}
	/// <summary>
	/// Sets this instance's health directly.
	/// </summary>
	/// <param name="health">
	/// The value to set <see cref="currentHealth"/> to
	/// </param>
	public void SetHealth(float health)
	{
		var info = new HealthChangeInfo
		{
			HealthComp = this,
			NewHealth = health,
			OldHealth = currentHealth
		};

		currentHealth = health;

		if (HealthChanged != null)
		{
			HealthChanged(info);
		}
	}
    
	/// <summary>
	/// Use the alignment to see if taking damage is a valid action
	/// </summary>
	/// <param name="damage">
	/// The damage to take
	/// </param>
	/// <param name="damageAlignment">
	/// The alignment of the other combatant
	/// </param>
	/// <param name="output">
	/// The output data if there is damage taken
	/// </param>
	/// <returns>
	/// <value>true if this instance took damage</value>
	/// <value>false if this instance was already dead, or the alignment did not allow the damage</value>
	/// </returns>
	public bool TakeDamage(float damage, out HealthChangeInfo output)
	{
		output = new HealthChangeInfo
		{
			HealthComp = this,
			NewHealth = currentHealth,
			OldHealth = currentHealth
		};

		if (IsDead)
		{
			return false;
		}

		ChangeHealth(-damage, output);
		SafelyDoAction(Damaged, output);
		if (IsDead)
		{
			SafelyDoAction(Died, output);
		}
		return true;
	}

	/// <summary>
	/// Logic for increasing the health.
	/// </summary>
	/// <param name="health">Health.</param>
	public HealthChangeInfo IncreaseHealth(float health)
	{
		var info = new HealthChangeInfo { HealthComp = this };
		ChangeHealth(health, info);
		SafelyDoAction(Healed, info);
		if (IsAtMaxHealth)
		{
			SafelyDoAction(ReachedMaxHealth);
		}

		return info;
	}

	/// <summary>
	/// Changes the health.
	/// </summary>
	/// <param name="healthIncrement">Health increment.</param>
	/// <param name="info">HealthChangeInfo for this change</param>
	protected void ChangeHealth(float healthIncrement, HealthChangeInfo info)
	{
		info.OldHealth = currentHealth;
		currentHealth += healthIncrement;
		currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
		info.NewHealth = currentHealth;

		if (HealthChanged != null)
		{
			HealthChanged(info);
		}
	}

	/// <summary>
	/// A helper method for null checking actions
	/// </summary>
	/// <param name="action">Action to be done</param>
	protected void SafelyDoAction(UnityAction action)
	{
        action?.Invoke();
    }

	/// <summary>
	/// A helper method for null checking actions
	/// </summary>
	/// <param name="action">Action to be done</param>
	/// <param name="info">The HealthChangeInfo to be passed to the Action</param>
	protected void SafelyDoAction(UnityAction<HealthChangeInfo> action, HealthChangeInfo info)
	{
        action?.Invoke(info);
    }
}
