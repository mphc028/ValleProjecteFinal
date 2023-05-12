using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRecoil : MonoBehaviour
{

    private float verticalPhase = 0;
    private Vector3 targetRotation;
    private Vector3 currentRotation;
    private Vector3 startRotation;
    public bool isActive;

    private void Start()
    {
        startRotation = transform.localRotation.eulerAngles;
    }

    public void SetVerticalPhase(float phase)
    {
        verticalPhase = phase * 100F;
        targetRotation -= new Vector3(verticalPhase, Random.Range(-verticalPhase, verticalPhase), Random.Range(-verticalPhase, verticalPhase));
    }

    private void Update()
    {
            targetRotation = Vector3.Slerp(targetRotation, Vector3.zero, 120f * Time.deltaTime);
            currentRotation = Vector3.Slerp(currentRotation, targetRotation, 32f * Time.deltaTime);
            transform.localRotation *= Quaternion.Euler(currentRotation);
    }
}