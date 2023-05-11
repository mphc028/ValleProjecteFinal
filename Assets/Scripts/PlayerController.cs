using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Realtime;
using UnityEngine.UI;

public class PlayerController : MonoBehaviourPunCallbacks, IDamageable
{
    [SerializeField] Image healthbarImage;
    [SerializeField] GameObject ui;

    [SerializeField] GameObject cameraHolder;
    [SerializeField] float mouseSensitivity, smoothTime;

    [SerializeField] Item[] items;

    int itemIndex;
    int previousItemIndex = -1;

    float verticalLookRotation;
    [SerializeField] bool grounded;
    bool crouched;
    bool jumping;
    Vector3 smoothMoveVelocity;
    Vector3 moveAmount;
    Rigidbody rb;

    PhotonView PV;

    const float maxHealth = 100f;
    float currentHealth = maxHealth;

    PlayerManager playerManager;

    Vector3 originalCamPos;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();

        playerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
    }

    private void Start()
    {
        if (PV.IsMine)
        {
            originalCamPos = transform.GetChild(0).localPosition;
            EquipItem(0);
        }
        else
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(rb);
            Destroy(ui);
        }

    }

    private void Update()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (!PV.IsMine) return;
        Look();

        for (int i = 0; i < items.Length; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                EquipItem(i);
                break;
            }
        }

        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
        {
            if(itemIndex >= items.Length -1)
            {
                EquipItem(0);
            }
            else
            {
                EquipItem(itemIndex + 1);
            }
        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
        {
            if (itemIndex <= 0)
            {
                EquipItem(items.Length-1);
            }
            else
            {
                EquipItem(itemIndex - 1);
            }
        }

        if (Input.GetMouseButtonDown(0)) items[itemIndex].Use();

        if (transform.position.y < -10) Die();

        items[itemIndex].TransferMovement(rb.velocity.magnitude/10);
    }

    void Look()
    {
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);

        verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

        cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
    }

    void EquipItem(int _index)
    {
        if (_index == previousItemIndex) return;

        itemIndex = _index;
        items[itemIndex].itemGameObject.SetActive(true);


        if (previousItemIndex != -1)
        {
            items[previousItemIndex].GetComponent<SingleShotGun>().Hide();
        }

        previousItemIndex = itemIndex;
        
        if (PV.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("itemIndex", itemIndex);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("itemIndex") && !PV.IsMine && targetPlayer == PV.Owner)
        {
            EquipItem((int)changedProps["itemIndex"]);
        }
    }

    public void SetGroundedState(bool _grounded)
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, 2 * 0.5f + 0.2f, LayerMask.NameToLayer("Ground"));
    }

    private void FixedUpdate()
    {
        if (!PV.IsMine) return;
        rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }

    public void TakeDamage(float damage)
    {
        PV.RPC(nameof(RPC_TakeDamage), PV.Owner, damage);
    }

    [PunRPC]
    void RPC_TakeDamage(float damage, PhotonMessageInfo info)
    {
        currentHealth -= damage;

        healthbarImage.fillAmount = currentHealth / maxHealth;

        if (currentHealth <= 0)
        {
            Die();
            PlayerManager.Find(info.Sender).GetKill();
        }
    }

    void Die()
    {
        playerManager.Die();
    }
}
