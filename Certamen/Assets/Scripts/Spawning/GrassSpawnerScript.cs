using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassSpawnerScript : MonoBehaviour
{
    // Kiválasztjuk melyik karaktert spawnolja
    public GameObject Grass;
    //Parent container 
    public Transform parentObj;
    // Ettõl függ, hány fûcsomó spawnol a legelején.
    public int startAmount;
    // Hány másodpercenként spawnol új fûcsomó
    public float spawnRate = 2;
    // Megadjuk a kordinátákat, amin belül spawnol a fû
    public float lowestX;
    public float highestX;
    public float lowestZ;
    public float highestZ;
    // Idõ múlását mérjük
    private float timer = 0;
    void Start()
    {
        // 'startAmount'-szor spawnol egy füvet a 'spawnGrass()' függvényt meghívva
        for (int i = 0; i < startAmount; i++)
        {
            spawnGrass();
        }
    }

    void Update()
    {
        // Csak abban esetben hívja meg a 'spawnGrass()' függvényt, ha a 'timer'
        // megegyezik a 'spawnRate'-ben meadottal.
        if (timer < spawnRate)
        {
            timer = timer + Time.deltaTime;
        }
        else
        {
            spawnGrass();
            timer = 0;
        }
    }

    // A bekért paraméterek szerint spawnol egy fûcsomót
    void spawnGrass()
    {
        Instantiate(Grass, new Vector3(Random.Range(lowestX, highestX), 0, Random.Range(lowestZ, highestZ)), Quaternion.identity, parentObj.transform);
    }
}
