using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RabbitSpawnerScript : MonoBehaviour
{
    // Kiv�lasztjuk melyik karaktert spawnolja
    public GameObject Rabbit;
    // Ett�l f�gg, h�ny nyul spawnol a legelej�n.
    public int startAmount;
    // Megadjuk a kordin�t�kat, amin bel�l spawnolnak a nyulak
    public float lowestX;
    public float highestX;
    public float lowestZ;
    public float highestZ;
    // Megadott fokokon bel�l v�letlen ir�ny a kezd�sn�l
    private float minRotation = 0f;
    private float maxRotation = 90f;

    void Start()
    {
        // 'startAmount'-szor spawnol egy nyulat a 'spawnRabbit()' f�ggv�nyt megh�vva
        for (int i = 0; i < startAmount; i++)
        {
            spawnRabbit();
        }
    }

    void Update()
    {
        
    }

    // A bek�rt param�terek szerint spawnol egy f�csom�t
    void spawnRabbit()
    {
        Instantiate(Rabbit, new Vector3(Random.Range(lowestX, highestX), 0.19f, Random.Range(lowestZ, highestZ)), transform.rotation);
        float randomRotation = Random.Range(minRotation, maxRotation);
        transform.Rotate(Vector3.up, randomRotation);
    }
}
