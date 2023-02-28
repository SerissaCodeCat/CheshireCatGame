using UnityEngine;

public class bulletScript : MonoBehaviour
{
    private Rigidbody2D Self;
    public float lifetime = 10.0f;
    private PlayerAimAndFireWeapon aimandFireRefferance;
    private void Start()
    {
        Self = GetComponent<Rigidbody2D>();
        aimandFireRefferance = GameObject.Find("Chesenay").GetComponent<PlayerAimAndFireWeapon>();
        Self.velocity = transform.right * aimandFireRefferance.velocity;
    }
    void Update()
    {
        lifetime -= Time.deltaTime;
        if (lifetime <= 0.0f)
        {
            Destroy(gameObject);
        }
    }
}
