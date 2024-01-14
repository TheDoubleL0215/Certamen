using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnerScript : MonoBehaviour
{
    [Header("Grass")]
    [SerializeField] private bool spawningOfGrass = true;
    // Kiv�lasztjuk melyik karaktert spawnolja
    public GameObject Grass;
    //Parent container 
    public Transform grassParentObj;
    // Ett�l f�gg, h�ny f�csom� spawnol a legelej�n.
    public int grassStartAmount;
    // H�ny m�sodpercenk�nt spawnol �j f�csom�
    public float grassSpawnRateTime;
    // Ennyi f�csom� spawnol egy adott spawnol�sn�l
    public float grassSpawnRatePerSpawning;
    // Kiv�lasztjuk melyik karaktert spawnolja


    [Header("Rabbit")]
    [SerializeField] private bool spawningOfRabbit = true;
    public GameObject Rabbit;
    //Parent container 
    public GameObject nameTag;
    public Transform nameTagParent;
    public Transform rabbitParentObj;
    // Ett�l f�gg, h�ny nyul spawnol a legelej�n.
    public int rabbitStartAmount;
    // Megadjuk a kordin�t�kat, amin bel�l spawnol a f�

    [Header("Fox")]
    [SerializeField] private bool spawningOfFox = true;
    public GameObject Fox;
    //Parent container 
    public Transform foxParentObj;
    // Ett�l f�gg, h�ny nyul spawnol a legelej�n.
    public int foxStartAmount;
    // Megadjuk a kordin�t�kat, amin bel�l spawnol a f�

    [Header("Positioning")]
    public float lowestX;
    public float highestX;
    public float lowestZ;
    public float highestZ;
    // Megadott fokokon bel�l v�letlen ir�ny a kezd�sn�l
    private float minRotation = 0f;
    private float maxRotation = 90f;

    public float spawnHeight;
    // Id� m�l�s�t m�rj�k

    [Header("Timer")]
    private float grassTimer = 0;
    void Start()
    {
        // 'startAmount'-szor spawnol egy f�vet a 'spawnGrass()' f�ggv�nyt megh�vva
        if(spawningOfGrass){
            for (int i = 0; i < grassStartAmount; i++)
            {
                spawnGrass();
            }
        }
        if(spawningOfRabbit){
            for (int i = 0; i < rabbitStartAmount; i++)
            {
                spawnRabbit();
            }
        }
        if(spawningOfFox){
            for (int i = 0; i < foxStartAmount; i++)
            {
                spawnFox();
            }
        }
    }

    void Update()
    {
        // Csak abban esetben h�vja meg a 'spawnGrass()' f�ggv�nyt, ha a 'timer'
        // megegyezik a 'spawnRate'-ben meadottal.
        if(spawningOfGrass){
            if (grassTimer >= grassSpawnRateTime)
            {
                for (int i = 0; i < grassSpawnRatePerSpawning; i++)
                {
                    spawnGrass();
                }
                grassTimer = 0;
            }
            else
            {
                grassTimer = grassTimer + Time.deltaTime;
            }
        }
    }

    // A bek�rt param�terek szerint spawnol egy f�csom�t
    void spawnGrass()
    {
        bool placed = false;

        while (!placed)
        {
            float tryPositionX = Random.Range(lowestX, highestX);
            float tryPositionZ = Random.Range(lowestZ, highestZ);

            Vector3 raycastStart = new Vector3(tryPositionX, 100f, tryPositionZ);

            if (Physics.Raycast(raycastStart, Vector3.down, out RaycastHit hit, Mathf.Infinity))
            {
                if (hit.point.y > 12.5f)
                {
                    Instantiate(Grass, new Vector3(tryPositionX, hit.point.y, tryPositionZ), Quaternion.identity, grassParentObj.transform);
                    placed = true;
                }
            }
        }
    }

    void spawnRabbit()
    {
        bool placed = false;

        while (!placed)
        {
            float tryPositionX = Random.Range(lowestX, highestX);
            float tryPositionZ = Random.Range(lowestZ, highestZ);

            Vector3 raycastStart = new Vector3(tryPositionX, 100f, tryPositionZ);

            if (Physics.Raycast(raycastStart, Vector3.down, out RaycastHit hit, Mathf.Infinity))
            {
                if (hit.point.y > 12.5f)
                {
                    Vector3 position = new Vector3(Random.Range(lowestX, highestX), spawnHeight, Random.Range(lowestZ, highestZ));
                    GameObject RabbitObj = Instantiate(Rabbit, position, Quaternion.identity, rabbitParentObj);

                    float randomRotation = Random.Range(minRotation, maxRotation);
                    transform.Rotate(Vector3.up, randomRotation);
                    placed = true;
                }
            }
        }
    }

    void spawnFox()
    {

        bool placed = false;

        while (!placed)
        {
            float tryPositionX = Random.Range(lowestX, highestX);
            float tryPositionZ = Random.Range(lowestZ, highestZ);

            Vector3 raycastStart = new Vector3(tryPositionX, 100f, tryPositionZ);

            if (Physics.Raycast(raycastStart, Vector3.down, out RaycastHit hit, Mathf.Infinity))
            {
                if (hit.point.y > 12.5f)
                {
                    // Set the initial rotation values
                    Vector3 initialRotation = new Vector3(-90f, Random.Range(0f, 360f), 90f);

                    // Create a Quaternion based on the initial rotation
                    Quaternion rotation = Quaternion.Euler(initialRotation);

                    // Set the spawn position
                    Vector3 position = new Vector3(Random.Range(lowestX, highestX), spawnHeight, Random.Range(lowestZ, highestZ));

                    // Instantiate the fox with the desired rotation
                    GameObject FoxObj = Instantiate(Fox, position, rotation, foxParentObj);
                    placed = true;
                }
            }
        }

        
    }
}
