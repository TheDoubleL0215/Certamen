using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] GameObject menu;
    // Meg�ll�tja a j�t�k idej�t
    public void Pause()
    {
        Time.timeScale = 0f;
    }

    // Elind�tja a j�t�k idej�t
    public void Play()
    {
        menu.SetActive(false);
        Time.timeScale = 1f;
    }

    // Meg�ll�tja a j�t�k iej�t �s a men�t megjelen�ti
    public void PauseMenu()
    {
        menu.SetActive(true);
        Time.timeScale = 0f;
    }


    void Update()
    {
        // Escape gomb megnyom�s�ra meg�ll�tja a j�t�k iej�t �s a men�t megjelen�ti
        if (Input.GetKey(KeyCode.Escape))
        {
            menu.SetActive(true);
            Time.timeScale = 0f;
        }
    }
}
