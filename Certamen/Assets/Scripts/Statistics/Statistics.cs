using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Statistics : MonoBehaviour
{
    // Bek�ri azt a Text objektumot amire ki�rja az eredm�nyt
    public Text grassCountText;
    public Text rabbitCountText;
    // V�ltoz�k l�trehoz�sa
    private int grassObjectCount = 0;
    private int rabbitObjectCount = 0;

    void Update()
    {
        UpdateStatistics();
    }

    void UpdateStatistics()
    {
        // Kisz�molja a f� objektumok sz�m�t
        GameObject[] grassObjects = GameObject.FindGameObjectsWithTag("Grass");
        GameObject[] rabbitObjects = GameObject.FindGameObjectsWithTag("Rabbit");

        grassObjectCount = grassObjects.Length;
        rabbitObjectCount = rabbitObjects.Length;

        // Friss�t� a statisztik�kat a felhaszn�l�i fel�leten
        grassCountText.text = "F�csom�k sz�ma: " + grassObjectCount;
        rabbitCountText.text = "Nyulak sz�ma: " + rabbitObjectCount;
    }
}
