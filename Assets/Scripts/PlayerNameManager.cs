using Photon.Pun;
using TMPro;
using UnityEngine;

public class PlayerNameManager : MonoBehaviour
{

    private void Start()
    {
        PhotonNetwork.NickName = PlayerPrefs.GetString("username");
    }
}
