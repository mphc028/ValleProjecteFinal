using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviourPunCallbacks
{
    public ItemInfo itemInfo;
    public GameObject itemGameObject;
    public Animator animator;
    protected AudioSource audioSrc;

    protected PlayerMovement player;

    [SerializeField] public SkinnedMeshRenderer[] meshes;

    protected bool canUse = true;

    public abstract void Use();
    public abstract void Reload();
    public abstract void Inspect();

    private void Start()
    {
        player = transform.parent.parent.parent.GetComponent<PlayerMovement>();
    }

    public Sprite GetSprite()
    {
        return itemInfo.image;
    }

    [PunRPC]
    public void PlaySound(AudioClip sound)
    {
        audioSrc.clip = sound;
        audioSrc.Play();
    }

    public void PlayAnimation(string name)
    {
        if (animator.gameObject.activeSelf) animator.Play(name);
    }

    public void TransferMovement(float speed)
    {
        if (animator.gameObject.activeSelf) animator.SetFloat("speed", speed);
    }

    public void Hide()
    {
        canUse = false;
        StopAllCoroutines();
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }

    public void Show()
    {
        PlaySound(((GunInfo)itemInfo).drawSounds[Random.Range(0, ((GunInfo)itemInfo).drawSounds.Length - 1)]);
        canUse = true;
        if (!gameObject.transform.GetChild(0).gameObject.activeSelf) StartCoroutine(WaitAndActivate());
        else gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }

    protected IEnumerator WaitAndDeactivate()
    {
        canUse = false;
        PlayAnimation("Hide");
        Debug.Log("Hide");
        yield return new WaitForSeconds(0);
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }
    protected IEnumerator WaitAndActivate()
    {
        canUse = false;
        yield return new WaitForSeconds(.2f);
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        PlayAnimation("Show");
        Debug.Log("Show");
        canUse = true;

    }

    protected abstract IEnumerator ReloadTimer();

}
