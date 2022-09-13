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
        // Define os textos com as informa��es do jogador se ele houver. Se n�o, insinua o jogador a ver os controles antes de come�ar
        if (PlayerPrefs.GetInt("MaxWave") > 0)
        {
            waveText.text = "ONDA M�XIMA: " + PlayerPrefs.GetInt("MaxWave");
            plantsText.text = "PLANTAS EXTERMINADAS: " + PlayerPrefs.GetInt("AllPlants");
        } else
        {
            waveText.text = "VEJA OS CONTOLES";
            plantsText.text = "ANTES DE COME�AR";
        }
    }

    public void StartGame() // Comer�a a carregar a cena do jogo
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

    IEnumerator StartButton() // Contagem para o in�cio do jogo
    {
        yield return new WaitForSeconds(1);

        SceneManager.LoadScene(1);
    }
}
