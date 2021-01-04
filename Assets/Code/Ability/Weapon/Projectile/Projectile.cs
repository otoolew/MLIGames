using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour, IPoolable
{
    public GameObject GameObject => gameObject;

    [SerializeField] private ProjectileAbility abilityOriginComp;
    public ProjectileAbility AbilityOriginComp { get => abilityOriginComp; set => abilityOriginComp = value; }

    [SerializeField] private float range;
    public float Range { get => range; set => range = value; }

    [SerializeField] private float moveSpeed;
    public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }

    [SerializeField] private Vector3 fireOriginPoint;
    public Vector3 FireOriginPoint { get => fireOriginPoint; set => fireOriginPoint = value; }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
        if (Vector3.Distance(fireOriginPoint, transform.position)>=Range)
        {
            Repool();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        HitCollider hitCollider = other.GetComponent<HitCollider>();
        if (hitCollider)
        {
            //Debug.Log(string.Format("{0} Hit {1}", this.name, hitCollider.transform.root.name));
            hitCollider.HealthComp.ApplyHealthChange(-10);
        }
        Repool();
    }

    public void Repool()
    {
        GameAssetManager.Instance.ProjectileResourcePool.ReturnToPool(this);
    }
}
