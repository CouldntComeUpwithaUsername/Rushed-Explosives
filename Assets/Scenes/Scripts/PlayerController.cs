using UnityEngine;

[DefaultExecutionOrder(-1)]
public class PlayerController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Components")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Camera playerCamera;

    [Header("Base Movement")]
    public float runAcceleration = .25f;
    public float runSpeed = 4f;
    public float drag = .1f;

    [Header ("Camera Settings")]
    public float lookSenseH = .1f;
    public float lookSenseV = .1f;
    public float lookLimitV = 89f;

    private PlayerLocomotionInput playerLocomotionInput;
    private Vector2 cameraRotation = Vector2.zero;
    private Vector2 playerTargetRotation = Vector2.zero;

    private void Awake()
    {
        playerLocomotionInput = GetComponent<PlayerLocomotionInput>();
    }

    private void Update()
    {
        Vector3 cameraForwardXZ = new Vector3(playerCamera.transform.forward.x,0f, playerCamera.transform.forward.z).normalized;
        Vector3 cameraRightXZ = new Vector3(playerCamera.transform.right.x, 0f, playerCamera.transform.right.z).normalized;
        Vector3 movementDirection = cameraRightXZ * playerLocomotionInput.MovementInput.x + cameraForwardXZ * playerLocomotionInput.MovementInput.y;

        Vector3 movementDelta = movementDirection * runAcceleration * Time.deltaTime;
        Vector3 newVelocity = characterController.velocity + movementDelta;

        Vector3 currentDrag = newVelocity.normalized * drag * Time.deltaTime;
        newVelocity = (newVelocity.magnitude > drag * Time.deltaTime) ? newVelocity - currentDrag : Vector3.zero;
        newVelocity = Vector3.ClampMagnitude(newVelocity, runSpeed);

        characterController.Move(newVelocity *Time.deltaTime);
    }
    private void LateUpdate()
    {
        cameraRotation.x += lookSenseH * playerLocomotionInput.LookInput.x;
        cameraRotation.y = Mathf.Clamp(cameraRotation.y - lookSenseH * playerLocomotionInput.LookInput.y, -lookLimitV, lookLimitV);

        playerTargetRotation.x += transform.eulerAngles.x + lookSenseH * playerLocomotionInput.LookInput.x;
        transform.rotation = Quaternion.Euler(0f, playerTargetRotation.x, 0f);

        playerCamera.transform.rotation = Quaternion.Euler(cameraRotation.y, cameraRotation.x, 0f);
    }
}
