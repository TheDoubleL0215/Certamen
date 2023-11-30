using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class rabbitBehaviour : MonoBehaviour
{
    //VÁLTOZÓK//
    [Header("ID")]
    public int id; // egyéni azonosító
    public int fatherId; // örökölt azonosító
    [Header("Reproduction")]
    public int fertility; // ez határozza meg, hány kölyke lehet a nyúlnak
    public float maturity; // érettség, szaporodásban van szerepe
    public float maturityLimit; // ezt az értéket elérve, végbe megy a szaporodás
    [Header("Other")]
    public float age; // nyúl életkora
    public float lifeTime; // ha eléri ezt, megdöglik
    public GameObject Rabbit; // ezt az objektumot fogjuk klónozni szapodrodásnál

    void Start()
    {
        id = Random.Range(10000, 99999); // ázonosító "sorsolása"
        maturity = Random.Range(0f, maturityLimit); // lespawnolt nyulak érettsége véletlen
        age = 0f;
        if (fatherId != 0)
        {
            maturity = 0f; // ha már egy született és nem spawnolt nyúl, akkor alapból 0 az érettsége
        }
    }

    void Update()
    {
        // folyamatosan növeljük az érettséget és a kort
        maturity += Time.deltaTime;
        age += Time.deltaTime;
        // ha az érettség eléri a mehatározott szintet
        if (maturity >= maturityLimit)
        {
            for (int i = 0; i < fertility; i++) // "fertility" változó értékeszer meghívja a "Reproduction()" függvényt
            {
                Reproduction();
            }
            maturity = 0f; //nullázódik a maturity
        }
        // elöregedett nyulak elpusztulnak
        if (age >= lifeTime)
        {
            Destroy(gameObject);
        }

    }

    //ÚJ EGYED SZÜLETÉSE
    void Reproduction()
    {
        GameObject newRabbit = Instantiate(Rabbit, transform.position, transform.rotation); //klónozzuk a Rabbit objektumot

        // Definiáld a pályaterület határait
        float minX = -75f;
        float maxX = 75f;
        float minZ = -75f;
        float maxZ = 75f;

        Vector3 offset = new Vector3(Random.Range(-1f, 1f), 0.0f, Random.Range(-1f, 1f)); // Távolság a szülő nyúltól
        Vector3 newPosition = transform.position + offset;

        // Korlátozd a kis nyúl pozícióját a pálya határai között
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.z = Mathf.Clamp(newPosition.z, minZ, maxZ);

        newRabbit.transform.position = newPosition;

        // Az új egyed megörökli a szülő értékeit kisebb módosulásokkal
        rabbitBehaviour newRabbitScript = newRabbit.GetComponent<rabbitBehaviour>();
        newRabbitScript.fatherId = id;
    }
}
