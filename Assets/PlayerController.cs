using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    public float MovementSpeed = 12f;

    public float JumpForce = 5f;

    public float DistanceToGround = 0.5f;

    public GameObject CameraAnchor;

    public float RotationSmoothing = 20f;

    private Rigidbody _RigidBody;

    private PlayerInputAction _PlayerInputActions;

    private float yaw;

    private float pitch;

    private PlayerAnimationController _PlayerAnimationController;
    private void Awake()
    {
        _RigidBody = GetComponent<Rigidbody>();
        _PlayerAnimationController = GetComponent<PlayerAnimationController>();

        _PlayerInputActions = new PlayerInputAction();
        _PlayerInputActions.Enable();
        _PlayerInputActions.Player.Jump.performed += Jump;
    }

    public void Jump(InputAction.CallbackContext Context)
    {
        if (IsGround()) _RigidBody.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
    }

    private bool IsGround()
    {
        return Physics.Raycast(transform.position, Vector3.down, DistanceToGround);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3.down * DistanceToGround));

    }

    public void Move()
    {
        float UpDown = _PlayerInputActions.Player.UpDown.ReadValue<float>();
        float RightLeft = _PlayerInputActions.Player.RightLeft.ReadValue<float>();

        _RigidBody.AddForce(new Vector3(RightLeft, 0, UpDown) * MovementSpeed, ForceMode.Force);
        _PlayerAnimationController.SetAnimationMove(new Vector2(RightLeft, UpDown));
    }

    private void FixedUpdate()
    {
        Move();

        SetRotation();
    }

    public void SetRotation()
    {
        Vector2 InputVector = _PlayerInputActions.Player.Aim.ReadValue<Vector2>();

        yaw += InputVector.x;
        pitch += InputVector.y;

        pitch = Mathf.Clamp(pitch, -60, 90);

        Quaternion SmoothRotation = Quaternion.Euler(pitch, yaw, 0);

        CameraAnchor.transform.rotation = Quaternion.Slerp(CameraAnchor.transform.rotation, SmoothRotation,
            RotationSmoothing * Time.fixedDeltaTime);

        

        SmoothRotation = Quaternion.Euler(0, yaw, 0);

        transform.rotation = Quaternion.Slerp(transform.rotation, SmoothRotation, RotationSmoothing * Time.fixedDeltaTime);
    }
}
