using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public string BattleSceneName;
    public GameObject LoadingScreen;
    public Slider LoadingBar;
    public GameObject CreditsScreen;

    private Coroutine _levelLoadCoroutine;
    private bool _inCreditsMenu;

    public void Awake()
    {
        LoadingScreen?.SetActive(false);
        CreditsScreen?.SetActive(false);
    }

    public void StartGame()
    {
        if (_levelLoadCoroutine == null && !_inCreditsMenu)
        {
            _levelLoadCoroutine = StartCoroutine(LoadLevelAsync());
        }
    }

    public void QuitGame()
    {
        if (_levelLoadCoroutine == null && !_inCreditsMenu)
        {
            Application.Quit();
        }
    }

    private IEnumerator LoadLevelAsync()
    {
        LoadingScreen.SetActive(true);

        LoadingBar.value = 0f;
        yield return new WaitForSeconds(2f);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(BattleSceneName);
        while (!asyncLoad.isDone)
        {
            LoadingBar.value = asyncLoad.progress;
            yield return null;
        }

        yield return new WaitForSeconds(3f);
    }

    public void ShowCredits()
    {
        if (CreditsScreen != null)
        {
            CreditsScreen.SetActive(true);
            _inCreditsMenu = true;
        }
    }

    public void HideCredits()
    {
        if (CreditsScreen != null)
        {
            CreditsScreen.SetActive(false);
            _inCreditsMenu = false;
        }
    }
}
