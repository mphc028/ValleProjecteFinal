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
    }

    public override void Use()
    {
        Shoot();
    }

    public override void PlayAnimation(string name)
    {
        animator.Play(name);
    }
    public override void TransferMovement(float speed)
    {
        animator.SetFloat("speed",speed);
    }

    void Shoot()
    {
        PlayAnimation("Shoot");
        Camera cam = transform.parent.parent.GetComponentInChildren<Camera>();    
        Ray ray = cam.ViewportPointToRay(new Vector3(.5f, .5f));
        ray.origin = cam.transform.position;
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(((GunInfo)itemInfo).damage);
            PV.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
        }
    }

    public void Hide()
    {
        PlayAnimation("Hide");
        StartCoroutine(WaitAndDeactivate());
    }

    public void Show()
    {
        StartCoroutine(WaitAndActivate());
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

    IEnumerator WaitAndDeactivate()
    {
        yield return new WaitForSeconds(.3f);
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }
    IEnumerator WaitAndActivate()
    {
        yield return new WaitForSeconds(.6f);
        
        PlayAnimation("Show");
    }
}
