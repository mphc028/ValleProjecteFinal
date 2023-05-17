using System.Collections;
using UnityEngine;

public class ProceduralRecoil : MonoBehaviour
{
    Vector3 currentRotation, targetRotation, targetPosition, currentPosition, initialGunPosition;
    public Transform cam;

    [SerializeField] float recoilX;
    [SerializeField] float recoilY;
    [SerializeField] float recoilZ;

    [SerializeField] float kickBackZ;
    SingleShotGun gun;

    public float snappiness, returnAmount;

    private void Start()
    {
        gun = GetComponent<SingleShotGun>();
        initialGunPosition = transform.localPosition;
    }

    private void LateUpdate()
    {
        if (!gun.animator.gameObject.activeSelf) return;
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, Time.deltaTime * returnAmount);
        currentPosition = Vector3.Slerp(currentRotation, targetRotation, Time.fixedDeltaTime*snappiness);
        transform.localRotation = Quaternion.Euler(currentRotation);
        if (cam == null) return;
        cam.localRotation = Quaternion.Euler(-targetRotation);
        Back();
    }

    public void Recoil()
    {
        if (!gun.animator.gameObject.activeSelf) return;
        targetPosition -= new Vector3(0, 0, kickBackZ);
        targetRotation += new Vector3(recoilY, Random.Range(-recoilX, recoilX), Random.Range(-recoilZ, recoilZ));
        Debug.Log("REC");
    }

    private void Back()
    {
        targetPosition = Vector3.Lerp(targetPosition, initialGunPosition, Time.deltaTime * returnAmount);
        currentPosition = Vector3.Lerp(currentPosition, targetPosition, Time.fixedDeltaTime * snappiness);  
        transform.localPosition = currentPosition;
    }
}
