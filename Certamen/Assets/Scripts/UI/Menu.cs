using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    // A MenuObjectet fogja megjeleníteni
    public GameObject MenuObject;
    // Ezzel eszközöljük a várakozást
    private float WaitTime = 0;

    // Az elején meghívjuk a "PauseMenu()" ciklust
    void Start()
    {
        PauseMenu();
    }

    void PauseMenu()
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
                    // Játék sebességét visszaállítja
                    Time.timeScale = 1f;
                    // Várakozási idõ megnövekszik (azért 10x több mint mikor megállítjuk, mert az idõ is 10x gyorsabb)
                    WaitTime = 0.5f;
                }
            }
        }

        // Meghívja a "callPauseMenu()" függvényt
        Invoke("callPauseMenu", WaitTime);
    }

    // Ez a függvény újból és újból meghívja a "PauseMenu()" függvényt
    void callPauseMenu()
    {
        Invoke("PauseMenu", WaitTime);
    }
}
