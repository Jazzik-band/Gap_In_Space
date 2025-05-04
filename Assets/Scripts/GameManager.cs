using System;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager: MonoBehaviour
{
    public Button start, settings, quit, back;
    public GameObject menuPanel, staminaUI, inventoryPanel, batteryValue;
    public GameObject continueButton, restart, menuSettings, mainMenu, menuQuit, menuBack;
    // private bool isOpenMenu = false;

    public void StartGame()
    {
        SceneManager.LoadScene("Hub");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            menuPanel.SetActive(true);
            staminaUI.SetActive(false);
            inventoryPanel.SetActive(false);
            batteryValue.SetActive(false);
            FlashlightController.isFlashLightOn = false;
        }
    }

    public void Continue()
    {
        menuPanel.SetActive(false);
        staminaUI.SetActive(true);
        inventoryPanel.SetActive(true);
        batteryValue.SetActive(true);
    }

    public void MainMenuSettings()
    {
        start.gameObject.SetActive(false);
        settings.gameObject.SetActive(false);
        quit.gameObject.SetActive(false);
        back.gameObject.SetActive(true);
    }

    public void MenuSettings()
    {
        continueButton.SetActive(false);
        restart.SetActive(false);
        menuSettings.SetActive(false);
        mainMenu.SetActive(false);
        menuQuit.SetActive(false);
        menuBack.SetActive(true);
    }

    public void MenuBack()
    {
        continueButton.SetActive(true);
        restart.SetActive(true);
        menuSettings.SetActive(true);
        mainMenu.SetActive(true);
        menuQuit.SetActive(true);
        menuBack.SetActive(false);
    }
    public void MainMenuBack()
    {
        start.gameObject.SetActive(true);
        settings.gameObject.SetActive(true);
        quit.gameObject.SetActive(true);
        back.gameObject.SetActive(false);
    }

    public void QuitToMainMenu()
    {
        SceneManager.LoadScene("Main menu");
    }

    public void Quit()
    {
        Application.Quit();
    }

}
