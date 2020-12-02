using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssetManager : Singleton<GameAssetManager>
{
    [SerializeField] private PoolComponent<PopUpText> popUpPool;
    public PoolComponent<PopUpText> PopUpPool { get => popUpPool; set => popUpPool = value; }

    [SerializeField] private PoolComponent<Projectile> projectilePool;
    public PoolComponent<Projectile> ProjectilePool { get => projectilePool; set => projectilePool = value; }

}
