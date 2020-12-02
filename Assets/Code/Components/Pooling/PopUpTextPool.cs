using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpTextPool : PoolComponent<PopUpText>
{
    [SerializeField] PopUpText popUpTextPrefab;
    public override PopUpText PoolablePrefab { get => popUpTextPrefab; set => popUpTextPrefab = value; }

    public override PopUpText CreatePooledObject()
    {
        PopUpText popUp = Instantiate(popUpTextPrefab);
        return popUp;
    }
}
