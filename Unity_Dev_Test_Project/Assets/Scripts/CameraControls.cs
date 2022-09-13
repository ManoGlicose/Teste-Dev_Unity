using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraControls : MonoBehaviour
{
    [Header("Components")] // Componentes necess�rios para o funcionamento do script
    public Transform centerDot; // O ponto central entre dois objetos, o player e o inimigo alvo
    CinemachineVirtualCamera cam; // Componente do Cinemachine que controla a c�mera

    [Header("Targets")]
    public List<Transform> targets = new List<Transform>(); // Lista de alvos que a c�mera segue (o jogador � sempre o primeiro)
    public Transform enemy; // O inimigo alvo
    public LayerMask enemyLayer; // A camada em que o inimigo est�

    [Header("Detect Targets")]
    [HideInInspector] public Vector3 mousePosition; // A posi��o do mouse dada pelo novo Input System

    Vector3 GetCenterPoint() // Fun��o que me retorna o ponto central entre dois objetos encapsulados
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

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Cinemachine.CinemachineVirtualCamera>(); 

        targets.Add(FindObjectOfType<PlayerController>().transform); // Adiciona o player para a lista de alvos como primeiro
    }

    // Update is called once per frame
    void Update()
    {
        FindEnemies();

        if(enemy != null)
        {
            if (!targets.Contains(enemy)) targets.Add(enemy); // Adiciona o inimigo alvo � lista
        }
        else
        {
            if(targets.Count > 1)
            targets.Remove(targets[1]); // Remove da lista o inimigo alvo que est� fora do alcance do mouse
        }
    }

    private void LateUpdate()
    {
        if (targets.Count <= 0)
        {
            Debug.LogError("No targets detected. Assign them to the 'targets' list "); // Me d� um erro caso n�o tenha nenhum alco na lista
            return;
        }

        Move(); 
    }

    void FindEnemies() // Fun��o que encontra o inimigo mais pr�ximo do mouse
    {
        Vector2 rayPos = new Vector2(Camera.main.ScreenToWorldPoint(mousePosition).x, Camera.main.ScreenToWorldPoint(mousePosition).y);
        // Define a posi��o da origem do mouse em rela��o ao mundo, n�o � c�mera

        Collider2D[] enemies = Physics2D.OverlapCircleAll(rayPos, 1, enemyLayer); // Cria um c�rculo que detecta colisores

        if (enemies.Length > 0) // Se tiver colisores, ent�o define o inimigo alvo. Se n�o, o inimigo � nulo
        {
            enemy = enemies[0].transform;
        }
        else
            enemy = null;
    }

    void Move() // Move o ponto central para a localiza��o entre o player e o inimigo alvo
    {
        Vector3 centerPoint = GetCenterPoint(); // Guarda em uma vari�vel local o valor dado pela fun��o

        centerDot.position = centerPoint; // Define a posi��o do ponto central de acordo com a vari�vel acima

        cam.Follow = centerDot; // Define o alvo do Cinemachine 
    }
}
