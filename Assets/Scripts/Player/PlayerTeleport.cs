using UnityEngine;
public class PlayerTeleport : MonoBehaviour
{
    public float movementSpeed;
    private GameObject player;
    public float Range;
    private PlayerMovement movementScriptRefferance;
    private PLayerControls controls;
    private Transform playerTeleport;
    private SpriteRenderer playerTeleportSprite;
    private Vector2 teleportlocationMovement;

    private bool teleportActive = false;

    private Color stealthedTransparencey = new Color(1.0f, 1.0f, 1.0f, 0.35f);
    private void Awake()
    {
        playerTeleport = transform.Find("TeleportTarget");
        player = GameObject.FindGameObjectWithTag("Player");
        movementScriptRefferance = GetComponent<PlayerMovement>();
        controls = movementScriptRefferance.Controls;
        playerTeleportSprite = playerTeleport.GetComponent<SpriteRenderer>();
        playerTeleportSprite.enabled = false;

        controls.Gameplay.Teleport.performed += ctx => startTeleporting();
        controls.Gameplay.Teleport.canceled += ctx => endTeleporting();

        controls.Gameplay.Action.performed += ctx => teleport();

        controls.Gameplay.Movement.performed += ctx => teleportlocationMovement = ctx.ReadValue<Vector2>();
        controls.Gameplay.Movement.canceled += ctx => teleportlocationMovement = Vector2.zero;
    }

    void Update()
    {
            updateTeleportTargetLocation();
    }
    private void startTeleporting()
    {
        if (!teleportActive && movementScriptRefferance.teleporting)
        {
            if (playerTeleportSprite != null)
            {
                teleportActive = true;
                playerTeleportSprite.enabled = true;
                playerTeleportSprite.color = stealthedTransparencey;
            }
        }
    }

    private void endTeleporting()
    {
        if (playerTeleportSprite != null)
        {
            teleportActive = false;
            playerTeleportSprite.enabled = false;
            playerTeleport.transform.localPosition = new Vector3(2.0f, 0.0f, 0);
        }
    }

    private void teleport()
    {
        if (teleportActive)
        {
            playerTeleportSprite.enabled = false;
            movementScriptRefferance.TeleportToLocation(playerTeleport.transform.position.x, playerTeleport.transform.position.y);
            movementScriptRefferance.setStaminaTo(0.00f);
        }
    }

    private void updateTeleportTargetLocation()
    {
        if(!movementScriptRefferance.teleporting && teleportActive)
        {
            endTeleporting();
        }
        if (teleportActive)
        {
            var desiredX = playerTeleport.transform.position.x + ((teleportlocationMovement.x * movementSpeed) * Time.deltaTime);
            var desiredY = playerTeleport.transform.position.y + ((teleportlocationMovement.y * movementSpeed) * Time.deltaTime);
            var desiredLocationRange = Vector2.Distance( new Vector2 (desiredX, desiredY), player.transform.position);

            if (desiredLocationRange <= Range)
            {
                if (movementScriptRefferance.facingRight)
                {
                    playerTeleport.transform.localPosition = new Vector2(playerTeleport.transform.localPosition.x + (teleportlocationMovement.x * movementSpeed) * Time.deltaTime,
                        playerTeleport.transform.localPosition.y + (teleportlocationMovement.y * movementSpeed) * Time.deltaTime);
                }
                else
                {
                    playerTeleport.transform.localPosition = new Vector2(playerTeleport.transform.localPosition.x + (-teleportlocationMovement.x * movementSpeed) * Time.deltaTime,
                        playerTeleport.transform.localPosition.y + (teleportlocationMovement.y * movementSpeed) * Time.deltaTime);
                }
            }
            else
            {
                if (movementScriptRefferance.facingRight)
                {
                    playerTeleport.transform.localPosition = new Vector2(playerTeleport.transform.localPosition.x + (teleportlocationMovement.x * movementSpeed) * Time.deltaTime,
                        playerTeleport.transform.localPosition.y + (teleportlocationMovement.y * movementSpeed) * Time.deltaTime).normalized * Range;
                }
                else
                {
                    playerTeleport.transform.localPosition = new Vector2(playerTeleport.transform.localPosition.x +(-teleportlocationMovement.x * movementSpeed) * Time.deltaTime,
                        playerTeleport.transform.localPosition.y + (teleportlocationMovement.y * movementSpeed) * Time.deltaTime).normalized * Range;
                }
            }
        }
    }

}