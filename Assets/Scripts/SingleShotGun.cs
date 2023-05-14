using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleShotGun : Gun
{
    Camera cam;
    PhotonView PV;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
        audioSrc = GetComponent<AudioSource>();
        SetStartAmmo();
    }

    public override void Use()
    {
        if (!HasAmmo()) Reload();
        Shoot();
    }

    private void OnEnable()
    {
   
    }


    void Shoot()
    {
        if (!canUse || !HasAmmo()) return;
        float damage = ((GunInfo)itemInfo).damage;
        float recoil = ((GunInfo)itemInfo).recoil;
        float frequency = ((GunInfo)itemInfo).frequency;
        float dispersion = ((GunInfo)itemInfo).dispersion;
        float damageOverDistance = ((GunInfo)itemInfo).damageOverDistance;
        AudioClip sound = ((GunInfo)itemInfo).useSounds[Random.Range(0,( (GunInfo)itemInfo).useSounds.Length - 1)];
        audioSrc.clip = sound;
        audioSrc.Play();

        float hDispersion = Random.Range(-1f, 1f)* dispersion;
        float vDispersion = Random.Range(-1f, 1f) * dispersion;

        ShootAmmo();
        PlayAnimation("Shoot");

        Camera cam = transform.parent.parent.GetComponentInChildren<Camera>();
        ProceduralRecoil rec = GetComponent<ProceduralRecoil>();

        rec.Recoil();

        Ray ray = cam.ViewportPointToRay(new Vector3(.5f+hDispersion, .5f+vDispersion));
        ray.origin = cam.transform.position;
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            float realDamage = damage/(hit.distance*damageOverDistance);
            hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(realDamage);
            PV.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
        }

        StartCoroutine(ShootFreq(frequency));
    }

    protected IEnumerator ShootFreq(float freq)
    {
        canUse = false;
        yield return new WaitForSeconds(1f/freq);
        canUse = true;
    }
    public override void Reload()
    {
        if (!canUse || IsFullAmmo() || !HasAmmoSaved()) return;

        AudioClip sound = ((GunInfo)itemInfo).reloadSounds[Random.Range(0, ((GunInfo)itemInfo).reloadSounds.Length - 1)];
        audioSrc.clip = sound;
        audioSrc.Play();

        StartCoroutine(ReloadTimer());
    }


    [PunRPC]
    void RPC_Shoot(Vector3 hitPosition, Vector3 hitNormal)
    {
        Collider[] colliders = Physics.OverlapSphere(hitPosition, .3f);
        if (colliders.Length != 0) 
        { 
            GameObject bulletImpactObj = Instantiate(bulletImpactPrefab, hitPosition + hitNormal *.001f, Quaternion.LookRotation(hitNormal, Vector3.up) * bulletImpactPrefab.transform.rotation);
            Destroy(bulletImpactObj, 10f);
            bulletImpactObj.transform.SetParent(colliders[0].transform);
        }
    }
}
