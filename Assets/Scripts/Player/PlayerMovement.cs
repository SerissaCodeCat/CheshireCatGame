using UnityEngine;
public class PlayerMovement : MonoBehaviour
{
    public PLayerControls Controls;
    [SerializeField] private int playerJumpPower;
    private Rigidbody2D playerRigidBody;
    private Animator playerAnimator;
    public bool facingRight = true;
    public Vector2 movementValue;
    [SerializeField] private float defaultSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float deceleration;
    [SerializeField] private float runMultiplier;
    [SerializeField] private float sneakMultiplier;
    [SerializeField] private float wallSlowdownSpeed;

    public float Stamina { get; set; } = 10;
    [SerializeField] private float Gravity = 2.0f;
    public bool canDoubleJump = false;
    public bool Aiming = false;
    public bool stealthed = false;
    public bool teleporting = false;

    private float currentSpeed = 0.0f;
    private float verticalSlowDown = 0.0f;
    private bool hasJumped = false;
    private bool isAirborn = false;
    private float MaxStamina = 10;
    private float MinStamina = 0;
    private Color stealthedTransparencey = new Color(1.0f, 1.0f, 1.0f, 0.35f);
    private Color normalTransparencey = new Color(1.0f, 1.0f, 1.0f, 1.00f);
    private bool clinging = false;
    private bool climbableObjectOnRight = false;
    private SpriteRenderer sprite;
    private characterHealth healthScriptRef;
    private bool movementEnabled = true;
    private bool holdInPlace = false;


