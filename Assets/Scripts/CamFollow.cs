using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CamFollow : MonoBehaviour
{
    [SerializeField] GameObject holder;
 
    void LateUpdate()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, holder.transform.rotation, 10f * Time.deltaTime);
        transform.position = holder.transform.position;
    }
}
