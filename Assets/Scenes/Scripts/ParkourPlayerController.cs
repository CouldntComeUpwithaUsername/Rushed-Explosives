using UnityEngine;

public class ParkourPlayerController : MonoBehaviour
{
    CharacterController characterController;

    public Transform groundCheck;

    public LayerMask groundMask;
    public LayerMask wallMask;
    public Camera playerCamera;

    Vector3 movement;
    Vector3 input;
    Vector3 Yvelocity;
    Vector3 forwardDirection;
    Vector3 wallNormal;
    Vector3 lastWallNormal;

    int jumpStocks = 1;

    public bool isGrounded;
    public bool isSprinting;
    public bool isCrouching;
    public bool isSliding;
    public bool isWallRunning;
    public bool onLeftWall;
    public bool onRightWall;
    public bool hasWallRun = false;
    public bool isClimibing;
    public bool canClimb;
    public bool hasClimbed;


   public float normalFOV;
   public float speed;
   public float gravity;

    public float wallRunSpeedIncrease;
    public float wallRunSpeedDecrease;
    public float slideSpeedIncrease;
    public float slideSpeedDecrease;
    public float runSpeed;
    public float sprintSpeed;
    public float crouchSpeed;
    public float airSpeed;
    public float climbSpeed;
    public float normalGravity;
    public float wallRunGravity;
    public float jumpHeight;
   public float slideTimer;
    public float maxSlideTimer;
    public float climbTimer;
    public float MaxClimbTimer;
    public float specialFOV;
    public float CameraChange;
    public float wallRuntilt;
    public float tilt;
    public float startHeight;
    public float crouchHeight = 0.5f;

    RaycastHit leftWallHit;
    RaycastHit rightWallHit;
    RaycastHit WallHit;


