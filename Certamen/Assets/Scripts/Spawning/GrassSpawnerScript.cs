using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassSpawnerScript : MonoBehaviour
{
    // Kiv�lasztjuk melyik karaktert spawnolja
    public GameObject Grass;
    //Parent container 
    public Transform parentObj;
    // Ett�l f�gg, h�ny f�csom� spawnol a legelej�n.
    public int startAmount;
    // H�ny m�sodpercenk�nt spawnol �j f�csom�
    public float spawnRate = 2;
    // Megadjuk a kordin�t�kat, amin bel�l spawnol a f�
    public float lowestX;
    public float highestX;
    public float lowestZ;
    public float highestZ;
    // Id� m�l�s�t m�rj�k
    private float timer = 0;
    void Start()
    {
        // 'startAmount'-szor spawnol egy f�vet a 'spawnGrass()' f�ggv�nyt megh�vva
        for (int i = 0; i < startAmount; i++)
        {
            spawnGrass();
        }
    }

    void Update()
    {
        // Csak abban esetben h�vja meg a 'spawnGrass()' f�ggv�nyt, ha a 'timer'
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

    // A bek�rt param�terek szerint spawnol egy f�csom�t
    void spawnGrass()
    {
        Instantiate(Grass, new Vector3(Random.Range(lowestX, highestX), 0, Random.Range(lowestZ, highestZ)), Quaternion.identity, parentObj.transform);
    }
}
