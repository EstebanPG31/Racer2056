using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject HUDPanel;
    public GameObject pausePanel;
    public GameObject countdownPanel;
    public GameObject car;
    public TextMeshProUGUI count;
    public TextMeshProUGUI raceTime;
    private float startTime;
    private float timer = 0f;
    private bool timePaused = false;

    void Start()
    {

        car.GetComponent<CarController>().enabled=false;
        StartCoroutine(Countdown());
    }

    private void Update()
    {
        if (!timePaused) { 
            timer = Time.time - startTime;
            string minutes = ((int)timer/60).ToString("00");
            string seconds = (timer % 60).ToString("00");
            string miliseconds = Mathf.Floor((timer * 1000) % 1000).ToString("000");

            raceTime.text = minutes + ":" + seconds + ":" + miliseconds;
        }
    }

    public void CleanPanel()
    {
        countdownPanel.SetActive(false);
        HUDPanel.SetActive(false);
        pausePanel.SetActive(false);
    }

    public void ShowHUD()
    {
        CleanPanel();
        HUDPanel.SetActive(true);
        timePaused = false;
        startTime = Time.time - (Time.time - startTime);
        //Time.timeScale = 1.0f;
    }

    public void ShowPause()
    {
        CleanPanel();
        pausePanel.SetActive(true);
        timePaused = true;
        //Time.timeScale = 0.0f;
    }
    public IEnumerator Countdown()
    {
        yield return new WaitForSeconds(1);
        countdownPanel.SetActive(true);
        for (int i = 3; i > 0; i--)
        {
            count.text = i.ToString();
            yield return new WaitForSeconds(1);
        }
        count.text = "GO!";
        car.GetComponent<CarController>().enabled = true;
        yield return new WaitForSeconds(1);
        ShowHUD();
        startTime = Time.time;
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
