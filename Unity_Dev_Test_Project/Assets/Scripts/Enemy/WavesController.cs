using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WavesController : MonoBehaviour
{
    // Enum que dita qual o estado da onda
    public enum State
    {
        Running,
        Waiting
    }

    public State state;

    [Header("Enemies")] // Lista de inimigos e pontos de spawn
    public List<Transform> allEnemies = new List<Transform>();
    public List<Transform> spawnPoints = new List<Transform>();

    [Header("Wave Info")] // Informaçóes da onda
    public bool started = false;
    bool canStart = true;

    [HideInInspector]
    public int waveIndex = 0;
    int enemySpawnAmount = 0;
    int enemiesSpawned = 0;
    int enemies = 0;
    int enemiesRemaining = 0;

    int maxOnScreen = 12;

    bool endOfWave = false;

    [Header("UI")]
    public Text waveText;
    public Text enemiesText;
    public Text announcementText;

    // Start is called before the first frame update
    void Start()
    {
        started = false;

        announcementText.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        #region UI
        waveText.text = waveIndex.ToString() + " : ONDA";
        enemiesText.text = enemiesRemaining.ToString() + " : INIMIGOS";
        #endregion

        if (canStart) // Se pode começar, então começa
        {
            if (!started)
            {
                StartWave();
                started = true;
            }
        }

        if (started)
        {
            // Gerar inimigos
            if (state == State.Running)
            {
                if (enemiesSpawned < enemySpawnAmount)
                {
                    if (enemies < maxOnScreen)
                    {
                        SpawnEnemy();
                    }
                }
            }

            // Quando todos os inimigos da onda são derrotados
            if (enemiesRemaining <= 0)
            {
                if (!endOfWave)
                    StartCoroutine(CallNextWave());
            }
        }
    }

    void SpawnEnemy() // Spawna o inimigo
    {
        print("Enemy spawned");
        GameObject currentEnemy = Instantiate(allEnemies[Random.Range(0, allEnemies.Count)].gameObject, spawnPoints[Random.Range(0, spawnPoints.Count)].position, Quaternion.identity, transform);

        enemiesSpawned++;
        enemies++;
    }

    void StartWave() // Começa a primeira onda
    {
        waveIndex = 1;

        enemySpawnAmount = 2;
        enemiesSpawned = 0;
        enemies = 0;
        enemiesRemaining = enemySpawnAmount;
    }

    void NextWave() // Cria a próxima onda
    {
        waveIndex++;

        enemySpawnAmount += 2;
        enemiesSpawned = 0;
        enemies = 0;
        enemiesRemaining = enemySpawnAmount;
        state = State.Running;

        endOfWave = false;
    }

    IEnumerator CallNextWave() // Contador para a onda seguinte
    {
        endOfWave = true;

        StartCoroutine(TypeText("Onda limpa!"));

        state = State.Waiting;

        yield return new WaitForSeconds(6);

        StopCoroutine(TypeText(""));
        StartCoroutine(TypeText("Nova onda chegou..."));

        NextWave();
        HighscoreWave();

        yield return new WaitForSeconds(3);

        StopCoroutine(TypeText(""));
        announcementText.text = "";
    }

    void HighscoreWave() // Guarda a informação da onda mais distante que o jogador conseguiu
    {
        int highWave = PlayerPrefs.GetInt("MaxWave");

        if (waveIndex > highWave)
        {
            highWave = waveIndex;
            PlayerPrefs.SetInt("MaxWave", highWave);
        }
    }

    public void EnemyKilled() // Remove o inimigo das contagens
    {
        enemies--;
        enemiesRemaining--;

        int allPlants = PlayerPrefs.GetInt("AllPlants");
        allPlants++;
        PlayerPrefs.SetInt("AllPlants", allPlants);
    }

    IEnumerator TypeText(string sentence) // Digita o texto dado letra por letra
    {
        announcementText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            announcementText.text += letter;
            yield return new WaitForSeconds(0.034f);
        }
    }
}
