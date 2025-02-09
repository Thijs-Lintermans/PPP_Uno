using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{

    [SerializeField] string webUrl = "";
    [SerializeField] GameObject HowToPlayPanel;

    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

    public void HowToPlay()
    {
        if(HowToPlayPanel != null)
        {
            HowToPlayPanel.SetActive(true);
        }
    }

    public void ShowWebsite()
    {
        Application.OpenURL(webUrl);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
