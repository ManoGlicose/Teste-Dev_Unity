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

    // Update is called once per frame
    void Update()
    {
        // Define os textos com as informações do jogador se ele houver. Se não, insinua o jogador a ver os controles antes de começar
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

    public void StartGame() // Comerça a carregar a cena do jogo
    {
        menuCanvas.blocksRaycasts = false;
        StartCoroutine(StartButton());
    }

    public void ShowHideControls(bool show) // Mostra e esconde a tela com os controles
    {
        menuCanvas.alpha = show ? 0 : 1;
        menuCanvas.blocksRaycasts = !show;

        controlsCanvas.alpha = show ? 1 : 0;
        controlsCanvas.blocksRaycasts = show;
    }

    public void ExitGame() // Fecha o jogo
    {
        Application.Quit();
    }

    IEnumerator StartButton() // Contagem para o início do jogo
    {
        yield return new WaitForSeconds(1);

        SceneManager.LoadScene(1);
    }
}
