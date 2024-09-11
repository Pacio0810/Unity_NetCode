using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button HostButton;
    [SerializeField] private Button ClientButton;
    [SerializeField] private Button QuitButton;

    private void Awake()
    {
        // utilizzo le landa per creare una funzione veloce
        HostButton.onClick.AddListener(() =>
        {
            bool isHosting = NetworkManager.Singleton.StartHost();
            if (isHosting)
            {
                DisableUI();
            }

        });

        ClientButton.onClick.AddListener(() =>
        {
            while(!NetworkManager.Singleton.StartClient())
            {
                continue;
            }
            DisableUI();
        });

        QuitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }

    private void DisableUI()
    {
        HostButton.gameObject.SetActive(false);
        ClientButton.gameObject.SetActive(false);
    }
}
