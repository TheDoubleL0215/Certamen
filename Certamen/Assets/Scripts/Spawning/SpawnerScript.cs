using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerScript : MonoBehaviour
{
    [Header("Grass")]
    // Kiválasztjuk melyik karaktert spawnolja
    public GameObject Grass;
    //Parent container 
    public Transform grassParentObj;
    // Ettõl függ, hány fûcsomó spawnol a legelején.
    public int grassStartAmount;
    // Hány másodpercenként spawnol új fûcsomó
    public float grassSpawnRateTime;
    // Ennyi fûcsomó spawnol egy adott spawnolásnál
    public float grassSpawnRatePerSpawning;
    // Kiválasztjuk melyik karaktert spawnolja


    [Header("Rabbit")]
    public GameObject Rabbit;
    //Parent container 
    public GameObject nameTag;
    public Transform nameTagParent;
    public Transform rabbitParentObj;
    // Ettõl függ, hány nyul spawnol a legelején.
    public int rabbitStartAmount;
    // Megadjuk a kordinátákat, amin belül spawnol a fû

    [Header("Fox")]
    public GameObject Fox;
    //Parent container 
    public Transform foxParentObj;
    // Ettõl függ, hány nyul spawnol a legelején.
    public int foxStartAmount;
    // Megadjuk a kordinátákat, amin belül spawnol a fû

    [Header("Positioning")]
    public float lowestX;
    public float highestX;
    public float lowestZ;
    public float highestZ;
    // Megadott fokokon belül véletlen irány a kezdésnél
    private float minRotation = 0f;
    private float maxRotation = 90f;
    // Idõ múlását mérjük

    [Header("Timer")]
    private float grassTimer = 0;
    void Start()
    {
        // 'startAmount'-szor spawnol egy füvet a 'spawnGrass()' függvényt meghívva
        for (int i = 0; i < grassStartAmount; i++)
        {
            spawnGrass();
        }
        for (int i = 0; i < rabbitStartAmount; i++)
        {
            spawnRabbit();
        }
        for (int i = 0; i < foxStartAmount; i++)
        {
            spawnFox();
        }
    }

    void Update()
    {
        // Csak abban esetben hívja meg a 'spawnGrass()' függvényt, ha a 'timer'
        // megegyezik a 'spawnRate'-ben meadottal.
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

    // A bekért paraméterek szerint spawnol egy fûcsomót
    void spawnGrass()
    {
        Instantiate(Grass, new Vector3(Random.Range(lowestX, highestX), 0, Random.Range(lowestZ, highestZ)), Quaternion.identity, grassParentObj.transform);
    }

    void spawnRabbit()
    {
        Vector3 position = new Vector3(Random.Range(lowestX, highestX), 0.19f, Random.Range(lowestZ, highestZ));
        GameObject RabbitObj = Instantiate(Rabbit, position, Quaternion.identity, rabbitParentObj);

        float randomRotation = Random.Range(minRotation, maxRotation);
        transform.Rotate(Vector3.up, randomRotation);

        rabbitBehaviour RabbitBehaviour = RabbitObj.GetComponent<rabbitBehaviour>();

        // Enable the script component if it exists
        if (RabbitBehaviour != null)
        {
            RabbitBehaviour.enabled = true;
        }
    }

    void spawnFox()
    {
        // Set the initial rotation values
        Vector3 initialRotation = new Vector3(-90f, Random.Range(0f, 360f), 90f);

        // Create a Quaternion based on the initial rotation
        Quaternion rotation = Quaternion.Euler(initialRotation);

        // Set the spawn position
        Vector3 position = new Vector3(Random.Range(lowestX, highestX), 0.19f, Random.Range(lowestZ, highestZ));

        // Instantiate the fox with the desired rotation
        GameObject FoxObj = Instantiate(Fox, position, rotation, foxParentObj);

        FoxManager foxManager = FoxObj.GetComponent<FoxManager>();

        // Enable the script component if it exists
        if (foxManager != null)
        {
            foxManager.enabled = true;
        }
    }
}
