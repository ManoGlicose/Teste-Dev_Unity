using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Components")]
    Controls controls;
    PlayerController player;

    [Space(15)]
    public Transform cane;
    public Transform nozzle;
    public Transform aimTransform;
    Vector3 mousePosition;

    [Header("Weapon")]
    public float rateOfFire = 300;
    float fireRate;
    float fireTimer;
    bool isShooting = false;
    public Animation caneAnim;

    [Header("Projectile")]
    public GameObject projectilePrefab;

    private void Awake()
    {
        controls = new Controls();

        controls.Combat.Aim.performed += ctx => mousePosition = ctx.ReadValue<Vector2>();

        controls.Combat.Shoot.performed += _ => isShooting = true;
        controls.Combat.Shoot.canceled += _ => isShooting = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<PlayerController>();

        fireRate = 1 / (rateOfFire / 60);
        fireTimer = fireRate;
    }

    // Update is called once per frame
    void Update()
    {
        Aim();

        if (isShooting) Shoot();

        fireRate = 1 / (rateOfFire / 60);

        if (fireTimer < fireRate) fireTimer += Time.deltaTime;
    }

    void Aim()
    {
        var dir = mousePosition - Camera.main.WorldToScreenPoint(transform.position);
        var angle = Mathf.Atan2(player.isFacingRight ? dir.y : -dir.y, player.isFacingRight ? dir.x : -dir.x) * Mathf.Rad2Deg;

        var aimAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        
        cane.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        aimTransform.transform.rotation = Quaternion.AngleAxis(aimAngle, Vector3.forward);
    }

    void Shoot()
    {
        if (fireTimer < fireRate) return;

        GameObject projectile = Instantiate(projectilePrefab, nozzle.position, aimTransform.rotation);

        caneAnim.Play("fire");

        fireTimer = 0;
    }

    #region Enable/Disable Controls
    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
    #endregion
}
