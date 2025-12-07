using UnityEngine;

public class PlayerState : MonoBehaviour
{
    [field: SerializeField] public PlayerMovementState CurrentPlayerMovementState { get; private set; } = PlayerMovementState.Idling;

    public void SetPlayerMovementState(PlayerMovementState playerMovementState)
    {
        CurrentPlayerMovementState = playerMovementState;
    }
    public bool InGroundedState()
    {
        return CurrentPlayerMovementState == PlayerMovementState.Idling ||
            CurrentPlayerMovementState == PlayerMovementState.Running ||
            CurrentPlayerMovementState == PlayerMovementState.Sprinting;

    }
}
    public enum PlayerMovementState

    { 
        Idling = 0,
        Running = 1,
        Sprinting = 2,
        Jumping = 3,
        Sliding = 4,
        WallRunning = 5,
        Falling = 6,
        Crouching = 7,
    }

