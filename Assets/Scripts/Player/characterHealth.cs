using UnityEngine;
using UnityEngine.SceneManagement;
public class characterHealth : MonoBehaviour
{
    public bool hasDied = false;
    private int maxHealth = 100;
    private int health = 100;
    private const float invulnerabilityTime = 1.4f;
    private float damageTimer = 0.0f;
    private PlayerMovement moveScriptRef;
    private void Awake()
    {
        moveScriptRef = GetComponent<PlayerMovement>();
    }
    // Update is called once per frame
    void Update()
    {
        if(damageTimer >= 0.0f)
        {
            damageTimer -= Time.deltaTime;
            if (damageTimer <= 0.0f)
            {
                moveScriptRef.EnableMovement();
            }
        }
        if (hasDied || gameObject.transform.position.y <= -7.0 || health <= 0.0f)
        {
            die();
        }
    }
    public void ReduceHealthBy(int deductionValue)
    {
        if (damageTimer <= 0.0f)
        {
            health -= deductionValue;
            damageTimer = invulnerabilityTime;
            moveScriptRef.DisableMovement();
        }
    }
    public void IncreaseHealthBy(int recoveryValue)
    {
        health += recoveryValue;
        if(health > maxHealth)
        {
            health = maxHealth;
        }
    }
    void die()
    {
        hasDied = false;
        SceneManager.LoadScene("Level1");
    }
}
