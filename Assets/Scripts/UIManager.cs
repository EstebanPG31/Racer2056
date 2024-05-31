using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject HUDPanel;
    public GameObject pausePanel;
    public GameObject countdownPanel;
    public GameObject car;
    public TextMeshProUGUI count;
    void Start()
    {
        car.GetComponent<CarController>().enabled=false;
        StartCoroutine(Countdown());
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
        //Time.timeScale = 1.0f;
    }

    public void ShowPause()
    {
        CleanPanel();
        pausePanel.SetActive(true);
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
    }

    /*public void ExitGame()
    {
       Time.timeScale = 1.0f;
        SceneManager.LoadScene(0);
        //Application.Quit();
    }*/
}
