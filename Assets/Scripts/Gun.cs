using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Gun : Item
{
    public abstract override void Use();
    public abstract override void PlayAnimation(string name);

    public abstract override void TransferMovement(float speed);

    public GameObject bulletImpactPrefab;
}
