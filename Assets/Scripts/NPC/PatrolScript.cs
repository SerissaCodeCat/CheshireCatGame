using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolScript : MonoBehaviour
{

    public List<Vector3> PatrolPoints = new List<Vector3>();
    public bool Active = true;
    public bool disabled = false;

    [SerializeField] private float PatrolActiveDistance;
    [SerializeField] private float MaxSpeed = 10;
    [SerializeField] private float Acceleration;
    [SerializeField] private float PatrolDelay;
    [SerializeField] private float FieldOfView;
    private bool PlayerDetected = false;
    private int PatrolDestination = 0;
    private float currentSpeed = 0;
    private float timer = 0.0f;
    private Rigidbody2D NPCRigidBody;
    private GameObject player;
    private FieldOfView FovScript;
    private Vector2 movementValue;

    private void Awake()
    {

        PatrolPoints[0] = new Vector2(transform.position.x, transform.position.y);
        NPCRigidBody = GetComponent<Rigidbody2D>();
        FovScript = GetComponent<FieldOfView>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        //FovScript.SetOrigin(this.transform.position);
        //FovScript.SetAimDirrection(this.transform.forward);
        if(timer > 0.0f)
        {
            timer -= Time.deltaTime;
            NPCRigidBody.velocity = Vector3.zero;
            return;
        }
        var Distance = Vector2.Distance(NPCRigidBody.transform.position, player.transform.position);
        Active = (Distance <= PatrolActiveDistance) ? true : false;
        if (!disabled && Active)
        {
            Vector2 toNextPatrolPoint = PatrolPoints[PatrolDestination] - NPCRigidBody.transform.position;
            float distance = Vector2.Distance(NPCRigidBody.transform.position, PatrolPoints[PatrolDestination]);
            if ( distance <= 1.0f)
            {
                PatrolDestination++;
                if (PatrolDestination > PatrolPoints.Count-1)
                {
                    PatrolDestination = 0;
                }
                currentSpeed = 0.0f;
                timer = PatrolDelay;
            }
            movementValue = toNextPatrolPoint.normalized;
            float topSpeed;
            if (distance < 7.0f)
            {
                topSpeed = MaxSpeed - (MaxSpeed - distance); 
            }
            else
            {
                topSpeed = MaxSpeed;
            }
            if (currentSpeed < topSpeed)
            {
                currentSpeed += Acceleration * Time.deltaTime;
                if(currentSpeed > MaxSpeed)
                {
                    currentSpeed = MaxSpeed;
                }
            }
            else if (currentSpeed > topSpeed)
            {
                currentSpeed -= Acceleration * Time.deltaTime;
            }
            NPCRigidBody.velocity = movementValue * currentSpeed;
        }
        else if (!Active)
        {
            NPCRigidBody.velocity = Vector3.zero;
        }
    }

    public void Disable()
    {
        disabled = true;
    }
    public void Enable()
    {
        disabled = false;
    }
}
