using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Components")]
    Controls controls;

    [Header("UI")]
    public CanvasGroup pauseScreen;

    private void Awake()
    {
        controls = new Controls();

        controls.Menus.Pause.performed += _ => Pause(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Pause(bool pause)
    {
        Time.timeScale = pause ? 0 : 1;

        pauseScreen.blocksRaycasts = pause;
        pauseScreen.alpha = pause ? 1 : 0;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        SceneManager.LoadScene(0);
    }

    #region Enable/Disable Controls
    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
    #endregion
}
