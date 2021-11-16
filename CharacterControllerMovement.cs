using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterControllerMovement : MonoBehaviour
{
    [SerializeField] public Transform player;
    [SerializeField] public Transform orientation;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 6f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float runBuildUp = 6f;
    [SerializeField] private float moveSmoothTime = 0.1f;
    [SerializeField] private KeyCode runKey = KeyCode.LeftShift;

    [Header("Jumping")]
    [SerializeField] private AnimationCurve jumpFallOff;
    [SerializeField] private float jumpMultiplier = 10f;
    [SerializeField] public KeyCode jumpKey;

    [Header("Slopes")]
    [SerializeField] private float slopeForce = 3f;
    [SerializeField] private float slopeForceRayLength = 1.5f;
    
    [Header("Other")]
    [SerializeField] private bool lockCursor = true;
    [Range(1,10)]
    [SerializeField] private float gravityMultiplier = 1f;
    [SerializeField] private bool useAnimation = false;
    private float gravityForce = 9.81f;
    
    private bool isJumping;
    private float movementSpeed;
    private CharacterController charController;

    private Vector2 movement = Vector2.zero;
    private Vector2 movementVelocity = Vector2.zero;

    float velocityY = 0.0f;
    
    void Start()
    {
        charController = GetComponent<CharacterController>();
        
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (charController == null)
        {
            Debug.LogWarning("no character controller attached");
        }
    }

    void Update()
    {
        DeclareMovementSpeeds();
        JumpInput();

        if (useAnimation)
        {
            PlayerAnimation();
        }
    }

    void FixedUpdate()
    {
        PlayerMovement();
    }

    private void PlayerMovement()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        
        Vector2 targetDirection = new Vector2(horizontalInput, verticalInput);
        targetDirection.Normalize();

        movement = Vector2.SmoothDamp(movement, targetDirection, ref movementVelocity, moveSmoothTime);

        if(charController.isGrounded)
        {
            velocityY = 0.0f;
        }

        velocityY -= (gravityForce * gravityMultiplier) * Time.fixedDeltaTime;
        Vector3 velocity = (orientation.forward * movement.y + orientation.right * movement.x) * movementSpeed + Vector3.up * velocityY;

        charController.Move(velocity * Time.deltaTime);

        if((verticalInput != 0 || horizontalInput != 0) && OnSlope())
        {
            charController.Move(Vector3.down * charController.height / 2 * slopeForce * Time.deltaTime);
        }

    }

    private void PlayerAnimation()
    {
       //animation would go here, if you were to implement it
    }

    private void DeclareMovementSpeeds()
    {
        if (Input.GetKey(runKey))
        {
            movementSpeed = Mathf.Lerp(movementSpeed, runSpeed, Time.deltaTime * runBuildUp);
        }   
        else
        {   
            movementSpeed = Mathf.Lerp(movementSpeed, walkSpeed, Time.deltaTime * runBuildUp);
        }
    }

    private bool OnSlope()
    {
        if(isJumping)
        {
            return false;
        }        

        RaycastHit hit;

        if(Physics.Raycast(transform.position, Vector3.down, out hit, charController.height / 2 * slopeForceRayLength))
        {
            if (hit.normal != Vector3.up)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        return false;
    }

    private void JumpInput()
    {
        if (Input.GetKey(jumpKey) && !isJumping)
        {
            isJumping = true;
            StartCoroutine(JumpEvent());
        }        
    }

    private IEnumerator JumpEvent()
    {
        charController.slopeLimit = 90.0f;
        float timeInAir = 0.0f;
        
        do 
        {
            float jumpForce = jumpFallOff.Evaluate(timeInAir);
            charController.Move(Vector3.up * jumpForce * jumpMultiplier * Time.deltaTime);
            timeInAir += Time.deltaTime;
            yield return null;
        }
        while (!charController.isGrounded && charController.collisionFlags != CollisionFlags.Above);
        
        charController.slopeLimit = 45.0f;
        isJumping = false;
        
    }
}
