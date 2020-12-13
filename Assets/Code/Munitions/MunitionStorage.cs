using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MunitionStorage : MonoBehaviour
{
    [SerializeField] private int ammoCapacity;
    public int AmmoCapacity { get => ammoCapacity; set => ammoCapacity = value; }

    [SerializeField] private int ammoCount;
    public int AmmoCount { get => ammoCount; set => ammoCount = value; }

    [SerializeField] private int magazineCapacity;
    public int MagazineCapacity { get => magazineCapacity; set => magazineCapacity = value; }

    [SerializeField] private int magazineCount;
    public int MagazineCount { get => magazineCount; set => magazineCount = value; }

    public bool IsMagazineEmpty { get { if (magazineCount <= 0) return true; else return false; } }

    //public UnityEvent<int> onConsumeAmmo;
    public UnityEvent<string> onAmmoCountChange;

    private void Awake()
    {

    }

    private void OnEnable()
    {
        onAmmoCountChange = new UnityEvent<string>();
    }

    //public void Init(MunitionStorage munitionStorage)
    //{
    //    ammoCapacity = munitionStorage.ammoCapacity;
    //    ammoCount = munitionStorage.ammoCount;
    //    magazineCapacity = munitionStorage.magazineCapacity;
    //    magazineCount = munitionStorage.magazineCount;
    //}

    public bool ComsumeAmmo()
    {
        if (magazineCount > 0)
        {
            magazineCount -= 1;
            onAmmoCountChange.Invoke(magazineCount + " / " + ammoCount);
            return true;
        }
        else return false;
    }

    public void FillMagazine()
    {
        if (ammoCount <= 0)
            return;
        int requestAmount = magazineCapacity - magazineCount;

        if (ammoCount >= requestAmount)
        {
            ammoCount -= requestAmount;
            magazineCount += requestAmount;
        }
        else
        {
            requestAmount -= ammoCount;

            ammoCount -= requestAmount;
            magazineCount += requestAmount;
        }
        onAmmoCountChange.Invoke(magazineCount + " / " + ammoCount);
    }

    public string GetMunitionsDisplay()
    {
        return magazineCount + " / " + ammoCount;
    }
}
