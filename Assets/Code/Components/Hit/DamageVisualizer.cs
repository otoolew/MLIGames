using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageVisualizer : MonoBehaviour
{

    [SerializeField] private HealthComponent healthComp;
    public HealthComponent HealthComp { get => healthComp; set => healthComp = value; }

    [SerializeField] private Vector3 spawnOffset;
	public Vector3 SpawnOffset { get => spawnOffset; set => spawnOffset = value; }

	protected virtual void Awake()
	{
		if (HealthComp != null)
		{
			AssignHealthComponent(HealthComp);
		}
	}

	/// <summary>
	/// Sets the visibility status of this visualiser
	/// </summary>
	public void SetVisible(bool visible)
	{
		gameObject.SetActive(visible);
	}

	/// <summary>
	/// Assigns the damageable, subscribing to the damaged event
	/// </summary>
	/// <param name="damageable">Damageable to assign</param>
	public void AssignHealthComponent(HealthComponent healthComp)
	{
		if (HealthComp != null)
		{
			HealthComp.HealthChanged -= OnHealthChanged;
		}
		HealthComp = healthComp;
		HealthComp.HealthChanged += OnHealthChanged;
	}

	void OnHealthChanged(HealthChangeInfo healthChangeInfo)
	{
		ActivatePopUpText(Mathf.Abs(healthChangeInfo.HealthDifference));
	}
	public void ActivatePopUpText(string value)
	{
		PopUpText popup = GameAssetManager.Instance.PopUpPool.FetchFromPool();
		popup.gameObject.SetActive(true);
		popup.ChangeText(value);
		popup.transform.position = transform.position + spawnOffset;
	}

	public void ActivatePopUpText(float value)
	{
		int val = (int)value;
		PopUpText popup = GameAssetManager.Instance.PopUpPool.FetchFromPool();
		popup.gameObject.SetActive(true);
		popup.ChangeText(val, Color.red);
		popup.transform.position = transform.position + spawnOffset;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.cyan;
		Gizmos.DrawIcon(transform.position + spawnOffset, "PopUp Spawn");
	}
}
