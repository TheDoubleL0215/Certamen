using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Statistics : MonoBehaviour
{
    // Bekéri azt a Text objektumot amire kiírja az eredményt
    public Text grassCountText;
    public Text rabbitCountText;
    // Változók létrehozása
    private int grassObjectCount = 0;
    private int rabbitObjectCount = 0;

    void Update()
    {
        UpdateStatistics();
    }

    void UpdateStatistics()
    {
        // Kiszámolja a fû objektumok számát
        GameObject[] grassObjects = GameObject.FindGameObjectsWithTag("Grass");
        GameObject[] rabbitObjects = GameObject.FindGameObjectsWithTag("Rabbit");

        grassObjectCount = grassObjects.Length;
        rabbitObjectCount = rabbitObjects.Length;

        // Frissítí a statisztikákat a felhasználói felületen
        grassCountText.text = "Fûcsomók száma: " + grassObjectCount;
        rabbitCountText.text = "Nyulak száma: " + rabbitObjectCount;
    }
}
