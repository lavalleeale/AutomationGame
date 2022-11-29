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
        GameManager.openGUIs.Remove(GameManager.GUIType.menu);
        saveMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void Return()
    {
        GameManager.openGUIs.Remove(GameManager.GUIType.menu);
        Time.timeScale = 1;
        mainMenu.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && GameManager.openGUIs.Count == 0)
        {
            GameManager.openGUIs.Add(GameManager.GUIType.menu);
            Time.timeScale = 0;
            mainMenu.SetActive(true);
        }
        else if (
            Input.GetKeyDown(KeyCode.Escape) && GameManager.openGUIs.Contains(GameManager.GUIType.menu)
        )
        {
            GameManager.openGUIs.Remove(GameManager.GUIType.menu);
            Time.timeScale = 1;
            mainMenu.SetActive(false);
            saveMenu.SetActive(false);
        }
    }
}
