using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SpawnManager : MonoBehaviour
{
    [Header("Addressable Player variable")]
    [SerializeField] private AssetReferenceGameObject PlayerAssets;

    private GameObject PlayerRef;

    private void Awake()
    {
        DontDestroyOnLoad(this);

    }
    public void LoadPlayer()
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
}
