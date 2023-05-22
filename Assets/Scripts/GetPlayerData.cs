using System.Collections;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GetPlayerData : MonoBehaviour
{
    [SerializeField] TMP_Text username;
    [SerializeField] TMP_Text xp;
    [SerializeField] TMP_Text level;
    [SerializeField] Image xpBar;

    PlayerData playerData;

    void Start()
    {
        string email = PlayerPrefs.GetString("email");
        string url = "https://imanol.xyz/api/players/?email=" + UnityWebRequest.EscapeURL(email);

        username.text = "";
        xp.text = "";
        level.text = "";

        StartCoroutine(GetPlayerDataFromAPI(url));
    }

    private void SetPlayerDataOnUI()
    {
        username.text = playerData.userName;
        xp.text = playerData.userExperience+"/"+playerData.userNeededExperience;
        level.text = "LEVEL "+playerData.userLevel;
        xpBar.fillAmount = float.Parse(playerData.userExperience) / float.Parse(playerData.userNeededExperience);
    }

    IEnumerator GetPlayerDataFromAPI(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log("Error en la solicitud: " + webRequest.error);
            }
            else
            {
                string jsonResponse = webRequest.downloadHandler.text;
                Debug.Log("Respuesta: " + jsonResponse);

                // Parsear la respuesta JSON a un arreglo
                playerData = JsonUtility.FromJson<PlayerData>(jsonResponse);

                // Acceder a los datos individualmente
                Debug.Log("userId: " + playerData.userId);
                Debug.Log("userName: " + playerData.userName);
                Debug.Log("userMail: " + playerData.userMail);
                Debug.Log("userExperience: " + playerData.userExperience);
                Debug.Log("userNeededExperience: " + playerData.userNeededExperience);
                Debug.Log("userLevel: " + playerData.userLevel);
                Debug.Log("userRange: " + playerData.userRange);
                Debug.Log("userElo: " + playerData.userElo);
                PlayerPrefs.SetInt("id", int.Parse(playerData.userId));

                SetPlayerDataOnUI();
            }
        }


    }


    // Clase para almacenar los datos del jugador
    [System.Serializable]
    public class PlayerData
    {
        public string userId;
        public string userName;
        public string userMail;
        public string userExperience;
        public string userNeededExperience;
        public string userLevel;
        public string userRange;
        public string userElo;
    }
}