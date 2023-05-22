using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] Animator fullBody;
    private CharacterController cc;
    private PhotonView PV;

    float xSpeed = 0;
    float ySpeed = 0;
    float vSpeed = 0;

    public void SetBodyState(int state)
    {
        fullBody.SetInteger("state", state);
    }

    public void SetBodySpeed(Vector3 speed)
    {
        fullBody.SetBool("isGrounded", cc.isGrounded);
   
            fullBody.SetFloat("sSpeed", speed.x);
            fullBody.SetFloat("fSpeed", -speed.z);
    }

    private void Start()
    {
        PV = GetComponent<PhotonView>();
        cc = GetComponent<CharacterController>();

    }

    private void Update()
    {
        if (!PV.IsMine) return;
        float speed = cc.velocity.magnitude;
        xSpeed = Mathf.Lerp(xSpeed, Input.GetAxisRaw("Horizontal") * speed, Time.deltaTime * 10);
        ySpeed = Mathf.Lerp(ySpeed, Input.GetAxisRaw("Vertical") * speed, Time.deltaTime * 10);
        vSpeed = cc.velocity.y;

        Vector3 fullSpeed = new Vector3(xSpeed, vSpeed, ySpeed);

        SetBodySpeed(fullSpeed);
    }







}