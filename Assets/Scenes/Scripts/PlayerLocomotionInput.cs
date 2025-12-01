using FinalPlayerController;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLocomotionInput : MonoBehaviour, PlayerControls.IPlayerLocotmotionMapActions
{
   public PlayerControls PlayerControls {  get; private set; }
    public Vector2 MovementInput { get; private set; }

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
}
