using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PlayerControllerNetwork : NetworkBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float MovementSpeed = 3.0f;
    [SerializeField] private GameObject IsGroundedPosition;
    [SerializeField] private LayerMask GroundMask;

    [Header("Addressable Object To Spawn")]
    [SerializeField] private AssetReferenceGameObject PrefabObj;
    public float SpawnDistance = 2.0f;
    private GameObject instancePrefabRef;

    private CharacterController characterController;

    [Header("Camera Variable")]
    [SerializeField] private GameObject Camera;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        if (IsServer)
        {
            DisableLocalPlayerCamera();
        }
        CheckInput();
        
    }

    void DisableLocalPlayerCamera()
    {
        // prendo tutti i player
        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in allPlayers)
        {
            //tutti i player che sono client
            if (player.GetComponent<NetworkBehaviour>().OwnerClientId != 0)
            {
                // Disattiva la camera
                player.GetComponentInChildren<Camera>().enabled = false;
            }
        }
    }

    private void CheckInput()
    { 
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 Move = transform.right * x + transform.forward * z;
        characterController.Move(Move * MovementSpeed * Time.deltaTime);

        if (Input.GetKeyDown(KeyCode.F))
        {
            SpawnObject();
        }
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private void SpawnObject()
    {
        if (IsServer)
        {
            // sono il server/host, quindi ho l'autorita' per spawnare direttamente i GameObject
            OnSpawn();
        }
        else
        {
            // non sono il server/host, quindi per spawnare un GameObject devo fare una richiesta al server
            OnSpawnObjectServerRpc();
        }
    }

    // ServerRpc che viene chiamata dal client
    [ServerRpc]
    private void OnSpawnObjectServerRpc(ServerRpcParams rpcParams = default)
    {
        // chiamo la funzione di spawn
        OnSpawn();
    }

    private void OnSpawn()
    {
        // Istanzio il GameObject tramite gli addressable
        PrefabObj.InstantiateAsync().Completed += OnAddressableInstantiated;
    }

    private void OnAddressableInstantiated(AsyncOperationHandle<GameObject> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            // l'operazione ha successo quindi lo spawno
            instancePrefabRef = handle.Result;

            instancePrefabRef.transform.position = transform.position + transform.forward * SpawnDistance + Vector3.up;
            instancePrefabRef.GetComponent<NetworkObject>().Spawn(true);
        }
        else
        {
            Debug.Log("loading Asset Failed");
        }
    }
}
