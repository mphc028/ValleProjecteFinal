using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hastable = ExitGames.Client.Photon.Hashtable;
using TMPro;

public class RoomTimer : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_Text time;
    

    [SerializeField] private bool count;
    [SerializeField] private int counter;
    Hastable setTime = new Hastable();

    [SerializeField] private TMP_Text winnerText;
    bool gameEnded = false;

    void Start()
    {
        count = true;
    }

    



    void Update()
    {
        counter = (int)PhotonNetwork.CurrentRoom.CustomProperties["Time"];
        float minutes = Mathf.FloorToInt(counter / 60);
        float seconds = Mathf.FloorToInt(counter % 60);

        time.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        if (counter <= 0 && !gameEnded)
        {
            gameEnded = true;
            Player winner = null;
            int maxKills = 0;

            foreach (Player player in PhotonNetwork.PlayerList)
            {
                player.CustomProperties.TryGetValue("kills", out object kills);
                if (kills != null)
                    if ((int)kills > maxKills)
                    {
                        maxKills = (int)kills;
                        winner = player;
                    }
            }

            // Display the winner's name
            if (winner != null)
            {
                winnerText.text = "THE WINNER IS "+winner.NickName;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            StartCoroutine(ExitRoom());
        }
        else if (!gameEnded)
        {
            if (count)
            {
                count = false;
                if (PhotonNetwork.IsMasterClient) StartCoroutine(Timer());
            }
        }
        else
        {
            Time.timeScale = Mathf.Lerp(Time.timeScale, 0, Time.deltaTime*10);
        }
    }

    IEnumerator Timer()
    {
        yield return new WaitForSeconds(1F);
        int nextTime = --counter;
        setTime["Time"] = nextTime; 
        PhotonNetwork.CurrentRoom.SetCustomProperties(setTime);
        count = true;
    }

    IEnumerator ExitRoom()
    {
        yield return new WaitForSecondsRealtime(5F);
        Time.timeScale = 1;

        Destroy(RoomManager.Instance.gameObject);

        PhotonNetwork.LeaveRoom();
        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.LoadLevel(0);

    }
}