    Vector3 crouchingCenter = new Vector3 (0, 0.5f, 0);
    Vector3 standingCenter = new Vector3 (0, 0, 0);
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        startHeight = transform.localScale.y;
        normalFOV = playerCamera.fieldOfView;
    }

    void IncreaseSpeed(float speedIncrease)
    {
        speed += speedIncrease;
    }
    void DecreaseSpeed(float speedDecrease)
    {
        speed -= speedDecrease * Time.deltaTime;
    }
    void CameraEffects()
    {
        float fov = isWallRunning ? specialFOV : isSliding ? specialFOV : normalFOV;
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, fov, CameraChange * Time.deltaTime);

        if (isWallRunning)
        {
            if (onRightWall)
            {
                tilt = Mathf.Lerp(tilt, wallRuntilt, CameraChange * Time.deltaTime);
            }
            if (onLeftWall)
            {
                tilt = Mathf.Lerp(tilt, -wallRuntilt, CameraChange * Time.deltaTime);

            }
        }
        if (!isWallRunning) {
            tilt = Mathf.Lerp(tilt, 0f, CameraChange * Time.deltaTime);
        }
    }
    void HandleInput()
    {
        input = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));

        input = transform.TransformDirection(input);
        input = Vector3.ClampMagnitude(input, 1f);

        if(Input.GetKeyUp(KeyCode.Space) && jumpStocks > 0)
        {
            Jump();
        }

        if(Input.GetKeyDown(KeyCode.C))
        {
            Crouch();
        }
        if(Input.GetKeyUp(KeyCode.C))
        {
            ExitCrouch();
        }
        if(Input.GetKeyDown(KeyCode.LeftShift) && isGrounded)
        {
            isSprinting = true;
        }
        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            isSprinting = false;
        }

    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
        CheckWallRun();
        CheckClimbing();
        if (isGrounded && !isSliding)
        {
            GroundedMovement();
        }
        else if (!isGrounded && !isWallRunning && !isClimibing)
        {
            AirMovement();
        }
        else if (isSliding)
        {
            SlideMovement();
            DecreaseSpeed(slideSpeedDecrease);
            slideTimer -= 1f * Time.deltaTime;
            if (slideTimer < 0)
            {
                isSliding = false;
            }
        }
        else if (isWallRunning)
        {
            WallRunMovement();
            DecreaseSpeed(wallRunSpeedDecrease);
        }
        else if (isClimibing)
        {
            ClimbMovement();
            climbTimer -= 1f * Time.deltaTime;
            if (climbTimer < 0)
            {
                isClimibing = false;
                hasClimbed = true;
            }
        }
        

            checkGround();
        characterController.Move(movement * Time.deltaTime);
        ApplyGravity();
        CameraEffects();
    }
    void GroundedMovement()
    {
        speed = isSprinting ? sprintSpeed : isCrouching ? crouchSpeed : runSpeed;

        if (input.x != 0)
        {
            movement.x += input.x * speed;
        }
        else
        {
            movement.x = 0;
        }

        if (input.z != 0)
        {
            movement.z += input.z * speed;
        }
        else { 
            movement.z = 0;
        }

        movement = Vector3.ClampMagnitude(movement, speed);
    }

    void AirMovement()
    {
        movement.x += input.x * airSpeed;
        movement.z += input.z * airSpeed;

        movement = Vector3.ClampMagnitude(movement, speed);

    }

    void SlideMovement()
    {
        movement += forwardDirection;
        movement = Vector3.ClampMagnitude(movement, speed);
           
    }

    void WallRunMovement()
    {
        if (input.z > (forwardDirection.z - 10f) && input.z < (forwardDirection.z +10f))
        {
            movement += forwardDirection;
        }
        else if (input.z <  (forwardDirection.z - 10f) && input.z > (forwardDirection.z + 10f))
        {
            movement.x = 0f;
            movement.z = 0f;
            ExitWallRun();

        }
        movement.x += input.x * airSpeed;
        movement = Vector3.ClampMagnitude(movement, speed);
    }

    void ClimbMovement() 
    {
    forwardDirection = Vector3.up;
        movement.x += input.x * airSpeed;
        movement.z += input.z * airSpeed;

        Yvelocity += forwardDirection;
        speed = climbSpeed;

        movement = Vector3.ClampMagnitude(movement, speed);
        Yvelocity = Vector3.ClampMagnitude(Yvelocity, speed);
    }

    void checkGround()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, groundMask);
        if (isGrounded) 
        {
            jumpStocks = 1;
            hasWallRun = false;
            hasClimbed = false;
            climbTimer = MaxClimbTimer;
        }
    }

    void CheckWallRun() 
    {
        onLeftWall = Physics.Raycast(transform.position, -transform.right, out leftWallHit, 0.7f, wallMask);
        onRightWall = Physics.Raycast(-transform.position, -transform.right, out rightWallHit, 0.7f, wallMask);

        if((onRightWall || onLeftWall) && !isWallRunning)
        {
            TestWallRun();
        }
        if((!onRightWall || !onLeftWall) && isWallRunning)
        {
            ExitWallRun();
        }

    }

    void CheckClimbing() 
    {
        canClimb = Physics.Raycast(transform.position, transform.forward, out WallHit, 0.7f, wallMask);
        float wallAngle = Vector3.Angle(-WallHit.normal, transform.forward);
        if (wallAngle > 15 && !hasClimbed && canClimb) 
        {
        isClimibing = true;
        }
        else 
        {
        isClimibing = false;
        }
    }

    void TestWallRun()
    {
        wallNormal = onLeftWall ? leftWallHit.normal : rightWallHit.normal;
        if (hasWallRun)
        {
            float wallAngle = Vector3.Angle(wallNormal, lastWallNormal);
            if (wallAngle > 15)
            {
                WallRun();
            }
        }
        else
        {
            WallRun(); 
            hasWallRun = true;
        }
    }
    void ApplyGravity()
    {
        gravity = isWallRunning ? wallRunGravity : isClimibing ? 0f : normalGravity;
        Yvelocity.y += gravity * Time.deltaTime;
        characterController.Move(Yvelocity * Time.deltaTime);
    }
    void Jump()
    {
        if (!isGrounded && !isWallRunning ) 
        {
            jumpStocks -= 1;
        }
        else if (isWallRunning)
        {
            ExitWallRun();
            IncreaseSpeed(wallRunSpeedIncrease);
        }
            hasClimbed = false;
            climbTimer = MaxClimbTimer;
            Yvelocity.y = Mathf.Sqrt(jumpHeight * -2f * normalGravity);
    }

    void Crouch()
    {
        characterController.height = crouchHeight;
        characterController.center = crouchingCenter;
        transform.localScale = new Vector3(transform.localScale.x, crouchHeight, transform.localScale.z);
        isCrouching = true;
        if (speed > runSpeed)
        {
            isSliding = true;
            forwardDirection = transform.forward;
            if (isGrounded)
            {
            IncreaseSpeed(slideSpeedIncrease);
            }

            slideTimer = maxSlideTimer;
        }
    }
    void ExitCrouch()
    {
        characterController.height = startHeight * 2;
        characterController.center = standingCenter;
        transform.localScale = new Vector3(transform.localScale.x, startHeight, transform.localScale.z);
        isCrouching = false;
        isSliding = false;
    }

    void WallRun()
    {
        isWallRunning = true;
        jumpStocks = 1;
        IncreaseSpeed(wallRunSpeedIncrease);
        Yvelocity = new Vector3(0f, 0f, 0f);

        
        forwardDirection = Vector3.Cross(wallNormal, Vector3.up);

        if (Vector3.Dot(forwardDirection, transform.forward) < 0)
        {
            forwardDirection = -forwardDirection;
        }

    }

    void ExitWallRun()
    {
        isWallRunning = false;
        lastWallNormal = wallNormal;
    }


}

