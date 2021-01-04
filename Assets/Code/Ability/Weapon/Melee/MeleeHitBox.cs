using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeHitBox : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name + " was Melee Hit");
        HitCollider hitCollider = other.GetComponent<HitCollider>();
        if (hitCollider)
        {
            //Debug.Log(string.Format("{0} Hit {1}", this.name, hitCollider.transform.root.name));
            hitCollider.HealthComp.ApplyHealthChange(-10);
        }
    }
}
