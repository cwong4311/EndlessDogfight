using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct WaveEnemyData
{
    public BoidsAgent enemyType;
    public int maxCount;
    public int points;
}

[CreateAssetMenu(menuName = "Wave/LevelStats")]
public class WaveStats : ScriptableObject
{
    public WaveEnemyData[] usableEnemies;
    public PowerupToken[] droppablePowerups;
    public int totalWavePoints;
    public float WaveDuration;
}
