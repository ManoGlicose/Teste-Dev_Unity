using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Components")] // Componentes necess�rios para o funcionamento do script
    Controls controls; // Novo Input System
    PlayerController player; // Script do controle do jogador
    CameraControls cameraControls; // Script do controle da c�mera

    [Space(15)]
    public Transform cane;
    public Transform nozzle;
    public Transform aimTransform;
    Vector3 mousePosition;

    [Header("Weapon")] // Par�metros para a arma/bengala
    public float rateOfFire = 300;
    float fireRate;
    float fireTimer;
    bool isShooting = false;
    public Animation caneAnim;

    [Header("Projectile")] // Proj�til 
    public GameObject projectilePrefab;

    [Header("Audio")] // Arquivos de audio
    public AudioSource source;
    public AudioClip fireClip;

    private void Awake()
    {
        controls = new Controls(); // Cria o Input System

        controls.Combat.Aim.performed += ctx => mousePosition = ctx.ReadValue<Vector2>(); // Coloca o valor da posi��o do mouse na vari�vel

        controls.Combat.Shoot.performed += _ => isShooting = true; // Liga o disparo
        controls.Combat.Shoot.canceled += _ => isShooting = false; // Desliga o disparo
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<PlayerController>();
        cameraControls = FindObjectOfType<CameraControls>();

        // Calcula de in�cio o intervalo entre um disparo e outro para o c�lculo 
        fireRate = 1 / (rateOfFire / 60);
        fireTimer = fireRate;
    }

    // Update is called once per frame
    void Update()
    {
        Aim();

        if (isShooting) Shoot(); // Atira se o jogador mantiver o bot�o do mouse apertado

        // Calcula constantemente o intervalo entre um disparo e outro para o c�lculo 
        fireRate = 1 / (rateOfFire / 60);

        // Aumenta o contador do intervalo de disparo enquanto ele for menor que o intervalo
        if (fireTimer < fireRate) fireTimer += Time.deltaTime;

        // Define a posi��o do mouse para o script da c�mera
        cameraControls.mousePosition = mousePosition;
    }

    void Aim() // Calcula a dire��o em que o jogador vai atirar e rotaciona a bengala para apontar pro local correto
    {
        var dir = mousePosition - Camera.main.WorldToScreenPoint(transform.position);
        var angle = Mathf.Atan2(player.isFacingRight ? dir.y : -dir.y, player.isFacingRight ? dir.x : -dir.x) * Mathf.Rad2Deg;

        var aimAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        
        cane.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        aimTransform.transform.rotation = Quaternion.AngleAxis(aimAngle, Vector3.forward);
    }

    void Shoot() // Dispara o proj�til
    {
        if (fireTimer < fireRate) return;

        GameObject projectile = Instantiate(projectilePrefab, nozzle.position, aimTransform.rotation);
        projectile.GetComponent<ProjectileController>().isFromPlayer = true;

        caneAnim.Play("fire");
        source.PlayOneShot(fireClip, .5f);

        fireTimer = 0;
    }

    
    // Fun��es necess�rias para iniciar o novo Input System
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