    void Awake()
    {
        Controls = new PLayerControls();
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;


        sprite = (SpriteRenderer)transform.GetComponent("SpriteRenderer");
        healthScriptRef = GetComponent<characterHealth>();
        playerRigidBody = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();

        Controls.Gameplay.Action.performed += ctx => jump();
        
        Controls.Gameplay.Movement.performed += ctx => movementValue = ctx.ReadValue<Vector2>();
        Controls.Gameplay.Movement.canceled += ctx => movementValue = Vector2.zero;

        Controls.Gameplay.Sneak.performed += ctx => stealthOn();
        Controls.Gameplay.Sneak.canceled += ctx => stealthOff();

        Controls.Gameplay.Teleport.performed += ctx => startTeleporting();
        Controls.Gameplay.Teleport.canceled += ctx => stopTeleporting(); 

        Controls.Gameplay.Aim.performed += ctx => 
        {
            Aiming = true;
            currentSpeed = 0.0f;
            if (!hasJumped)
            {
                playerRigidBody.velocity = new Vector2(currentSpeed, playerRigidBody.velocity.y);
            }
        };
        Controls.Gameplay.Aim.canceled += ctx => { Aiming = false; };
    }
    private void OnEnable()
    {
        Controls.Gameplay.Enable();
    }
    private void OnDisable()
    {
        Controls.Gameplay.Disable();
    }
    void Update()
    {
        if(holdInPlace)
        {
            playerRigidBody.velocity = new Vector2(0.0f, 0.0f);
            return;
        }
        if (!Aiming)
        { 
                movement();
        }
        if (stealthed)
        {
            Stamina -= Time.deltaTime;
        }
        if (clinging)
        {
            Stamina -= Time.deltaTime;
        }
        if (Stamina <= MinStamina)
        {
            if (clinging)
            {
                Drop();
            }
            stopTeleporting();
        }

        if (!stealthed && !clinging)
        {
            if (Stamina < MaxStamina)
            {
                Stamina += Time.deltaTime;
                if (Stamina > MaxStamina)
                {
                    Stamina = MaxStamina;
                }
            }

        }
    }
    private void movement()
    {
        currentSpeed = playerRigidBody.velocity.x;
        if (!clinging)
        {
            if(!movementEnabled)
            {
                movementValue.x = 0.0f;
                movementValue.y = 0.0f;
            }
            if(holdInPlace)
            {
                movementValue.x = 0.0f;
                movementValue.y = 0.0f;
                currentSpeed = 0.0f;
            }
            if (movementValue.x > 0.0f && currentSpeed < (stealthed ? defaultSpeed * sneakMultiplier : defaultSpeed))
            {
                if (currentSpeed < 0.0f)
                {
                    currentSpeed += (acceleration * 2.5f) * Time.deltaTime;
                }
                else
                {
                    currentSpeed += acceleration * Time.deltaTime;
                }
                if(currentSpeed > (stealthed ? defaultSpeed * sneakMultiplier : defaultSpeed))
                {
                    currentSpeed = (stealthed ? defaultSpeed * sneakMultiplier : defaultSpeed);
                }
            }
            else if (movementValue.x < 0.0f && currentSpeed > (0 - (stealthed ? defaultSpeed * sneakMultiplier : defaultSpeed)))
            {
                if (currentSpeed > 0.0f)
                {
                    currentSpeed -= (acceleration * 2.5f) * Time.deltaTime;
                }
                else
                {
                    currentSpeed -= acceleration * Time.deltaTime; ;
                }
                if (currentSpeed < 0.0f - (stealthed ? defaultSpeed * sneakMultiplier : defaultSpeed))
                {
                    currentSpeed = 0.0f - (stealthed ? defaultSpeed * sneakMultiplier : defaultSpeed);
                }
            }
            else if (movementValue.x == 0.0f && currentSpeed != 0.0f)
            {
                if (currentSpeed > 0.5f)
                {
                    currentSpeed -= (isAirborn? deceleration / 2 : deceleration) * Time.deltaTime;
                    if(currentSpeed < 0.0f)
                    {
                        currentSpeed = 0.0f;
                    }
                }
                else if (currentSpeed < -0.5f)
                {
                    currentSpeed += (isAirborn ? deceleration / 2 : deceleration) * Time.deltaTime;
                    if (currentSpeed > 0.0f)
                    {
                        currentSpeed = 0.0f;
                    }
                }
                else
                {
                    currentSpeed = 0.0f;
                }
            }
            playerRigidBody.velocity = new Vector2(currentSpeed, playerRigidBody.velocity.y);
            if (movementValue.x < 0.0f && facingRight)
            {   
                if (currentSpeed >= 7.25f)
                {
                    //play turning around animation
                }
                flipSprite();
            }
            else if (movementValue.x > 0.0f && !facingRight)
            {    
                if (currentSpeed >= 7.25f)
                {
                    //play turning around animation
                }
                flipSprite();
            }
        }
        else
        {
            if(playerRigidBody.velocity.y < -0.5f)
            {
                verticalSlowDown = playerRigidBody.velocity.y + wallSlowdownSpeed * Time.deltaTime;
            }
            else
            {
                verticalSlowDown = 0.0f;
            }
            playerRigidBody.velocity = new Vector2(0.0f, verticalSlowDown);

            if (movementValue.y < -0.9f)
            {
                Drop();
            }
        }
    }
    private void jump()
    {
        if (!teleporting && !Aiming)
        {
            if (!clinging)
            {
                if (!hasJumped)
                {
                    hasJumped = true;
                    playerRigidBody.velocity = new Vector2(currentSpeed, ((stealthed ? sneakMultiplier : 1.0f) * playerJumpPower));
                }
                else if (canDoubleJump && hasJumped)
                {
                    canDoubleJump = false;
                    playerRigidBody.velocity = new Vector2(currentSpeed, ((stealthed ? sneakMultiplier : 1.0f) * playerJumpPower));
                }
            }
            else
            {
                endClinging();
                if (climbableObjectOnRight)
                {
                    hasJumped = true;
                    currentSpeed = (stealthed ? -(sneakMultiplier * defaultSpeed) : -defaultSpeed) * 2.5f;
                    playerRigidBody.velocity = new Vector2(currentSpeed, (stealthed ? (sneakMultiplier*2) : 2.0f) * playerJumpPower);
                }
                else
                {
                    hasJumped = true;
                    currentSpeed = (stealthed ? (sneakMultiplier*defaultSpeed) : defaultSpeed) * 2.5f;
                    playerRigidBody.velocity = new Vector2(currentSpeed, (stealthed ? (sneakMultiplier * 2) : 2.0f) * playerJumpPower);
                }
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "jumpReset")
        {
            if (Aiming)
            {
                currentSpeed = 0.0f;
                playerRigidBody.velocity = new Vector2(0.0f, 0.0f);
            }
            for (int i = 0; i < collision.contactCount; i++)
            {
                ContactPoint2D contact = collision.GetContact(i);
                if (contact.normal.y > 0.90f)
                {
                    isAirborn = false;
                    canDoubleJump = true;
                    hasJumped = false;
                }
            }
        }
        else if(collision.gameObject.tag == "Climbable")
        {
            startClinging(collision);
            isAirborn = false;
        }
        else if(collision.gameObject.tag == "basicHazard")
        {
            Vector2 heading = collision.gameObject.transform.position - transform.position;
            healthScriptRef.ReduceHealthBy(20);
            bounceBack(heading);
        }
    }
    private void bounceBack(Vector2 dirrection)
    {
        if (dirrection.y < 0.0f)
        {
            if (dirrection.x > 0.0f)
            {
                playerRigidBody.velocity = new Vector2(-15.0f, 15.0f);
            }
            else if (dirrection.x < 0.0f)
            {
                playerRigidBody.velocity = new Vector2(15.0f, 15.0f);
            }
            else
            {
                if (facingRight)
                {
                    playerRigidBody.velocity = new Vector2(-15.0f, 15.0f);
                }
                else
                {
                    playerRigidBody.velocity = new Vector2(15.0f, 15.0f);
                }
            }
        }
        else if (dirrection.y > 0.0f)
        {
            if (dirrection.x > 0.0f)
            {
                playerRigidBody.velocity = new Vector2(-15.0f, -15.0f);
            }
            else if (dirrection.x > 0.0f)
            {
                playerRigidBody.velocity = new Vector2(15.0f, -15.0f);
            }
            else
            {
                if (facingRight)
                {
                    playerRigidBody.velocity = new Vector2(-15.0f, -15.0f);
                }
                else
                {
                    playerRigidBody.velocity = new Vector2(15.0f, -15.0f);
                }
            }
        }
        else
        {
            if (dirrection.x > 0.0f)
            {
                playerRigidBody.velocity = new Vector2(-15.0f, 0.0f);
            }
            else if (dirrection.x > 0.0f)
            {
                playerRigidBody.velocity = new Vector2(15.0f, 0.0f);
            }
            else
            {
                if (facingRight)
                {
                    playerRigidBody.velocity = new Vector2(-15.0f, 0.0f);
                }
                else
                {
                    playerRigidBody.velocity = new Vector2(15.0f, 0.0f);
                }
            }
        }

    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if(!isAirborn && collision.gameObject.tag != "Climbable")
        {
            isAirborn = true;
        }

    }
    private void stealthOn()
    {
        if (!stealthed && Stamina >= 2.00)
        {
            stealthed = true;
            if (sprite != null)
            {
                sprite.color = stealthedTransparencey;
            }
        }
    }
    private void stealthOff()
    {
        stealthed = false;
        if (sprite != null)
        {
            sprite.color = normalTransparencey;
        }
    }
    private void startTeleporting()
    {
        if (Stamina >= 5.00 && !stealthed)
        {
            stealthOn();
            playerRigidBody.gravityScale = 0;
            playerRigidBody.velocity = new Vector2(0.0f, 0.0f);
            Aiming = true;
            teleporting = true;
        }
    }
    private void stopTeleporting()
    {
        stealthOff();
        playerRigidBody.gravityScale = Gravity;
        Aiming = false;
        teleporting = false;
    }
    private void startClinging(Collision2D collision)
    {
        if(isAirborn)
        { 
            if (Stamina >= 1.0f)
            {
                clinging = true;
                canDoubleJump = true;
                hasJumped = false;
                playerRigidBody.gravityScale = 0;
                verticalSlowDown = 0.0f; 
                if(playerRigidBody.velocity.y < -0.5f)
                {
                    verticalSlowDown = playerRigidBody.velocity.y + wallSlowdownSpeed;
                }
                playerRigidBody.velocity = new Vector2(0.0f, verticalSlowDown);
                for (int i = 0; i < collision.contactCount; i++)
                {
                    ContactPoint2D contact = collision.GetContact(i);
                    Vector2 heading = contact.point - (Vector2)transform.position;
                    Vector2 dirrection = heading / heading.magnitude;
                    climbableObjectOnRight = (dirrection.x > 0.0f) ? true : false;
                }

                if (climbableObjectOnRight && facingRight)
                {
                    flipSprite();
                }
                else if (!climbableObjectOnRight && !facingRight)
                {
                    flipSprite();
                }
            }
        }
    }
    private void endClinging()
    {
        clinging = false;
        isAirborn = true;
        playerRigidBody.gravityScale = Gravity;
    }
    private void flipSprite()
    {
        facingRight = !facingRight;
        playerRigidBody.transform.Rotate(0.0f, 180.0f, 0.0f);
    }
    private void Drop()
    {
        if (climbableObjectOnRight)
        {
            playerRigidBody.MovePosition(new Vector2(playerRigidBody.position.x - 0.1f, playerRigidBody.position.y));
            currentSpeed = -0.5f * playerJumpPower;
            playerRigidBody.velocity = new Vector2(currentSpeed, 0.0f);
        }
        else
        {
            playerRigidBody.MovePosition(new Vector2(playerRigidBody.position.x + 0.1f, playerRigidBody.position.y));
            currentSpeed = 0.5f * playerJumpPower;
            playerRigidBody.velocity = new Vector2(currentSpeed, 0.0f);
        }
        endClinging();
    }
    public void TeleportToLocation(float x, float y)
    {
        playerRigidBody.transform.position = new Vector3(x, y, playerRigidBody.transform.position.z);
    }
    public void setStaminaTo( float desiredStamina)
    {
        Stamina = desiredStamina;
    }
    public void DisableMovement()
    {
        movementEnabled = false;
    }
    public void EnableMovement()
    {
        movementEnabled = true;
        movementValue = Controls.Gameplay.Movement.ReadValue<Vector2>();
    }
    public void EnableHoldInPlace()
    {
        holdInPlace = true;
        currentSpeed = 0.0f;
    }
    public void disableHoldInPlace()
    {

    }
}
