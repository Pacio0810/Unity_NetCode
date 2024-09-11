using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button HostButton;
    [SerializeField] private Button ClientButton;
    [SerializeField] private Button QuitButton;

    public AssetReferenceGameObject PlayerAssets;
    private GameObject PlayerRef;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        // utilizzo le landa per creare una funzione veloce
        HostButton.onClick.AddListener(() =>
        {
            bool isHosting = NetworkManager.Singleton.StartHost();
            if (isHosting)
            {
                if (NetworkManager.Singleton != null && PlayerAssets != null)
                {
                    // istanzio il player dagli addressable
                    PlayerAssets.InstantiateAsync().Completed += OnAddressablePlayerInstantiated;

                    // una volta caricato lo assegno al NetworkManager
                    NetworkManager.Singleton.NetworkConfig.PlayerPrefab = PlayerRef;
                }
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

    private void OnAddressablePlayerInstantiated(AsyncOperationHandle<GameObject> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            // il caricamento e' avvenuto con successo quindi mi salvo il player nella sua reference e lo spawno
            PlayerRef = handle.Result;
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
