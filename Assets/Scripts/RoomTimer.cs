using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using Hastable = ExitGames.Client.Photon.Hashtable;
using TMPro;
using UnityEngine.Networking;

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
                    if (player.CustomProperties.TryGetValue("id", out object userId)) StartCoroutine(AddXP((int)kills, (int)userId));
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

    IEnumerator AddXP(int kills, int userId)
    {
        // Obtener el ID del jugador y la experiencia del jugador (puedes reemplazar estos valores con los adecuados)
        int userExperience = 20;

        // Crear los datos que se enviarán en la petición POST
        WWWForm form = new WWWForm();
        form.AddField("userId", userId);
        form.AddField("userExperience", userExperience * kills);

        using (UnityWebRequest webRequest = UnityWebRequest.Post("https://imanol.xyz/api/players/index.php", form))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Error en la solicitud: " + webRequest.error);
            }
            else
            {
                Debug.Log("Datos del jugador enviados correctamente.");
            }
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
        PhotonNetwork.LoadLevel(1);

    }
}
