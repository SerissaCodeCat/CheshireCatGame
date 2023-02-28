using System.Collections;
using UnityEngine;

public class PlayerAimAndFireWeapon : MonoBehaviour
{
    public GameObject BulletPrefab;
    public float velocity;
    public Vector2 AimDirrectionValue;
    public int resolution = 10;
    public float slingshotDelay = 3.0f;
    private float slingshotTimer = 0.0f;


    //private Transform PlayerAim;
    private PLayerControls controls;
    private Transform firepoint;
    private PlayerMovement movementScriptRefferance;
    private LineRenderer launcharc;
    private Renderer launcharcRenderer;
    private Transform playerLaunchArc;

    private Vector2 AimInput;
    private Vector3[] arcPoints;

    private float angle;
    private float g; //gravity
    private float radianAngle;
    private void Awake()
    {
        //PlayerAim = transform.Find("PlayerWeaponAim");
        firepoint = transform.Find("FirePoint");
        movementScriptRefferance = GetComponent<PlayerMovement>();
        playerLaunchArc = transform.Find("PlayerLaunchArc");
        launcharc = playerLaunchArc.GetComponent<LineRenderer>();
        launcharcRenderer = playerLaunchArc.GetComponent<Renderer>();

        g = Mathf.Abs(Physics2D.gravity.y);
        arcPoints = new Vector3[resolution + 1];

        launcharc.enabled = false;
        controls = movementScriptRefferance.Controls;

        controls.Gameplay.Aim.performed += ctx => updateAim();
        controls.Gameplay.Aim.started += ctx => { launcharc.enabled = true; launcharc.startColor = new Color(0.71f, 0.97f, 0.02f, 0.0f); launcharc.endColor = new Color(1.0f, 0.01f, 0.01f, 0.0f); };
        controls.Gameplay.Aim.canceled += ctx => {launcharc.enabled = false; AimInput = Vector2.zero; };
        
        controls.Gameplay.Movement.performed += ctx => { if (movementScriptRefferance.Aiming) AimInput = ctx.ReadValue<Vector2>(); };
        controls.Gameplay.Movement.canceled += ctx => { AimInput = Vector2.zero; };

        controls.Gameplay.Action.performed += ctx => { if (slingshotTimer <= 0.0f && movementScriptRefferance.Aiming == true) shoot(); };

        Color newStartColor = new Color(0.71f, 0.97f, 0.02f, 0.0f);
        Color newEndColor = new Color(1.0f, 0.01f, 0.01f, 0.0f);
        launcharc.startColor = newStartColor;
        launcharc.endColor = newEndColor;
    }
    private void Update()
    {
        if (slingshotTimer > 0.0f)
        {
            slingshotTimer -= Time.deltaTime;
        }
        if (launcharc.enabled)
        {
            updateAim();
            if (movementScriptRefferance.teleporting)
            {
                launcharc.enabled = false; AimInput = Vector2.zero;
            }
        }
    }

    private void renderArc()
    {
        launcharc.positionCount = resolution + 1;
        launcharc.SetPositions(calculateLaunchArc());

        launcharcRenderer.material.mainTextureScale =
    new Vector2(Vector2.Distance(launcharc.GetPosition(0), launcharc.GetPosition(launcharc.positionCount - 1)) / launcharc.widthMultiplier,
        1);
        if (slingshotTimer <= 0.25f)
        {
            if (launcharc.startColor.a < 1.0f)
            {
                launcharc.startColor = new Color(0.71f, 0.97f, 0.02f, launcharc.startColor.a + (1.30f * Time.deltaTime));
            }
            else if (launcharc.startColor.a > 1.0f)
            {
                launcharc.startColor = new Color(0.71f, 0.97f, 0.02f, 1.0f);
            }
        }
    }
    private Vector3[] calculateLaunchArc()
    {
        radianAngle = Mathf.Deg2Rad * angle;
        float maxDistance = (velocity * velocity * Mathf.Sin(2 * radianAngle)) / g;
        for (int i = 0; i <= resolution; i++ )
        {
            float t = (float)i / (float)resolution;
            arcPoints[i] = calculateArcPoint(t, maxDistance);
        }
        return arcPoints;
    }
    private Vector3 calculateArcPoint(float t, float incomingMaxDistance)
    {
        float x = t * incomingMaxDistance;
        float y = x * Mathf.Tan(radianAngle) - ((g * (x * x)) / (2 * (velocity * velocity) * (Mathf.Cos(radianAngle) * Mathf.Cos(radianAngle))));
        return new Vector3(x,y);
    }

    private void shoot()
    {
        if (launcharc.enabled)
        {
            Color newStartColor = new Color(183.0f, 248.0f, 6.0f, 0.0f);
            Color newEndColor = new Color(255.0f, 4.0f, 4.0f, 0.0f);
            launcharc.startColor = newStartColor;
            launcharc.endColor = newEndColor;

            Instantiate(BulletPrefab, firepoint.position, firepoint.rotation);
            slingshotTimer = slingshotDelay;
            launcharc.startColor = new Color(0.71f, 0.97f, 0.02f, 0.0f); 
            launcharc.endColor = new Color(1.0f, 0.01f, 0.01f, 0.0f);
        }
    }

    private void updateAim()
    {
        if (movementScriptRefferance.facingRight == true)
        {
            if (firepoint.localRotation.eulerAngles.z < 85f && AimInput.y > 0f)
            {
                firepoint.Rotate(new Vector3(0, 0, AimInput.y * 100f * Time.deltaTime), Space.World);
            }
            else if (firepoint.localRotation.eulerAngles.z > 5f && AimInput.y < 0f)
            {
                firepoint.Rotate(new Vector3(0, 0, AimInput.y * 100f * Time.deltaTime), Space.World);
            }
            else if (firepoint.localRotation.eulerAngles.z < 5f)
            {
                firepoint.rotation = Quaternion.Euler(0, 0, 5.0f);
            }
            else if (firepoint.localRotation.eulerAngles.z > 85f)
            {
                firepoint.rotation = Quaternion.Euler(0, 0, 85f);
            }
        }
        else if (movementScriptRefferance.facingRight == false)
        {
            if (firepoint.localRotation.eulerAngles.z < 85f && AimInput.y > 0f)
            {
                firepoint.Rotate(new Vector3(0, 0, -AimInput.y * 100f * Time.deltaTime), Space.World);
            }
            else if (firepoint.localRotation.eulerAngles.z > 5f && AimInput.y < 0f)
            {
                firepoint.Rotate(new Vector3(0, 0, -AimInput.y * 100f * Time.deltaTime), Space.World);
            }
            else if (firepoint.localRotation.eulerAngles.z < 5f)
            {
                firepoint.localRotation = Quaternion.Euler(0, 0, 5.0f);
            }
            else if (firepoint.rotation.eulerAngles.z > 85f)
            {
                firepoint.localRotation = Quaternion.Euler(0, 0, 85f);
            }
        }
        angle = firepoint.localRotation.eulerAngles.z;

        renderArc();
    }
}
