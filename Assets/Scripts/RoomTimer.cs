using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hastable = ExitGames.Client.Photon.Hashtable;
using TMPro;

public class RoomTimer : MonoBehaviour
{
    [SerializeField] private TMP_Text time;
    [SerializeField] private bool count;
    [SerializeField] private int Time;
    Hastable setTime = new Hastable();

    void Start()
    {
        count = true;
    }

    void Update()
    {
        Time = (int)PhotonNetwork.CurrentRoom.CustomProperties["Time"];
        float minutes = Mathf.FloorToInt(Time / 60);
        float seconds = Mathf.FloorToInt(Time % 60);

        time.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        if (count)
        {
            count = false;
            if (PhotonNetwork.IsMasterClient) StartCoroutine(Timer());
        }
    }

    IEnumerator Timer()
    {
        yield return new WaitForSeconds(1);
        int nextTime = --Time;
        setTime["Time"] = nextTime; 
        PhotonNetwork.CurrentRoom.SetCustomProperties(setTime);
        count = true;
    }
}
