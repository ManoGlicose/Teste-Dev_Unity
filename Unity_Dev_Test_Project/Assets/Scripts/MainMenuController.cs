using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("UI")]
    public CanvasGroup menuCanvas;
    public CanvasGroup controlsCanvas;
    public Text waveText;
    public Text plantsText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerPrefs.GetInt("MaxWave") > 0)
        {
            waveText.text = "ONDA MÁXIMA: " + PlayerPrefs.GetInt("MaxWave");
            plantsText.text = "PLANTAS EXTERMINADAS: " + PlayerPrefs.GetInt("AllPlants");
        } else
        {
            waveText.text = "VEJA OS CONTOLES";
            plantsText.text = "ANTES DE COMEÇAR";
        }
    }

    public void StartGame()
    {
        menuCanvas.blocksRaycasts = false;
        StartCoroutine(StartButton());
    }

    public void ShowHideControls(bool show)
    {
        menuCanvas.alpha = show ? 0 : 1;
        menuCanvas.blocksRaycasts = !show;

        controlsCanvas.alpha = show ? 1 : 0;
        controlsCanvas.blocksRaycasts = show;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    IEnumerator StartButton()
    {
        yield return new WaitForSeconds(1);

        SceneManager.LoadScene(1);
    }
}
