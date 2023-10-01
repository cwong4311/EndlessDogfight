using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaveController : MonoBehaviour
{
    public EnemyBoidsController EnemySpawner;
    public WaveStats[] WaveStats;
    public GameObject healPF;

    public TextMeshProUGUI UIWaveCounter;
    public TextMeshProUGUI UIWaveTimer;
    public TextMeshProUGUI EnemiesLeftCounter;

    [Range(1, 100)]
    public int PowerupDropChance = 5;

    [Range(1, 100)]
    public int HealDropChance = 10;

    private float _waveTimer;
    private int _waveLevel;
    private WaveStats _currentWave;
    private float _currentWaveDuration;

    private int _extraLevel = 0;
    private int _enemiesActive = 0;

    public static Action<Vector3> onEnemyDeathPowerup;

    private WaveStats _lastMobWave;
    private WaveStats _lastBossWave;

    private int _lastTime;
    
    public AudioClip TickDownSFX;
    [SerializeField]
    private AudioSource _audioSource;

    private void Start()
    {
        _waveLevel = 0;
        _lastMobWave = WaveStats[WaveStats.Length - 2];
        _lastBossWave = WaveStats[WaveStats.Length - 1];

        StartWave();
    }

    private void Update()
    {
        var timeLeft = (int)(_currentWaveDuration - (Time.time - _waveTimer));
        if (timeLeft < _lastTime)
        {
            _lastTime = timeLeft;
            if (_lastTime <= 10)
            {
                UIWaveTimer.color = Color.red;
                _audioSource.PlayOneShot(TickDownSFX);
            }
        }

        UIWaveTimer.text = timeLeft.ToString();
        

        if (Time.time - _waveTimer > _currentWaveDuration)
        {
            StartWave();
        }

        if (_waveLevel > 0 && _enemiesActive <= 0)
        {
            // Add extra pts for finishing early
            var remainingTime = (int)(_currentWaveDuration - (Time.time - _waveTimer)) * 3;
            ScoreController.onEnemyDeathScore.Invoke(remainingTime);

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

        UIWaveTimer.color = Color.white;
        UIWaveCounter.text = _waveLevel.ToString();

        if (_waveLevel <= WaveStats.Length)
        {
            _currentWave = WaveStats[_waveLevel - 1];
        }
        else
        {
            _currentWave = ((_waveLevel - 1) % 10 == 0) ? _lastBossWave: _lastMobWave;
            _extraLevel++;
        }
        float extraWaveMultiplier = _extraLevel / 10;
        var waveModifier = (extraWaveMultiplier > 1) ? extraWaveMultiplier : 1;

        _currentWaveDuration = _currentWave.WaveDuration * waveModifier;
        _lastTime = (int)_currentWaveDuration;

        List<BoidsAgent> enemiesThisWave = new List<BoidsAgent>();
        var pointsToUse = (int)(_currentWave.totalWavePoints);
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
            EnemiesLeftCounter.text = _enemiesActive.ToString();
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
        EnemiesLeftCounter.text = _enemiesActive.ToString();

        if (UnityEngine.Random.Range(0, 100) < PowerupDropChance)
        {
            var listOfPowerups = _currentWave.droppablePowerups;
            var powerIdx = UnityEngine.Random.Range(0, listOfPowerups.Length);

            ObjectPoolManager.Spawn(listOfPowerups[powerIdx].gameObject, deathLocation, Quaternion.identity);
        }
        else if (UnityEngine.Random.Range(0, 100) < HealDropChance)
        {
            ObjectPoolManager.Spawn(healPF, deathLocation, Quaternion.identity);
        }
    }
}
