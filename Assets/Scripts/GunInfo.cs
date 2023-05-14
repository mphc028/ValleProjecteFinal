using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FPS/New gun")]
public class GunInfo : ItemInfo
{
    public float damage = 25f;
    public float damageOverDistance = 1f;
    public float frequency = 1f;
    public float dispersion = 1f;
    public float recoil = 1f;
    public float weight = 1f;
    public float reloadTime = 2.5f;
    public int maxBullets = 30;
    public int savedBullets = 90;

    public AudioClip[] useSounds;
    public AudioClip[] reloadSounds;
    public AudioClip[] drawSounds;
}
