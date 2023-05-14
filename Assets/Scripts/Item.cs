using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public ItemInfo itemInfo;
    public GameObject itemGameObject;
    public Animator animator;
    protected AudioSource audioSrc;

    protected bool canUse = true;

    public abstract void Use();
    public abstract void Reload();
    public abstract void Inspect();



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
        AudioClip sound = ((GunInfo)itemInfo).drawSounds[Random.Range(0, ((GunInfo)itemInfo).drawSounds.Length - 1)];
        audioSrc.clip = sound;
        audioSrc.Play();
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
