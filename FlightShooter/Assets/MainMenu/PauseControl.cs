using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseControl : MonoBehaviour
{
    public GameObject PauseMenuUI;
    private PlayerHealth _player;
    private bool _hasQuit;

    public void Start()
    {
        TogglePauseMenu(false);
        _player = GameObject.FindFirstObjectByType<PlayerHealth>();
        _hasQuit = false;
    }

    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    }

    public void Pause()
    {
        TogglePauseMenu(true);

        Time.timeScale = 0;
        AudioListener.pause = true;
    }

    public void ResumeGame()
    {
        TogglePauseMenu(false);

        Time.timeScale = 1;
        AudioListener.pause = false;
    }

    public void GiveUp()
    {
        if (_hasQuit == false)
        {
            _player.TakeDamage(_player.MaxHealth);
            _hasQuit = true;
            ResumeGame();
        }
    }

    private void TogglePauseMenu(bool isEnabled)
    {
        if (PauseMenuUI != null)
        {
            PauseMenuUI.SetActive(isEnabled);
        }
    }
}
