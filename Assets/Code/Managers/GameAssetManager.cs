using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssetManager : Singleton<GameAssetManager>
{
    [SerializeField] private PoolComponent<PopUpText> popUpPool;
    public PoolComponent<PopUpText> PopUpPool { get => popUpPool; set => popUpPool = value; }

    [SerializeField] private ProjectileResourcePool projectileResourcePool;
    public ProjectileResourcePool ProjectileResourcePool { get => projectileResourcePool; set => projectileResourcePool = value; }

    [Header("Factories")]
    [SerializeField] private WeaponFactory weaponFactory;
    public WeaponFactory WeaponFactory { get => weaponFactory; set => weaponFactory = value; }


    public void Start()
    {
        
    }

}
