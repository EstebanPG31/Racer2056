using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject HUDPanel;
    public GameObject pausePanel;
    void Start()
    {
        ShowHUD();
    }

    public void Update()
    {
       
    }

    public void CleanPanel()
    {
        HUDPanel.SetActive(false);
        pausePanel.SetActive(false);
    }

    public void ShowHUD()
    {
        CleanPanel();
        HUDPanel.SetActive(true);
        //Time.timeScale = 1.0f;
    }

    public void ShowPause()
    {
        CleanPanel();
        pausePanel.SetActive(true);
        //Time.timeScale = 0.0f;
    }

    public void ExitGame()
    {
       Time.timeScale = 1.0f;
        Application.Quit();
    }
}
