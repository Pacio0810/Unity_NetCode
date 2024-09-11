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
    [SerializeField] private Button HostButton;
    [SerializeField] private Button ClientButton;
    [SerializeField] private Button QuitButton;

    public AssetReferenceGameObject PlayerAssets;
    private GameObject PlayerRef;

    private void Awake()
    {
        // utilizzo le landa per creare una funzione veloce
        HostButton.onClick.AddListener(() =>
        {
            bool isHosting = NetworkManager.Singleton.StartHost();
            if (isHosting)
            {
                LoadPlayer();
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

    private void LoadPlayer()
    {
        if (NetworkManager.Singleton.IsServer)
        {

            OnLoadPlayer();
        }
        else
        {
            OnLoadPlayerServerRpc();
        }
    }

    [ServerRpc]
    private void OnLoadPlayerServerRpc(ServerRpcParams rpcParams = default)
    {
        PlayerRef.GetComponent<NetworkObject>().Spawn(true);
    }

    private void OnLoadPlayer()
    {
        if (NetworkManager.Singleton != null && PlayerAssets != null)
        {
            PlayerAssets.InstantiateAsync().Completed += OnAddressablePlayerInstantiated;
        }
    }

    private void OnAddressablePlayerInstantiated(AsyncOperationHandle<GameObject> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            // il caricamento e' avvenuto con successo quindi mi salvo il player nella sua reference e lo spawno
            PlayerRef = handle.Result;
            NetworkManager.Singleton.NetworkConfig.PlayerPrefab = PlayerRef;
            PlayerRef.GetComponent<NetworkObject>().Spawn(true);
        }
        else
        {
            Debug.Log("Loading playerPrefab Failed");
        }
    }

    private void DisableUI()
    {
        HostButton.gameObject.SetActive(false);
        ClientButton.gameObject.SetActive(false);
    }
}