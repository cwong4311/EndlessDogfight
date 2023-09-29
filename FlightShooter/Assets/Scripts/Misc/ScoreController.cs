using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;

public class ScoreController : MonoBehaviour
{
    public TextMeshProUGUI ScoreCounter;
    public TextMeshProUGUI ScoreAddonCounter;
    public TextMeshProUGUI EndScreenScoreCounter;

    public Animator ScoreCounterAnimator;

    public static Action<int> onEnemyDeathScore;
    public static Action onPlayerDeath;

    private int _currentScore;
    private Queue<int> _scoresToBeAdded = new Queue<int>();

    private bool _isGameRunning;

    private void Start()
    {
        _currentScore = 0;
        ScoreCounter.text = _currentScore.ToString();

        onEnemyDeathScore += ProcessScore;
        onPlayerDeath += ProcessGameEnd;

        _isGameRunning = true;
    }

    private void Update()
    {
        if (_isGameRunning == false)
        {
            return;
        }

        if (_scoresToBeAdded.Count > 0 && ScoreCounterAnimator.GetCurrentAnimatorStateInfo(0).IsTag("idle"))
        {
            var score = _scoresToBeAdded.Dequeue();
            ScoreAddonCounter.text = score.ToString();

            AddToScore(score);
            ScoreCounterAnimator.SetTrigger("AddScore");
        }
    }

    private void AddToScore(int score)
    {
        _currentScore += score;
        ScoreCounter.text = _currentScore.ToString();
    }

    private void ProcessScore(int scoreIncrease)
    {
        _scoresToBeAdded.Enqueue(scoreIncrease);
    }

    private void ProcessGameEnd()
    {
        _isGameRunning = false;

        int remainingScoreToAdd = _scoresToBeAdded.Sum();
        _currentScore += remainingScoreToAdd;

        ScoreCounter.text = _currentScore.ToString();
        EndScreenScoreCounter.text = _currentScore.ToString();

        _scoresToBeAdded.Clear();
        // Write To SaveData
    }
}
