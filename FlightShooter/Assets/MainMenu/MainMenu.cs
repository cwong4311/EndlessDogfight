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
    private Coroutine _levelLoadCoroutine;

    public void Awake()
    {
        LoadingScreen.SetActive(false);
    }

    public void StartGame()
    {
        if (_levelLoadCoroutine == null)
        {
            _levelLoadCoroutine = StartCoroutine(LoadLevelAsync());
        }
    }

    public void QuitGame()
    {
        if (_levelLoadCoroutine == null)
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
}
