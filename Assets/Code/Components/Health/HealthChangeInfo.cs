using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct HealthChangeInfo
{
	public HealthComponent HealthComp;

	public float OldHealth;

	public float NewHealth;

	public float HealthDifference
	{
		get { return NewHealth - OldHealth; }
	}

	public float AbsHealthDifference
	{
		get { return Mathf.Abs(HealthDifference); }
	}
}
