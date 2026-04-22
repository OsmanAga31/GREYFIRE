// Script fixed by Claude A.I.

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// <summary>
// Manages wave-based enemy spawning, inter-wave announcements, and win detection.
// Waves progress automatically: announce → countdown → spawn enemies → detect clear → repeat.
// </summary>
public class WaveManager : MonoBehaviour
{
    [Header("Wave Settings")]
    [SerializeField] private bool isWaveActive = false;
    [SerializeField] private float timeBetweenWaves = 5f;   // Pause in seconds between waves
    [SerializeField] private float countdown = 5f;           // Pre-wave countdown duration
    [SerializeField] private int waveIndex = 0;              // Current wave index (0-based)
    [SerializeField] private float radius = 10f;             // Spawn radius around this transform
    [SerializeField] private float spawnInterval = 1f;       // Seconds between each enemy spawn
    [SerializeField] private int[] enemiesPerWave;           // How many enemies to spawn per wave

    [Header("Text Settings")]
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private float textDisplayDuration = 3f; // How long text stays fully visible
    [SerializeField] private float textWriteSpeed = 0.25f;   // Seconds per character typed
    private bool doNextMessage = false;                       // Signals WriteText has finished

    [Header("Enemy Settings")]
    [SerializeField] private GameObject[] enemyPrefabs;
    private List<GameObject> enemiesAlive = new List<GameObject>();
    [SerializeField] private Transform player;

    // Tracks how many enemies have been spawned in the current wave
    private int enemiesSpawnedThisWave = 0;

    // True once all enemies for the current wave have been spawned
    private bool waveSpawningComplete = false;

    private Coroutine checkEnemiesAliveRoutine;
    private Coroutine spawnEnemiesRoutine;

    private void OnDisable()
    {
        // Stop coroutines to prevent errors when the object is deactivated
        if (checkEnemiesAliveRoutine != null) StopCoroutine(checkEnemiesAliveRoutine);
        if (spawnEnemiesRoutine != null) StopCoroutine(spawnEnemiesRoutine);
    }

    void Start()
    {
        StartCoroutine(AnnounceGame());
        checkEnemiesAliveRoutine = StartCoroutine(CheckEnemiesAlive());
        spawnEnemiesRoutine = StartCoroutine(SpawnEnemies());
    }

    // <summary>
    // Polls every second to detect when a wave has been fully cleared.
    // Only triggers after all enemies for the wave have been spawned (waveSpawningComplete),
    // preventing a false-positive clear at wave start before any enemies exist.
    // </summary>
    private IEnumerator CheckEnemiesAlive()
    {
        while (true)
        {
            if (isWaveActive && waveSpawningComplete && enemiesAlive.Count == 0)
            {
                isWaveActive = false;
                waveIndex++;

                if (waveIndex < enemiesPerWave.Length)
                {
                    // Announce wave clear and wait before starting the next wave
                    StartCoroutine(WriteText($"Wave {waveIndex} Complete!"));
                    yield return new WaitForSeconds(timeBetweenWaves);
                    StartCoroutine(AnnounceGame());
                }
                else
                {
                    // All waves cleared — player wins
                    StartCoroutine(WriteText("You Win!"));
                    while (!doNextMessage) yield return null; // Wait for the "You Win!" message to finish
                    StopAllCoroutines();
                }
            }
            yield return new WaitForSeconds(1f);
        }
    }

    // <summary>
    // Spawns enemies one at a time at spawnInterval until the wave's enemy quota is reached.
    // Sets waveSpawningComplete so CheckEnemiesAlive knows it's safe to evaluate a clear.
    // </summary>
    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            if (isWaveActive && !waveSpawningComplete)
            {
                if (enemiesSpawnedThisWave < enemiesPerWave[waveIndex])
                {
                    SpawnEnemy();
                    enemiesSpawnedThisWave++;

                    // Mark spawning done once the quota is met
                    if (enemiesSpawnedThisWave >= enemiesPerWave[waveIndex])
                        waveSpawningComplete = true;
                }
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    // Instantiates a random enemy prefab at a random position within the spawn radius
    private void SpawnEnemy()
    {
        Vector3 spawnPosition = transform.position + Random.insideUnitSphere * radius;
        spawnPosition.y = 0; // Keep enemies on the ground plane

        GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        enemiesAlive.Add(enemy);

        // Set Chase Target to follow the player immediately upon spawning
        EnemyMover em = enemy.GetComponent<EnemyMover>();
        if (em != null)
        {
            em.ChaseTarget = player.transform;
        }

        // Subscribe to the enemy's death event to remove it from the alive list
        Target target = enemy.GetComponent<Target>();
        if (target != null)
            target.OnDeath.AddListener(() => DelEnemy(enemy));
    }

    // Removes the enemy from the alive list and destroys its GameObject
    private void DelEnemy(GameObject enemy)
    {
        if (enemiesAlive.Contains(enemy))
            enemiesAlive.Remove(enemy);
        Destroy(enemy);
    }

    // <summary>
    // Plays the wave intro sequence: flavour text → wave number → countdown → "Go!".
    // Resets per-wave spawn tracking so each wave starts fresh.
    // </summary>
    private IEnumerator AnnounceGame()
    {
        // Reset spawn state for the incoming wave
        enemiesSpawnedThisWave = 0;
        waveSpawningComplete = false;

        StartCoroutine(WriteText("Fight the Waves!"));
        while (!doNextMessage) yield return null;

        StartCoroutine(WriteText($"Wave {waveIndex + 1}"));
        while (!doNextMessage) yield return null;

        // Count down using a local variable so the serialized field stays reusable across waves
        float currentCountdown = countdown;
        while (currentCountdown > 0)
        {
            waveText.text = $"{currentCountdown}";
            currentCountdown -= 1;
            yield return new WaitForSeconds(1f);
        }

        StartCoroutine(WriteText("Go!"));
        isWaveActive = true; // Opens spawning and clear-detection
    }

    // <summary>
    // Typewriter effect: types text character by character, holds it, then erases it.
    // Sets doNextMessage = true when done so callers know they can proceed.
    // </summary>
    private IEnumerator WriteText(string text)
    {
        doNextMessage = false;
        waveText.text = "";

        // Type each character with a small delay
        foreach (char c in text)
        {
            waveText.text += c;
            yield return new WaitForSeconds(textWriteSpeed);
        }

        yield return new WaitForSeconds(textDisplayDuration);

        // Erase characters from right to left at double the type speed
        while (waveText.text.Length > 0)
        {
            waveText.text = waveText.text.Substring(0, waveText.text.Length - 1);
            yield return new WaitForSeconds(textWriteSpeed / 2);
        }

        doNextMessage = true;
    }
}