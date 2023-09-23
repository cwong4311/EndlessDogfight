using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveController : MonoBehaviour
{
    public EnemyBoidsController EnemySpawner;
    public WaveStats[] WaveStats;
    public GameObject healPF;

    private float _waveTimer;
    private int _waveLevel;
    private WaveStats _currentWave;
    private float _currentWaveDuration;

    private int _extraLevel = 0;
    private int _enemiesActive = 0;

    public static Action<Vector3> onEnemyDeathPowerup;

    private void Start()
    {
        _waveLevel = 0;
        StartWave();
    }

    private void Update()
    {
        if (Time.time - _waveTimer > _currentWaveDuration)
        {
            StartWave();
        }

        if (_waveLevel > 0 && _enemiesActive <= 0)
        {
            StartWave();
        }
    }

    private void OnEnable()
    {
        onEnemyDeathPowerup += OnEnemyDeathPowerup;
    }

    private void OnDisable()
    {
        onEnemyDeathPowerup -= OnEnemyDeathPowerup;
    }

    private void StartWave()
    {
        _waveTimer = Time.time;
        _waveLevel++;

        if (_waveLevel <= WaveStats.Length)
        {
            _currentWave = WaveStats[_waveLevel - 1];
        }
        else
        {
            _currentWave = WaveStats[WaveStats.Length - 1];
            _extraLevel++;
        }
        float extraWaveMultiplier = _extraLevel / 10;
        var waveModifier = (extraWaveMultiplier > 1) ? extraWaveMultiplier : 1;

        _currentWaveDuration = _currentWave.WaveDuration * waveModifier;

        List<BoidsAgent> enemiesThisWave = new List<BoidsAgent>();
        var pointsToUse = (int)(_currentWave.totalWavePoints * waveModifier);
        var enemiesList = _currentWave.usableEnemies;
        for (int i = 0; i < enemiesList.Length; i++)
        {
            int spawnCount = 0;
            int enemyMaxCount = (int)(enemiesList[i].maxCount * waveModifier);

            // If last enemy on list, maximise spawn count
            if (i == enemiesList.Length - 1)
            {
                spawnCount = pointsToUse / enemiesList[i].points;
            }
            // Otherwise, randomise a value based on max spawnable and remaining points
            else
            {
                var maxSpawnable = Mathf.Min(enemyMaxCount / 2, pointsToUse / enemiesList[i].points);
                spawnCount = UnityEngine.Random.Range(1, maxSpawnable + 1); // Add one to make maxSpawnable inclusive
            }
            
            pointsToUse -= enemiesList[i].points * spawnCount;
            for (int n = 0; n < spawnCount; n++)
            {
                enemiesThisWave.Add(enemiesList[i].enemyType);
            }

            _enemiesActive += spawnCount;
        }

        EnemySpawner.SpawnEnemies(enemiesThisWave, extraWaveMultiplier);
    }

    /// <summary>
    /// Delegate for whenever an enemy dies. Chance to spawn powerup or heal
    /// </summary>
    /// <param name="deathLocation"></param>
    private void OnEnemyDeathPowerup(Vector3 deathLocation)
    {
        _enemiesActive--;

        if (UnityEngine.Random.Range(0, 100) < 5)
        {
            var listOfPowerups = _currentWave.droppablePowerups;
            var powerIdx = UnityEngine.Random.Range(0, listOfPowerups.Length);

            ObjectPoolManager.Spawn(listOfPowerups[powerIdx].gameObject, deathLocation, Quaternion.identity);
        }
        else if (UnityEngine.Random.Range(0, 100) < 10)
        {
            ObjectPoolManager.Spawn(healPF, deathLocation, Quaternion.identity);
        }
    }
}
