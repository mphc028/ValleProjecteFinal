using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Photon.Pun;


public class ClientDebug : MonoBehaviour
{
    private const int poolNumber = 10;
    private string[] debuggers;

    private void Start()
    {
        debuggers = new string[poolNumber];
    }



    public void SetDebugger(string content, int index)
    {
        debuggers[index] = content;
    }


    private void OnGUI()
    {
        for (int i = 0; i < debuggers.Length; i++)
        {
            GUI.Label(new Rect(10, 200 + i * 20, 100, 20), debuggers[i]);
        }
    }
}
