using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    private CharacterController _cc;
    [SerializeField] private Transform Player;
    [SerializeField] private Transform pitchTarget;
    [SerializeField] private Camera playerCamera;

    private Vector2 _input;
    private bool jumpPressed;
    private bool sprintHeld;

    [Header("Movement")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float gravity = -9.81f;

    private float yVelocity;    //players y velocity when jumping and falling

    [Header("Ground Check")]
    public float rayDistance = 1.1f;                        //ground check is difficult so fix it later on
    public LayerMask groundLayer;

    [Header("Mouse Look")]
    [SerializeField] private float sensitivity = 0.1f;      //mouse sensitivity 
    [SerializeField] private float minPitch = -80f;         //has issues when falling so fix it
    [SerializeField] private float maxPitch = 80f;          //has issues when falling so fix it

    private float pitch;

    [Header("HeadBob and Roll")]
    [SerializeField] private float bobFrequency = 1.8f;
    [SerializeField] private float bobYawAmplitude = 0.6f;
    [SerializeField] private float bobPitchAmplitude = 0.8f;
    [SerializeField] private float bobEase = 10f;
    [SerializeField] private float moveThreshold = 0.1f;    

    [SerializeField] private float rollAmount = 2f;         //max roll anlge for strafing
    [SerializeField] private float rollEase = 12f;          //smoothing

    [Header("Fall Damping")]
    [SerializeField] private float maxFallPitch = -88f;        // negative to rotate upwards
    [SerializeField] private float fallStartVel = -2f;        // effect starts at this negative fall speed
    [SerializeField] private float fallMaxVel = -25f;         // full effect by this fall speed
    [SerializeField] private float fallEase = 4f;             // smoothing speed

    private float fallPitchCurrent;
    private float baseYaw;
    private float bobTime;
    private float bobYawCurrent;
    private float bobPitchCurrent;
    private float rollCurrent;
    private float bobWeight;

    [Header("Sprint")]
    [SerializeField] private float sprintMultiplier = 2f;
    [SerializeField] private float sprintFovIncrease = 8f;
    [SerializeField] private float sprintEase = 10f;

    private float sprintBlend;
    private float baseFov;


    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (playerCamera == null){playerCamera = GetComponentInChildren<Camera>();}
        if (playerCamera != null){baseFov = playerCamera.fieldOfView;}
        _cc = GetComponent<CharacterController>();
    }

    private void Update()
    {

        //ground check
        bool isGrounded = CheckGrounded();

        if (isGrounded && yVelocity < 0f){yVelocity = -2f;}
        if (isGrounded && jumpPressed){yVelocity = Mathf.Sqrt(jumpForce * -2f * gravity);jumpPressed = false;}
        if (!isGrounded){yVelocity += gravity * Time.deltaTime;}
        Debug.DrawRay(transform.position, Vector3.down * rayDistance, isGrounded ? Color.green : Color.red); //visualize the ray

        //Move
        Vector3 forward = Player.forward;
        Vector3 right = Player.right;

        forward.y = 0f; right.y = 0f;
        forward.Normalize(); 
        right.Normalize();

        yVelocity += gravity * Time.deltaTime;
        Vector3 move = right * _input.x + forward * _input.y;

        float targetSprint = sprintHeld ? 1f : 0f;
        sprintBlend = Mathf.Lerp(sprintBlend, targetSprint, Time.deltaTime * sprintEase);

        float currentSpeed = speed * Mathf.Lerp(1f, sprintMultiplier, sprintBlend);
        Vector3 horizontalMove = move * currentSpeed;
        Vector3 verticalMove = Vector3.up * yVelocity;
        _cc.Move((horizontalMove + verticalMove) * Time.deltaTime);


        //jump
        if (isGrounded && jumpPressed)
        {
            yVelocity = Mathf.Sqrt(jumpForce * -2f * gravity);
            jumpPressed = false;
        }
        UpdateHeadMotion(isGrounded);
        

        //FOV
        if (playerCamera != null)
        {
            float targetFov = baseFov + sprintFovIncrease * sprintBlend;
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFov, Time.deltaTime * sprintEase);
        }


        UpdateFallDamping(isGrounded);
    }
    

    //mouse input
    public void Look(InputAction.CallbackContext ctx)
    {
        Vector2 delta = ctx.ReadValue<Vector2>();
        ApplyLook(delta);
    }
    //mouse fix
    private void ApplyLook(Vector2 delta)
    {
        float yawDelta = delta.x * sensitivity;
        float pitchDelta = -delta.y * sensitivity;

        baseYaw += yawDelta;
        pitch = math.clamp(pitch + pitchDelta, minPitch, maxPitch);
    }

    //head bob and roll effect
    //do not mind the monstosity i wrote here, im NOT wrting this again
    private void UpdateHeadMotion(bool grounded)
    {
        float dt = Time.deltaTime;

        float moveAmount = _input.magnitude;

        float targetWeight = (grounded && moveAmount > moveThreshold) ? 1f : 0f;
        bobWeight = Mathf.Lerp(bobWeight, targetWeight, dt * bobEase);

        if (bobWeight > 0.001f)
        {
            float sprintFreqMult = Mathf.Lerp(1f, 2f, sprintBlend);
            bobTime += dt * bobFrequency * sprintFreqMult * (0.8f + moveAmount);
        }

        float targetBobYaw = Mathf.Sin(bobTime) * bobYawAmplitude * bobWeight;
        float targetBobPitch = Mathf.Cos(bobTime * 2f) * bobPitchAmplitude * bobWeight;

        bobYawCurrent = Mathf.Lerp(bobYawCurrent, targetBobYaw, dt * bobEase);
        bobPitchCurrent = Mathf.Lerp(bobPitchCurrent, targetBobPitch, dt * bobEase);

        float targetRoll = 0f;
        if (_input.x > 0.01f) targetRoll = -rollAmount;
        else if (_input.x < -0.01f) targetRoll = rollAmount;

        rollCurrent = Mathf.Lerp(rollCurrent, targetRoll, dt * rollEase * Mathf.Lerp(1f,2f,sprintBlend));

        Player.localRotation = Quaternion.Euler(0f, baseYaw + bobYawCurrent, 0f);
        pitchTarget.localRotation = Quaternion.Euler(pitch + bobPitchCurrent + fallPitchCurrent,0f,rollCurrent);
    }

    // player input references
    //move input
    public void Move(InputAction.CallbackContext ctx){_input = ctx.ReadValue<Vector2>();}
    //jump input
    public void Jump(InputAction.CallbackContext ctx)
    {if (ctx.performed){jumpPressed = true;}}
    //sprint input
    public void Sprint(InputAction.CallbackContext ctx){sprintHeld = ctx.ReadValueAsButton();}


    //fall and land effect
    private void UpdateFallDamping(bool isGrounded)
    {
        float dt = Time.deltaTime;
        float target = 0f;
        if (!isGrounded && yVelocity < fallStartVel)
        {
            float t = Mathf.InverseLerp(fallStartVel, fallMaxVel, yVelocity);
            target = Mathf.Lerp(0f, maxFallPitch, t);
        }
        fallPitchCurrent = Mathf.Lerp(fallPitchCurrent, target, dt * fallEase);
    }

    //ground check with raycast (buggy)
    private bool CheckGrounded(){return Physics.Raycast(transform.position, Vector3.down, rayDistance, groundLayer);}





}
