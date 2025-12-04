using FinalPlayerController;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-2)]
public class PlayerLocomotionInput : MonoBehaviour, PlayerControls.IPlayerLocotmotionMapActions
{
    [SerializeField] private bool holdToSprint = true;

    public bool SprintToggledOn { get; private set; }
    public PlayerControls PlayerControls { get; private set; }
    public Vector2 MovementInput { get; private set; }
    public Vector2 LookInput { get; private set; }

    private void OnEnable()
    {
        PlayerControls = new PlayerControls();
        PlayerControls.Enable();

        PlayerControls.PlayerLocotmotionMap.Enable();
        PlayerControls.PlayerLocotmotionMap.SetCallbacks(this);
    }

    private void OnDisable()
    {
        PlayerControls.PlayerLocotmotionMap.Disable();
        PlayerControls.PlayerLocotmotionMap.RemoveCallbacks(this);
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        MovementInput = context.ReadValue<Vector2>();
        print(MovementInput);

    }

    public void OnLook(InputAction.CallbackContext context)
    {
        LookInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnToggleSprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SprintToggledOn = holdToSprint || !SprintToggledOn;
        }
        else if (context.canceled)
        {
            SprintToggledOn = !holdToSprint && !SprintToggledOn;
        }
    }
}
