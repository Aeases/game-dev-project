using System;
using System.Collections;
using System.Threading;
using UnityEngine;

public class WaveController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
     
    [SerializeField] public Wave[] waves;
    [SerializeField] public GameObject[] spawnpoints;
    [SerializeField] public GameObject gameWonUI;
    [SerializeField] public GameObject fifteenSecondsUI;
    [SerializeField] public GameObject waveCompleteUI;
    public static bool gameWon {get; private set; } = false;
    [HideInInspector] private int currentWave = 0;
    private float countdown = 0;

    AudioManager audioManager;
    private bool readyToCountdown = true;
    public static WaveController Instance;

    // this is not a reference, can be used for remaining enemeies or something
    [HideInInspector] public int remainingEnemies = 0;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    void Start()
    {
        audioManager.StopMusic();
        audioManager.PlayMusic(audioManager.background);
        gameWon = false;
        for (int i = 0; i < waves.Length; i++)
        {
            waves[i].remainingEnemies = waves[i].enemies.Length;
        }



        
    }

    private IEnumerator SpawnWave()
    {
        audioManager.StopMusic();
        audioManager.PlayMusic(audioManager.fight);
        if (currentWave > waves.Length) yield break;



        for (int i = 0; i < waves[currentWave].enemies.Length; i++)
        {
            WaveEnemy currentWaveEnemy = waves[currentWave].enemies[i];
            var spawnedEnemy = Instantiate(currentWaveEnemy.enemy, currentWaveEnemy.spawnpoint.transform.position, currentWaveEnemy.spawnpoint.transform.rotation);
            spawnedEnemy.transform.SetParent(currentWaveEnemy.spawnpoint.transform);
            spawnedEnemy.currentElement = currentWaveEnemy.enemyElement;
            spawnedEnemy.enemyType = currentWaveEnemy.enemyType;
            yield return new WaitForSeconds(waves[currentWave].timeToNextEnemy);
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (currentWave >= waves.Length)
        {
            if (gameWon == false)
            {
                gameWon = true;
            }
            else
            {
                return;
            }
        }

        if (readyToCountdown == true)
        {
            countdown -= Time.deltaTime;
        }

        if (countdown <= 0)
        {
            readyToCountdown = false;
            fifteenSecondsUI.SetActive(false);

            countdown = waves[currentWave].timeToNextWave;
            StartCoroutine(SpawnWave());
        }
        if (waves[currentWave].remainingEnemies == 0)
        {
            audioManager.StopMusic();
            audioManager.PlayMusic(audioManager.background);
            readyToCountdown = true;
            StartCoroutine(waveComplete());
            fifteenSecondsUI.SetActive(true);
            currentWave += 1;
        }


        
    }

    public void EnemyKilled()
    {
        waves[currentWave].remainingEnemies -= 1;
    }

    private IEnumerator waveComplete()
    {
        waveCompleteUI.SetActive(true);
        yield return new WaitForSeconds(3f);
        waveCompleteUI.SetActive(false);
    }


}

[System.Serializable]
public class Wave
{
    public WaveEnemy[] enemies;
    public float timeToNextWave = 22;
    [HideInInspector] public int remainingEnemies;
    public float timeToNextEnemy = 1;
}

[System.Serializable]
public class WaveEnemy
{
    public Enemy enemy;
    public elementType enemyElement;
    public Enemy.EnemyType enemyType;
    public GameObject spawnpoint;

}
