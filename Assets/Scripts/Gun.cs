using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gun : Item
{

    int ammo = 0;
    int savedAmmo = 0;
    int roundAmmo = 0;

    public abstract override void Use();
    public abstract override void Reload();

    public GameObject bulletImpactPrefab;

    public void SetStartAmmo()
    {
        ammo = ((GunInfo)itemInfo).maxBullets;
        savedAmmo = ((GunInfo)itemInfo).savedBullets;
        roundAmmo = ((GunInfo)itemInfo).maxBullets;
    }

    public bool HasAmmo()
    {
        return ammo > 0;
    }

    public bool HasAmmoSaved()
    {
        return savedAmmo > 0;
    }

    public void ShootAmmo()
    {
        ammo--;
    }

    protected bool IsFullAmmo()
    {
        return ammo >= roundAmmo;
    }



    public void FillBullets()
    {
        int bulletsToReload = roundAmmo - ammo;
        if (savedAmmo < bulletsToReload) bulletsToReload = savedAmmo;
        if (bulletsToReload > 0)
        {
            ammo += bulletsToReload;
            savedAmmo -= bulletsToReload;
        }
    }

    protected override IEnumerator ReloadTimer()
    {
        Debug.Log("Before" + ammo.ToString() + "/" + savedAmmo.ToString());
        Debug.Log("Reload");
        PlayAnimation("Reload");
        canUse = false;
        yield return new WaitForSeconds(((GunInfo)itemInfo).reloadTime);
        FillBullets();
        Debug.Log("After" + ammo.ToString() + "/" + savedAmmo.ToString());
        canUse = true;
    }
}
