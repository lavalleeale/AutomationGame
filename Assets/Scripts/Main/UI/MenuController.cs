using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject mainMenu,
        saveMenu;

    public void ShowSaveMenu()
    {
        mainMenu.SetActive(false);
        saveMenu.SetActive(true);
    }

    public void HideSaveMenu()
    {
        saveMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void Return()
    {
        Time.timeScale = 1;
        mainMenu.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Time.timeScale = 0;
            mainMenu.SetActive(true);
        }
    }
}