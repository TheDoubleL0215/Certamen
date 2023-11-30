using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    // A MenuObjectet fogja megjelen�teni
    public GameObject StatsPanel;
    public GameObject MenuObject;
    public GameObject StatisticsObject;
    // Ezzel eszk�z�lj�k a v�rakoz�st
    private float WaitTime = 0;

    // Az elej�n megh�vjuk a "PauseMenu()" ciklust
    void Start()
    {
        Detect();
    }

    void Detect()
    {
        // Mikor bel�p�nk a ciklusba 0 lesz a v�rakoz�s id�
        WaitTime = 0;
        // Escape gomb lenyom�s�ra...
        if (Input.GetKey(KeyCode.Escape))
        {
            // ...ha nem volt megjelen�tve a men�, megjelen�ti
            if (MenuObject.activeInHierarchy == false)
            {
                MenuObject.SetActive(true);
                StatisticsObject.SetActive(false);
                StatsPanel.SetActive(false);
                // A j�t�k idej�t lelass�tja
                Time.timeScale = 0.1f;
                // Megn�veli a v�rakoz�s id�t, a bugok elker�l�se v�gett
                WaitTime = 0.05f;
            }
            // ...ha meg volt jelen�tve a men�, elt�nteti
            else
            {
                if (MenuObject.activeInHierarchy == true)
                {
                    MenuObject.SetActive(false);
                    StatsPanel.SetActive(true);
                    // J�t�k sebess�g�t vissza�ll�tja
                    Time.timeScale = 1f;
                    // V�rakoz�si id� megn�vekszik (az�rt 10x t�bb mint mikor meg�ll�tjuk, mert az id� is 10x gyorsabb)
                    WaitTime = 0.5f;
                }
            }
        }
        if (Input.GetKey(KeyCode.G))
        {
            // ...ha nem volt megjelen�tve a men�, megjelen�ti
            if (StatisticsObject.activeInHierarchy == false)
            {
                StatisticsObject.SetActive(true);
                MenuObject.SetActive(false);
                StatsPanel.SetActive(false);
                // A j�t�k idej�t lelass�tja
                Time.timeScale = 0.1f;
                // Megn�veli a v�rakoz�s id�t, a bugok elker�l�se v�gett
                WaitTime = 0.05f;
            }
            // ...ha meg volt jelen�tve a men�, elt�nteti
            else
            {
                if (StatisticsObject.activeInHierarchy == true)
                {
                    StatisticsObject.SetActive(false);
                    StatsPanel.SetActive(true);
                    // J�t�k sebess�g�t vissza�ll�tja
                    Time.timeScale = 1f;
                    // V�rakoz�si id� megn�vekszik (az�rt 10x t�bb mint mikor meg�ll�tjuk, mert az id� is 10x gyorsabb)
                    WaitTime = 0.5f;
                }
            }
        }

        // Megh�vja a "callPauseMenu()" f�ggv�nyt
        Invoke("callDetect", WaitTime);
    }

    // Ez a f�ggv�ny �jb�l �s �jb�l megh�vja a "PauseMenu()" f�ggv�nyt
    void callDetect()
    {
        Invoke("Detect", WaitTime);
    }
}
