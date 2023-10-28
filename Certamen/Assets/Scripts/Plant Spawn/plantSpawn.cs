using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class plantSpawn : MonoBehaviour
{
    public GameObject plantPrefab;
    public GameObject groundPrefab;
    private int loopHelper = 100;
    private float groundPositionY;
    void Start()
    {
        groundPositionY = groundPrefab.transform.position.y;

        //loopHelper = Random.Range(1, 30);
        for(int i = 0; i < loopHelper; i++)
        {
            Vector3 randomSpawnPosition = new Vector3(Random.Range(-150, 150), groundPositionY, Random.Range(-150, 150));
            Instantiate(plantPrefab, randomSpawnPosition, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
