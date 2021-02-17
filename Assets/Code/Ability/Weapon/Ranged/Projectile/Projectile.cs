using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour, IPoolable
{
    public GameObject GameObject => gameObject;

    //[SerializeField] private Rigidbody rigidbodyComp;
    //public Rigidbody RigidbodyComp { get => rigidbodyComp; set => rigidbodyComp = value; }

    [SerializeField] private ProjectileAbility abilityOriginComp;
    public ProjectileAbility AbilityOriginComp { get => abilityOriginComp; set => abilityOriginComp = value; }

    [SerializeField] private float modifierValue;
    public float ModifierValue { get => modifierValue; set => modifierValue = value; }

    [SerializeField] private float range;
    public float Range { get => range; set => range = value; }

    [SerializeField] private float maxVelocity;
    public float MaxVelocity { get => maxVelocity; set => maxVelocity = value; }

    [SerializeField] private Vector3 fireOriginPoint;
    public Vector3 FireOriginPoint { get => fireOriginPoint; set => fireOriginPoint = value; }

    [SerializeField] private string ownerTag;
    public string OwnerTag { get => ownerTag; set => ownerTag = value; }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * maxVelocity * Time.deltaTime;
        if (Vector3.Distance(fireOriginPoint, transform.position)>= Range)
        {
            Repool();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        HealthComponent hitObject = other.GetComponent<HealthComponent>();
        if (hitObject != null)
        {
            hitObject.TakeDamage(Mathf.Abs(modifierValue), out HealthChangeInfo output);
        }
        PlayImpactEffects();
        Repool();
    }

    private void PlayImpactEffects()
    {
        //Debug.Log("TODO: PLAY IMPLACT FX");
    }

    public void Repool()
    {
        GameAssetManager.Instance.ProjectileResourcePool.ReturnToPool(this);
    }
    private void OnDisable()
    {
        ownerTag = "";
    }
}
