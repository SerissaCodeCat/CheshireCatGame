using UnityEngine;
public class CameraGuideSystem : MonoBehaviour
{
    private GameObject player;
    private GameObject teleportTarget;
    private PlayerMovement movementScriptRefferance;
    public int AdvanceOffset;
    public float SmoothSpeed;
    public float xMin;
    public float xMax;
    public float yMin;
    public float yMax;

    // Start is called before the first frame update
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        teleportTarget = GameObject.Find("TeleportTarget");
        movementScriptRefferance = player.GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if(movementScriptRefferance.teleporting)
        {
            float x = Mathf.Clamp(teleportTarget.transform.position.x, xMin, xMax);
            float y = Mathf.Clamp(teleportTarget.transform.position.y, yMin, yMax);
            Vector3 desiredPossition = new Vector3(x, y, transform.position.z);
            Vector3 smoothedPossition = Vector3.Lerp(transform.position, desiredPossition, (SmoothSpeed * 50) * Time.deltaTime);
            transform.position = smoothedPossition;
        }
    }
    void FixedUpdate()
    {
        if (!movementScriptRefferance.teleporting)
        {
            float x = Mathf.Clamp(player.transform.position.x + (movementScriptRefferance.facingRight ? AdvanceOffset : 0 - AdvanceOffset), xMin, xMax);
            float y = Mathf.Clamp(player.transform.position.y, yMin, yMax);
            Vector3 desiredPossition = new Vector3(x, y, transform.position.z);
            Vector3 smoothedPossition = Vector3.Lerp(transform.position, desiredPossition, SmoothSpeed);
            transform.position = smoothedPossition;
        }
    }
}
