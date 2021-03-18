using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour, IPoolable
{
    public GameObject GameObject => gameObject;
    
    [SerializeField] private Rigidbody rigidbodyComp;
    public Rigidbody RigidbodyComp { get => rigidbodyComp; set => rigidbodyComp = value; }

    [SerializeField] private ProjectileAbility ownerAbilityComp;
    public ProjectileAbility OwnerAbilityComp { get => ownerAbilityComp; set => ownerAbilityComp = value; }

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

    private void OnEnable()
    {
        transform.rotation = Quaternion.identity;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(fireOriginPoint, transform.position) >= Range)
        {
            Repool();
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.CompareTag(ownerTag))
            return;

        HealthComponent hitObject = collision.collider.GetComponent<HealthComponent>();
        if (hitObject != null)
        {
            Debug.Log("hitObject.CompareTag(ownerTag) " + hitObject.CompareTag(ownerTag));
            Debug.Log("HealthComponent " + hitObject.name + " Tag: " + hitObject.tag);
            hitObject.TakeDamage(Mathf.Abs(modifierValue), out HealthChangeInfo output);
        }

        PlayImpactEffects();
        Repool();
    }
    //private void OnTriggerEnter(Collider other)
    //{
    //    Debug.Log("OnTriggerEnter " + other.name + " Tag: " + other.tag);
    //    if (other.CompareTag(ownerTag))
    //        return;

    //    HealthComponent hitObject = other.GetComponent<HealthComponent>();
    //    if (hitObject != null)
    //    {
    //        Debug.Log("hitObject.CompareTag(ownerTag) " + hitObject.CompareTag(ownerTag));
    //        Debug.Log("HealthComponent " + hitObject.name + " Tag: " + hitObject.tag);
    //        hitObject.TakeDamage(Mathf.Abs(modifierValue), out HealthChangeInfo output);
    //    }

    //    PlayImpactEffects();
    //    Repool();
    //}
    public void PrepareForLaunch(ProjectileAbility projectileAbility)
    {
        ownerAbilityComp = projectileAbility;
        ownerTag = projectileAbility.OwnerTag;
        fireOriginPoint = projectileAbility.FirePoint.position;
        transform.position = projectileAbility.FirePoint.position;
        transform.rotation = projectileAbility.FirePoint.rotation;
    }
    public void Launch()
    {
        rigidbodyComp.velocity = transform.forward * maxVelocity;
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
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * 1.0f);
    }
}
