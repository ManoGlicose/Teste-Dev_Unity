using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WavesController : MonoBehaviour
{
    public enum State
    {
        Running,
        Waiting
    }

    public State state;

    [Header("Enemies")]
    public List<Transform> allEnemies = new List<Transform>();
    public List<Transform> spawnPoints = new List<Transform>();

    [Header("Wave Info")]
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

        if (canStart)
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

            if (enemiesRemaining <= 0)
            {
                if (!endOfWave)
                    StartCoroutine(CallNextWave());
            }
        }
    }

    void SpawnEnemy()
    {
        print("Enemy spawned");
        GameObject currentEnemy = Instantiate(allEnemies[0].gameObject, spawnPoints[Random.Range(0, spawnPoints.Count)].position, Quaternion.identity, transform);

        //currentEnemy.transform.position = new Vector3(Random.Range(spawnPoints[0].position.x - 15, spawnPoints[0].position.x + 15),
        //    Random.Range(spawnPoints[0].position.y - 15, spawnPoints[0].position.y + 15),
        //    Random.Range(spawnPoints[0].position.z - 15, spawnPoints[0].position.z + 15));

        //currentEnemy.GetComponent<EnemyAIController>().accuracy = enemyAccuracy;
        //currentEnemy.GetComponent<EnemyAIController>().shield = enemyShield;

        enemiesSpawned++;
        enemies++;
    }

    void StartWave()
    {
        waveIndex = 1;

        enemySpawnAmount = 2;
        enemiesSpawned = 0;
        enemies = 0;
        enemiesRemaining = enemySpawnAmount;
    }

    void NextWave()
    {
        waveIndex++;

        enemySpawnAmount += 2;
        enemiesSpawned = 0;
        enemies = 0;
        enemiesRemaining = enemySpawnAmount;
        state = State.Running;

        //enemyAccuracy += .1f;
        //if (waveIndex % 2 != 0)
        //    enemyShield++;

        endOfWave = false;
    }

    IEnumerator CallNextWave()
    {
        endOfWave = true;

        StartCoroutine(TypeText("Onda limpa!"));
        //player.AddScore(10, "WAVE CREARED", true);

        state = State.Waiting;
        //if (playerScore.timer > 0)
        //    playerScore.timer = 3;

        yield return new WaitForSeconds(6);

        StopCoroutine(TypeText(""));
        StartCoroutine(TypeText("Nova onda chegou..."));

        NextWave();
        HighscoreWave();

        yield return new WaitForSeconds(3);

        StopCoroutine(TypeText(""));
        announcementText.text = "";
    }

    void HighscoreWave()
    {
        int highWave = PlayerPrefs.GetInt("MaxWave");

        if (waveIndex > highWave)
        {
            highWave = waveIndex;
            PlayerPrefs.SetInt("MaxWave", highWave);
        }
    }

    public void EnemyKilled()
    {
        enemies--;
        enemiesRemaining--;

        int allPlants = PlayerPrefs.GetInt("AllPlants");
        allPlants++;
        PlayerPrefs.SetInt("AllPlants", allPlants);
    }

    IEnumerator TypeText(string sentence)
    {
        announcementText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            announcementText.text += letter;
            yield return new WaitForSeconds(0.034f);
        }
    }
}
