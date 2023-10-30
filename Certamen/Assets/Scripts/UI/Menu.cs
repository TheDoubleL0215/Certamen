using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] GameObject menu;
    // Megállítja a játék idejét
    public void Pause()
    {
        Time.timeScale = 0f;
    }

    // Elindítja a játék idejét
    public void Play()
    {
        menu.SetActive(false);
        Time.timeScale = 1f;
    }

    // Megállítja a játék iejét és a menüt megjeleníti
    public void PauseMenu()
    {
        menu.SetActive(true);
        Time.timeScale = 0f;
    }


    void Update()
    {
        // Escape gomb megnyomására megállítja a játék iejét és a menüt megjeleníti
        if (Input.GetKey(KeyCode.Escape))
        {
            menu.SetActive(true);
            Time.timeScale = 0f;
        }
    }
}
