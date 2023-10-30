using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] GameObject rabbitScript;
    // Megállítja a játék idejét
    public void Pause()
    {
        Time.timeScale = 0f;
    }

    // Elindítja a játék idejét
    public void Play()
    {
     //   menu.SetActive(false);
        Time.timeScale = 1f;
    }

    // Megállítja a játék iejét és a menüt megjeleníti
    public void PauseMenu()
    {
       // menu.SetActive(true);
        Time.timeScale = 0f;
    }


    void Update()
    {
        // Escape gomb megnyomására megállítja a játék iejét és a menüt megjeleníti
        if (Input.GetKey(KeyCode.Escape))
        {
            Time.timeScale = 0f;
        }

        if (Input.GetKey(KeyCode.P))
        {
           // rabbitScript.GetComponent("Rabbit Move").enabled = false;
            //Time.timeScale = 0f;
        }
    }
}
