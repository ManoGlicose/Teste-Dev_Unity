using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraControls : MonoBehaviour
{
    [Header("Components")] // Componentes necessários para o funcionamento do script
    public Transform centerDot; // O ponto central entre dois objetos, o player e o inimigo alvo
    CinemachineVirtualCamera cam; // Componente do Cinemachine que controla a câmera

    [Header("Targets")]
    public List<Transform> targets = new List<Transform>(); // Lista de alvos que a câmera segue (o jogador é sempre o primeiro)
    public Transform enemy; // O inimigo alvo
    public LayerMask enemyLayer; // A camada em que o inimigo está

    [Header("Detect Targets")]
    [HideInInspector] public Vector3 mousePosition; // A posição do mouse dada pelo novo Input System

    Vector3 GetCenterPoint() // Função que me retorna o ponto central entre dois objetos encapsulados
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
            if (!targets.Contains(enemy)) targets.Add(enemy); // Adiciona o inimigo alvo à lista
        }
        else
        {
            if(targets.Count > 1)
            targets.Remove(targets[1]); // Remove da lista o inimigo alvo que está fora do alcance do mouse
        }
    }

    private void LateUpdate()
    {
        if (targets.Count <= 0)
        {
            Debug.LogError("No targets detected. Assign them to the 'targets' list "); // Me dá um erro caso não tenha nenhum alco na lista
            return;
        }

        Move(); 
    }

    void FindEnemies() // Função que encontra o inimigo mais próximo do mouse
    {
        Vector2 rayPos = new Vector2(Camera.main.ScreenToWorldPoint(mousePosition).x, Camera.main.ScreenToWorldPoint(mousePosition).y);
        // Define a posição da origem do mouse em relação ao mundo, não à câmera

        Collider2D[] enemies = Physics2D.OverlapCircleAll(rayPos, 1, enemyLayer); // Cria um círculo que detecta colisores

        if (enemies.Length > 0) // Se tiver colisores, então define o inimigo alvo. Se não, o inimigo é nulo
        {
            enemy = enemies[0].transform;
        }
        else
            enemy = null;
    }

    void Move() // Move o ponto central para a localização entre o player e o inimigo alvo
    {
        Vector3 centerPoint = GetCenterPoint(); // Guarda em uma variável local o valor dado pela função

        centerDot.position = centerPoint; // Define a posição do ponto central de acordo com a variável acima

        cam.Follow = centerDot; // Define o alvo do Cinemachine 
    }
}
