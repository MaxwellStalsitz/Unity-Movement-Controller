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
    [SerializeField] public KeyCode jumpKey = KeyCode.Space;
    private bool canJump = true;

    [Header("Slopes")]
    [SerializeField] private float slopeForce = 3f;
    [SerializeField] private float slopeForceRayLength = 1.5f;
    
    [Header("Crouching")]
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;
    [SerializeField] private float crouchTime = 0.2f;
    private bool isCrouching;
    private float currentCrouchVelocity;
    private float currentHeight = 1f;

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
            Debug.LogWarning("No Character Controller Attached");
        }
    }

    void Update()
    {
        PlayerMovement();
        DeclareMovementSpeeds();
        JumpInput();
        CrouchInput();
        CrouchSmoothing();

        if (useAnimation)
        {
            PlayerAnimation();
        }
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

        velocityY -= (gravityForce * gravityMultiplier) * Time.deltaTime;
        Vector3 velocity = (orientation.forward * movement.y + orientation.right * movement.x) * movementSpeed + Vector3.up * velocityY;

        charController.Move(velocity * Time.deltaTime);

        if((verticalInput != 0 || horizontalInput != 0) && OnSlope())
        {
            charController.Move(Vector3.down * charController.height / 2 * slopeForce * Time.deltaTime);
        }

    }

    private void PlayerAnimation()
    {
       //animation would go here, if implementation were wanted
    }

    private void DeclareMovementSpeeds()
    {
        if(Input.GetKey(runKey))
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
        if (Input.GetKey(jumpKey) && !isJumping && canJump)
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

    private void CrouchInput()
    {
        if (!isJumping)
        {
            if (Input.GetKey(crouchKey))
            {
                if (!isCrouching)
                {
                    StartCoroutine(ChangeCrouching(true));
                }

                canJump = false;
            }
            else
            {
                if (isCrouching)
                {
                    StartCoroutine(ChangeCrouching(false));
                }

                canJump = true;
            }
        }
    }

    private IEnumerator ChangeCrouching(bool crouchChangeState)
    {
        if (!isCrouching)
        {
            isCrouching = true;
        }
        else
        {
            if (!Physics.Raycast(transform.position, Vector3.up, 1f))
            {
                isCrouching = false;
            }
        }

        yield return new WaitForSeconds(crouchTime * crouchTime);
        isCrouching = crouchChangeState;

    }

    private void CrouchSmoothing()
    {
        transform.localScale = new Vector3(transform.localScale.x, currentHeight, transform.localScale.z);
        
        if (isCrouching)
        {
            currentHeight = Mathf.SmoothDamp(currentHeight, 0.5f, ref currentCrouchVelocity, crouchTime);
        }
        else
        {
            currentHeight = Mathf.SmoothDamp(currentHeight, 1f, ref currentCrouchVelocity, crouchTime);
        }
    }

}
