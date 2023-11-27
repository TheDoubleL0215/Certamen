using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    // A MenuObjectet fogja megjeleníteni
    public GameObject StatsPanel;
    public GameObject MenuObject;
    public GameObject ChartsObject;
    public GameObject SpiralObject;
    public GameObject DecorObject;

    // Ezzel eszközöljük a várakozást
    private float WaitTime = 0;

    // Az elején meghívjuk a "PauseMenu()" ciklust
    void Start()
    {
        Detect();
    }

    void Detect()
    {
        // Mikor belépünk a ciklusba 0 lesz a várakozás idõ
        WaitTime = 0;
        // Escape gomb lenyomására...
        if (Input.GetKey(KeyCode.Escape))
        {
            // ...ha nem volt megjelenítve a menü, megjeleníti
            if (MenuObject.activeInHierarchy == false)
            {
                MenuObject.SetActive(true);
                ChartsObject.SetActive(false);
                StatsPanel.SetActive(false);
                SpiralObject.SetActive(false);
                DecorObject.SetActive(false);
                // A játék idejét lelassítja
                Time.timeScale = 0.1f;
                // Megnöveli a várakozás idõt, a bugok elkerülése végett
                WaitTime = 0.05f;
            }
            // ...ha meg volt jelenítve a menü, eltünteti
            else
            {
                if (MenuObject.activeInHierarchy == true)
                {
                    MenuObject.SetActive(false);
                    StatsPanel.SetActive(true);
                    // Játék sebességét visszaállítja
                    Time.timeScale = 1f;
                    // Várakozási idõ megnövekszik (azért 10x több mint mikor megállítjuk, mert az idõ is 10x gyorsabb)
                    WaitTime = 0.5f;
                }
            }
        }
        if (Input.GetKey(KeyCode.C))
        {
            // ...ha nem volt megjelenítve a menü, megjeleníti
            if (ChartsObject.activeInHierarchy == false)
            {
                ChartsObject.SetActive(true);
                DecorObject.SetActive(true);
                SpiralObject.SetActive(false);
                MenuObject.SetActive(false);
                StatsPanel.SetActive(false);
                // A játék idejét lelassítja
                Time.timeScale = 0.1f;
                // Megnöveli a várakozás idõt, a bugok elkerülése végett
                WaitTime = 0.05f;
            }
            // ...ha meg volt jelenítve a menü, eltünteti
            else
            {
                if (ChartsObject.activeInHierarchy == true)
                {
                    ChartsObject.SetActive(false);
                    DecorObject.SetActive(false);
                    StatsPanel.SetActive(true);
                    // Játék sebességét visszaállítja
                    Time.timeScale = 1f;
                    // Várakozási idõ megnövekszik (azért 10x több mint mikor megállítjuk, mert az idõ is 10x gyorsabb)
                    WaitTime = 0.5f;
                }
            }
        }

        if (Input.GetKey(KeyCode.V))
        {
            // ...ha nem volt megjelenítve a menü, megjeleníti
            if (SpiralObject.activeInHierarchy == false)
            {
                SpiralObject.SetActive(true);
                DecorObject.SetActive(true);
                MenuObject.SetActive(false);
                StatsPanel.SetActive(false);
                ChartsObject.SetActive(false);
                // A játék idejét lelassítja
                Time.timeScale = 0.1f;
                // Megnöveli a várakozás idõt, a bugok elkerülése végett
                WaitTime = 0.05f;
            }
            // ...ha meg volt jelenítve a menü, eltünteti
            else
            {
                if (SpiralObject.activeInHierarchy == true)
                {
                    SpiralObject.SetActive(false);
                    DecorObject.SetActive(false);
                    StatsPanel.SetActive(true);
                    // Játék sebességét visszaállítja
                    Time.timeScale = 1f;
                    // Várakozási idõ megnövekszik (azért 10x több mint mikor megállítjuk, mert az idõ is 10x gyorsabb)
                    WaitTime = 0.5f;
                }
            }
        }

        // Meghívja a "callPauseMenu()" függvényt
        Invoke("callDetect", WaitTime);
    }

    // Ez a függvény újból és újból meghívja a "PauseMenu()" függvényt
    void callDetect()
    {
        Invoke("Detect", WaitTime);
    }
}
