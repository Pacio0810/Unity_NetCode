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
        if(!IsOwner)
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

        return MoveDirection;
    }
}
