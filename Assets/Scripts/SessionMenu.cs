using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;


public class SessionMenu : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_InputField username;
    [SerializeField] TMP_InputField email;
    [SerializeField] TMP_InputField password;

    public void LoginUser()
    {
        StartCoroutine("Login");
    }

    public void RegisterUser()
    {
        StartCoroutine("Register");
    }


    void Start()
    {
        if (PlayerPrefs.HasKey("email") && PlayerPrefs.HasKey("password")) LoginUser();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F9)) PlayerPrefs.DeleteAll();
    }
    IEnumerator Register()
    {
        string url = "https://imanol.xyz/register.php";

        Dictionary<string, string> postData = new Dictionary<string, string>();
        postData.Add("userName", username.text);
        postData.Add("userMail", email.text);
        postData.Add("userPwd", password.text);

        string data = "";
        foreach (var pair in postData)
        {
            data += $"{pair.Key}={pair.Value}&";
        }
        byte[] byteData = Encoding.UTF8.GetBytes(data);

        var request = new UnityEngine.Networking.UnityWebRequest(url, "POST");
        request.uploadHandler = new UnityEngine.Networking.UploadHandlerRaw(byteData);
        request.downloadHandler = new UnityEngine.Networking.DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error en la petición: " + request.error);
        }
        else
        {
            string response = request.downloadHandler.text;
            Debug.Log("Respuesta: " + response);
            PlayerPrefs.SetString("username", username.text);
            PlayerPrefs.SetString("email", email.text);
            PlayerPrefs.SetString("password", password.text);
            LoginUser();
        }
    }

    IEnumerator Login()
    {
        string url = "https://imanol.xyz/login.php";

        Dictionary<string, string> postData = new Dictionary<string, string>();
        postData.Add("userMail", PlayerPrefs.GetString("email"));
        postData.Add("userPwd", PlayerPrefs.GetString("password"));

        string data = "";
        foreach (var pair in postData)
        {
            data += $"{pair.Key}={pair.Value}&";
        }
        byte[] byteData = Encoding.UTF8.GetBytes(data);

        var request = new UnityEngine.Networking.UnityWebRequest(url, "POST");
        request.uploadHandler = new UnityEngine.Networking.UploadHandlerRaw(byteData);
        request.downloadHandler = new UnityEngine.Networking.DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error en la petición: " + request.error);
        }
        else
        {
            string response = request.downloadHandler.text;
            EnterGame(response=="true");
        }
    }

    void EnterGame(bool enter)
    {
        PhotonNetwork.LoadLevel(1);
    }
}
