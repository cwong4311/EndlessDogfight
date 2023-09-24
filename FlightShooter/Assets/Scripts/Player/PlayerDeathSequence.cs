using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathSequence : MonoBehaviour
{
    public GameObject NormalCam;
    public GameObject DeathCam;

    public GameObject NormalUI;
    public GameObject DeathUI;

    public GameObject PlayerPlane;
    public GameObject PlayerModel;
    public GameObject ExplosionPF;

    public TMPro.TextMeshProUGUI LevelTracker;
    public TMPro.TextMeshProUGUI DeathUIWaveCount;
    public TMPro.TextMeshProUGUI DeathUITimePlayed;

    private PlayerController _playerController;
    private Coroutine _deathCoroutine;
    private float _startTime;

    public void Start()
    {
        _playerController = PlayerPlane.GetComponent<PlayerController>();
        _startTime = Time.time;
    }

    public void OnPlayerDeath()
    {
        if (_deathCoroutine == null)
        {
            NormalCam.SetActive(false);
            DeathCam.SetActive(true);

            _deathCoroutine = StartCoroutine(DeathSequence());
        }
    }

    private IEnumerator DeathSequence()
    {
        var playerRB = PlayerPlane.GetComponent<Rigidbody>();
        playerRB.velocity = Vector3.zero;

        _playerController.MaxThrust = 0f;
        _playerController.LiftStrength = 0f;
        playerRB.mass = 2000f;
        playerRB.AddTorque(transform.right * -2f, ForceMode.VelocityChange);

        _playerController.CanShoot = false;

        yield return new WaitForSecondsRealtime(0.8f);

        NormalCam.SetActive(false);
        DeathCam.SetActive(true);
        NormalUI.SetActive(false);

        Time.timeScale = 0.4f;

        yield return new WaitForSecondsRealtime(3f);

        Instantiate(ExplosionPF, PlayerPlane.transform.position, Quaternion.identity);
        playerRB.isKinematic = true;

        yield return new WaitForSecondsRealtime(0.3f);
        PlayerModel.SetActive(false);

        yield return new WaitForSecondsRealtime(6f);

        Time.timeScale = 1f;

        var secondsPlayed = Time.time - _startTime;
        DeathUI.SetActive(true);
        DeathUIWaveCount.text = (int.Parse(LevelTracker.text) - 1).ToString();
        DeathUITimePlayed.text = $"{(int)(secondsPlayed / 60)}minutes, {(int)(secondsPlayed % 60)}seconds";
    }
}
