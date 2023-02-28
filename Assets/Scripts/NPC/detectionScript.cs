using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class detectionScript : MonoBehaviour
{

    public float detectionRange;
    public float detectionCooldown;
    private float cooldownTimer = 0.0f;
    private bool playerDetected = false;
    private GameObject player;
    private PlayerMovement movementScriptRef;
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        movementScriptRef = player.GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!(cooldownTimer <= 0.0f))
        {

        }
    }
    private bool runDetection()
    {
        playerDetected = false;
        if (Vector2.Distance(transform.position, player.transform.position) < detectionRange)
        {
            Vector2 dirrection = (transform.position - player.transform.position).normalized;
            RaycastHit2D hit = Physics2D.Linecast(transform.position, dirrection);
            if(hit.collider.gameObject.CompareTag("Player"))
            {
                playerDetected = true;
                cooldownTimer = detectionCooldown;
            }
        }
        return playerDetected;
    }
}
