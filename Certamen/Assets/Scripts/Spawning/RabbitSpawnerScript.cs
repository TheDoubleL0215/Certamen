using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RabbitSpawnerScript : MonoBehaviour
{
    // Kiválasztjuk melyik karaktert spawnolja
    public GameObject Rabbit;
    //Parent container 
    public GameObject nameTag;
    public Transform nameTagParent;
    public Transform parentObj;
    // Ettõl függ, hány nyul spawnol a legelején.
    public int startAmount;
    // Megadjuk a kordinátákat, amin belül spawnolnak a nyulak
    public float lowestX;
    public float highestX;
    public float lowestZ;
    public float highestZ;
    // Megadott fokokon belül véletlen irány a kezdésnél
    private float minRotation = 0f;
    private float maxRotation = 90f;

    void Start()
    {
        // 'startAmount'-szor spawnol egy nyulat a 'spawnRabbit()' függvényt meghívva
        for (int i = 0; i < startAmount; i++)
        {
            spawnRabbit();
        }
    }

    // A bekért paraméterek szerint spawnol egy fûcsomót
    void spawnRabbit()
    {
        Vector3 position = new Vector3(Random.Range(lowestX, highestX), 0.19f, Random.Range(lowestZ, highestZ));
        GameObject RabbitObj = Instantiate(Rabbit, position, Quaternion.identity, parentObj);

        float randomRotation = Random.Range(minRotation, maxRotation);
        transform.Rotate(Vector3.up, randomRotation);
        GameObject nameTagObj = Instantiate(nameTag, position, Quaternion.identity, nameTagParent);
        nameTagObj.GetComponent<AnimalStats>().target = RabbitObj.transform;
    }
}