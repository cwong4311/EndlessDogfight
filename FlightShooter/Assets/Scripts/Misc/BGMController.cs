using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMController : MonoBehaviour
{
    public static Action OnBossSpawn;
    public static Action OnBossDefeat;

    public AudioClip BattleBGM;
    public AudioClip BossBGM;

    private AudioSource _audioSource;

    public void Start()
    {
        if (!TryGetComponent<AudioSource>(out _audioSource))
        {
            throw new MissingComponentException("BGM Controller does not have an AudioSource");
        }

        StartBaseBGM();

        OnBossSpawn += StartBossBGM;
        OnBossDefeat += StartBaseBGM;
    }

    private void OnDestroy()
    {
        OnBossSpawn -= StartBossBGM;
        OnBossDefeat -= StartBaseBGM;
    }

    private void StartBaseBGM()
    {
        _audioSource.Stop();
        _audioSource.clip = BattleBGM;
        _audioSource.Play();
    }

    private void StartBossBGM()
    {
        _audioSource.Stop();
        _audioSource.clip = BossBGM;
        _audioSource.Play();
    }
}
