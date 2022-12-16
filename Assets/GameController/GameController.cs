using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameController : MonoBehaviour
{
    [SerializeField] GameObject canvas;
    //Controls
    public PlayerControls.AnyStateActions anyActionsControl;
    
    private bool menuOpen;

    private void Start()
    {
        anyActionsControl = new PlayerControls().AnyState;
        anyActionsControl.Enable();

        canvas.transform.Find("YouLostTxt").gameObject.SetActive(false);
        canvas.transform.Find("YouWonTxt").gameObject.SetActive(false);
        HideMenu();
    }

    private void Update()
    {


        if (anyActionsControl.BringUpMenu.IsPressed() && !menuOpen)
            ShowMenu();
        
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Application.Quit(); 
    }

    public void LostGame()
    {
        canvas.transform.Find("YouLostTxt").gameObject.SetActive(true);
        canvas.transform.Find("ResumeGameBtn").gameObject.SetActive(false);
        ShowMenu();
    }
    public void WonGame()
    {
        canvas.transform.Find("YouWonTxt").gameObject.SetActive(true);
        canvas.transform.Find("ResumeGameBtn").gameObject.SetActive(false);
        ShowMenu();
    }
    
    public void HideMenu()
    {
        canvas.SetActive(false);
        Time.timeScale = 1;
        menuOpen = false;
    }

    public void ShowMenu()
    {
        canvas.SetActive(true);
        Time.timeScale = 0;
        menuOpen = true;
    }
}
