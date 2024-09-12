using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;

public class NetworkManagerUI : MonoBehaviour
{
    [Header("User Interface Button Variable")]
    [SerializeField] private Button HostButton;
    [SerializeField] private Button ClientButton;
    [SerializeField] private Button QuitButton;

    [SerializeField] private SpawnManager SpawnManager;

    private void Awake()
    {
        // utilizzo le landa per creare una funzione veloce
        HostButton.onClick.AddListener(() =>
        {
            bool isHosting = NetworkManager.Singleton.StartHost();
            if (isHosting)
            {
                SpawnManager.LoadPlayer();
                DisableUI();
            }

        });

        ClientButton.onClick.AddListener(() =>
        {
            while (!NetworkManager.Singleton.StartClient())
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
        QuitButton.gameObject.SetActive(false);
    }
}