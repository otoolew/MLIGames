using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthVisualizer : MonoBehaviour
{

    [SerializeField] private HealthComponent healthComp;
    public HealthComponent HealthComp { get => healthComp; set => healthComp = value; }

    /// <summary>
    /// The object whose X-scale we change to decrease the health bar. Should have a default uniform scale
    /// </summary>
    public Transform HealthBar;

	/// <summary>
	/// The object whose X-scale we change to increase the health bar background. Should have a default uniform scale
	/// </summary>
	public Transform BackgroundBar;

	/// <summary>
	/// Whether to show this health bar even when it is full
	/// </summary>
	public bool ShowWhenFull;

	/// <summary>
	/// Camera to face the visualization at
	/// </summary>
	protected Transform CameraToFace;

	/// <summary>
	/// Updates the visualization of the health
	/// </summary>
	/// <param name="normalizedHealth">Normalized health value</param>
	public void UpdateHealth(float normalizedHealth)
	{
		Vector3 scale = Vector3.one;

		if (HealthBar != null)
		{
			scale.x = normalizedHealth;
			HealthBar.transform.localScale = scale;
		}

		if (BackgroundBar != null)
		{
			scale.x = 1 - normalizedHealth;
			BackgroundBar.transform.localScale = scale;
		}

		SetVisible(ShowWhenFull || normalizedHealth < 1.0f);
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

	/// <summary>
	/// Turns us to face the camera
	/// </summary>
	protected virtual void Update()
	{
		Vector3 direction = CameraToFace.transform.forward;
		transform.forward = -direction;
	}

	protected virtual void Start()
	{
		CameraToFace = UnityEngine.Camera.main.transform;
		AssignHealthComponent(HealthComp);
	}

	void OnHealthChanged(HealthChangeInfo healthChangeInfo)
	{
		Debug.Log("OnHealthChanged " + JsonUtility.ToJson(healthChangeInfo));
		UpdateHealth(HealthComp.NormalisedHealth);
	}
}
