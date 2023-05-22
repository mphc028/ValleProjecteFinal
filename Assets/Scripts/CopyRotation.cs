using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyRotation : MonoBehaviour
{
    [SerializeField] GameObject cam;
    private void Update()
    {
        transform.rotation = cam.transform.rotation;
    }
}
