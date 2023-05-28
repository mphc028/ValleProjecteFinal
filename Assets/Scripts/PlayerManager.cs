using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;
using System.IO;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;

    GameObject controller;
    [SerializeField] AudioClip killSound;
    [SerializeField] GameObject ragdoll;

    int kills;
    int deaths;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (PV.IsMine)
        {
            CreateController();
        }
    }


    void CreateController()
    {
        Transform spawnpoint = SpawnManager.Instance.GetSpawnPoint();
        controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerEntity"), spawnpoint.position, spawnpoint.rotation, 0, new object[] { PV.ViewID });

        Debug.Log("Instantied Player");
    }

    public void Die()
    {
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Ragdoll"), controller.transform.position, controller.transform.rotation, 0);
        PhotonNetwork.Destroy(controller);
        CreateController();

        deaths++;

        Hashtable hash = new Hashtable();
        hash.Add("deaths", deaths);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    public void GetKill()
    {


        PV.RPC(nameof(RPC_GetKill), PV.Owner);
    }

    [PunRPC]
    void RPC_GetKill()
    {
        kills++;
        controller.GetComponent<AudioSource>().clip = killSound;
        controller.GetComponent<AudioSource>().Play();
        if (controller.GetComponent<AudioSource>().pitch < 2)
        {
            controller.GetComponent<AudioSource>().pitch *= 1.0594f;
        }
        else
        {
            controller.GetComponent<AudioSource>().pitch = 1;
        }
        Hashtable hash = new Hashtable();
        hash.Add("kills", kills);
        hash.Add("id", PlayerPrefs.GetInt("id"));
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    public static PlayerManager Find(Player player)
    {
        return FindObjectsOfType<PlayerManager>().SingleOrDefault(x => x.PV.Owner == player);
    }
}
