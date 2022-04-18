using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;
using Cinemachine;

public class PlayerController : NetworkBehaviour
{
    public float MovementSpeed = 12f;

    public float JumpForce = 5f;

    public float DistanceToGround = 0.5f;

    public float RotationSmoothing = 20f;

    public GameObject CameraAnchor;

    public CinemachineVirtualCamera PlayerCamera;

    private float yaw;
    private float pitch;

    private Rigidbody _Rigidbody;

    private PlayerInputAction _PlayerInputActions;
    private PlayerAnimationController _PlayerAnimationController;

    private void Start()
    {
        pitch = 0;
        yaw = 0;
       
        if (isClient && isLocalPlayer) PlayerCamera.Priority = 100;
    }

    private void Awake()
    {
        _Rigidbody = GetComponent<Rigidbody>();
        _PlayerAnimationController = GetComponent<PlayerAnimationController>();

        _PlayerInputActions = new PlayerInputAction();
        _PlayerInputActions.Enable();
        _PlayerInputActions.Player.Jump.performed += Jump;
    }

    private void FixedUpdate()
    {
        Vector2 InputVector = _PlayerInputActions.Player.Aim.ReadValue<Vector2>();

        yaw += InputVector.x;
        pitch -= InputVector.y;

        float UpDown = _PlayerInputActions.Player.UpDown.ReadValue<float>();
        float RightLeft = _PlayerInputActions.Player.RightLeft.ReadValue<float>();

        SetRotationCamera(pitch, yaw);
       
        if (isClient && isLocalPlayer)
        {

            Move(RightLeft, UpDown);

            SetRotation(yaw);
        }
    }

    public void Jump(InputAction.CallbackContext Context)
    {
        if (IsGround()) _Rigidbody.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
    }

    private bool IsGround()
    {
        return Physics.Raycast(transform.position, Vector3.down, DistanceToGround);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position +
            (Vector3.down * DistanceToGround));
    }

    [Command]
    public void Move(float RightLeft, float UpDown)
    {
        _Rigidbody.AddForce(new Vector3(RightLeft, 0, UpDown)
            * MovementSpeed, ForceMode.Force);

        _PlayerAnimationController.SetAnimationMove(new Vector2(RightLeft, UpDown));
    }

    [Command]
    public void SetRotation(float yaw)
    {
     
        Quaternion SmoothRotation = Quaternion.Euler(0, yaw, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, SmoothRotation, RotationSmoothing * Time.fixedDeltaTime);
    }

    public void SetRotationCamera(float pitch, float yaw)
    {
        pitch = Mathf.Clamp(pitch, -60, 90);

        Quaternion SmoothRotation = Quaternion.Euler(pitch, yaw, 0);

        CameraAnchor.transform.rotation = Quaternion.Slerp(CameraAnchor.transform.rotation, SmoothRotation,
            RotationSmoothing * Time.fixedDeltaTime);
    }
}