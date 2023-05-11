using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public ItemInfo itemInfo;
    public GameObject itemGameObject;
    public Animator animator;

    public abstract void Use();

    public abstract void PlayAnimation(string name);

    public abstract void TransferMovement(float speed);
}
