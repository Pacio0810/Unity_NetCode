using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerControllerNetwork : NetworkBehaviour
{
    [SerializeField] private float MovementSpeed = 3.0f;
    [SerializeField] private float JumpForce = 10.0f;
    [SerializeField] private LayerMask GroundMask;
    [SerializeField] private GameObject IsGroundedPosition;
    [SerializeField] private GameObject PrefabObj;

    private CharacterController characterController;
    private bool isGrounded = false;
    private Rigidbody _rb;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        _rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!IsOwner)
        {
            return;
        }
        CheckGround();
        transform.position += CheckInput() * MovementSpeed * Time.deltaTime;
    }

    void CheckGround()
    {
        isGrounded = false;
        if (Physics.CheckSphere(IsGroundedPosition.transform.position, 0.25f, GroundMask))
        {
            isGrounded = true;
        }
    }

    Vector3 CheckInput()
    {
        Vector3 MoveDirection = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.W))
        {
            MoveDirection.z = +1.0f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            MoveDirection.z = -1.0f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            MoveDirection.x = -1.0f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            MoveDirection.x = +1.0f;
        }
        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {
            Vector3 jump = Vector3.up * JumpForce;
            _rb.AddForce(jump, ForceMode.Impulse);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            SpawnObject();
        }

        return MoveDirection;
    }

    private void SpawnObject()
    {
        //Vector3 offset = new Vector3(transform.position.x, 1.0f, transform.position.z - 1.0f);
        //GameObject spawnedObj = Instantiate(SpawnedGameObject, offset, Quaternion.identity);

        
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

        //NetworkObject ObjSpawned = NetworkManager.SpawnManager.InstantiateAndSpawn(PrefabObj, forceOverride: true);
        //ObjSpawned.transform.position = offset;
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
        Vector3 offset = new Vector3(transform.position.x, 1.0f, transform.position.z - 1.0f);

        // Istanzio il GameObject e poi lo spawno
        GameObject spawnedObj = Instantiate(PrefabObj, offset, Quaternion.identity);
        spawnedObj.GetComponent<NetworkObject>().Spawn(true);
    }
}
