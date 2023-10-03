using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject certifiedWinner;

    private void Start()
    {
        if (PlayerStats.CurrentLevel == 4)
        {
            PlayerStats.CurrentLevel = 1;
            PlayerStats.IsWinner = true;
        }

        if (PlayerStats.IsWinner)
        {
            certifiedWinner.SetActive(true);
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
