using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class FoxBehaviour : MonoBehaviour
{

    [Header("ID")]
    public int id; // egy�ni azonos�t�
    public int fatherId; // �r�k�lt azonos�t�

    [Header("Reproduction")]
    public int fertility; // ez hat�rozza meg, h�ny k�lyke lehet a ny�lnak
    public float maturity; // �retts�g, szaporod�sban van szerepe
    public float maturityLimit; // ezt az �rt�ket el�rve, v�gbe megy a szaporod�s

    [Header("Other")]
    public float age; // ny�l �letkora
    public float lifeTime; // ha el�ri ezt, megd�glik
    public GameObject Fox; // ezt az objektumot fogjuk kl�nozni szapodrod�sn�l 

    void Start()
    {
        id = Random.Range(10000, 99999); // �zonos�t� "sorsol�sa"
        maturity = Random.Range(5f, maturityLimit); // lespawnolt nyulak �retts�ge v�letlen
        age = 0f;
        if (fatherId != 0)
        {
            maturity = 0f; // ha m�r egy sz�letett �s nem spawnolt ny�l, akkor alapb�l 0 az �retts�ge
        }
    }

    // Update is called once per frame
    void Update()
    {
        maturity += Time.deltaTime;
        age += Time.deltaTime;
        // ha az �retts�g el�ri a mehat�rozott szintet
        if (maturity >= maturityLimit)
        {
            for (int i = 0; i < fertility; i++) // "fertility" v�ltoz� �rt�keszer megh�vja a "Reproduction()" f�ggv�nyt
            {
                Reproduction();
            }
            maturity = 0f; //null�z�dik a maturity
        }
        // el�regedett nyulak elpusztulnak
        if (age >= lifeTime)
        {
            Destroy(gameObject);
        }
    }

    void Reproduction()
    {
        GameObject newFox = Instantiate(Fox, transform.position, transform.rotation); //kl�nozzuk a Rabbit objektumot

        // Defini�ld a p�lyater�let hat�rait
        float minX = -75f;
        float maxX = 75f;
        float minZ = -75f;
        float maxZ = 75f;

        Vector3 offset = new Vector3(Random.Range(-1f, 1f), 0.0f, Random.Range(-1f, 1f)); // T�vols�g a sz�l� ny�lt�l
        Vector3 newPosition = transform.position + offset;

        // Korl�tozd a kis ny�l poz�ci�j�t a p�lya hat�rai k�z�tt
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.z = Mathf.Clamp(newPosition.z, minZ, maxZ);

        newFox.transform.position = newPosition;

        // Az �j egyed meg�r�kli a sz�l� �rt�keit kisebb m�dosul�sokkal
        FoxBehaviour newFoxScript = newFox.GetComponent<FoxBehaviour>();
        newFoxScript.fatherId = id;
    }
}
