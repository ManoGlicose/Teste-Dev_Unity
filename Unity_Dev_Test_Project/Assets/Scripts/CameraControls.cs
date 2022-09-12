using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraControls : MonoBehaviour
{
    [Header("Components")]
    Controls controls;
    public Transform centerDot;
    CinemachineVirtualCamera cam;

    [Header("Targets")]
    public List<Transform> targets = new List<Transform>();
    public Transform enemy;
    public LayerMask enemyLayer;

    [Header("Detect Targets")]
    [HideInInspector] public Vector3 mousePosition;

    Vector3 GetCenterPoint()
    {
        if (targets.Count <= 0)
        {
            return targets[0].position;
        }

        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }

        return bounds.center;
    }

    private void Awake()
    {
        controls = new Controls();

        //controls.Combat.Aim.performed += ctx => mousePosition = ctx.ReadValue<Vector2>();
    }

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Cinemachine.CinemachineVirtualCamera>();

        targets.Add(FindObjectOfType<PlayerController>().transform);
    }

    // Update is called once per frame
    void Update()
    {
        FindEnemies();

        centerDot.position = mousePosition;

        if(enemy != null)
        {
            if (!targets.Contains(enemy)) targets.Add(enemy);
        }
        else
        {
            if(targets.Count > 1)
            targets.Remove(targets[1]);
        }
    }

    private void LateUpdate()
    {
        if (targets.Count <= 0)
        {
            Debug.LogError("No targets detected. Assign them to the 'targets' list ");
            return;
        }

        Move();
    }

    void FindEnemies()
    {
        Vector2 rayPos = new Vector2(Camera.main.ScreenToWorldPoint(mousePosition).x, Camera.main.ScreenToWorldPoint(mousePosition).y);
        //RaycastHit2D hit = Physics2D.Raycast(rayPos, Vector2.zero, 0f, enemyLayer);

        Collider2D[] enemies = Physics2D.OverlapCircleAll(rayPos, 1, enemyLayer);

        if (enemies.Length > 0)
        {
            enemy = enemies[0].transform;
        }
        else
            enemy = null;
    }

    void Move()
    {
        Vector3 centerPoint = GetCenterPoint();
        //Vector3 newPosition = centerPoint + new Vector3(0, 1, 0);

        //centerDot.position = Vector3.SmoothDamp(centerDot.position, newPosition, ref velocity, smoothTime);
        centerDot.position = centerPoint;

        cam.Follow = centerDot;
    }
}
