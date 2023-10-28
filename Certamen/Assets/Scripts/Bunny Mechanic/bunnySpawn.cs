using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bunnySpawn : MonoBehaviour
{
    public GameObject bunnyPrefab;


    // Start is called before the first frame update
    void Start()
    {
        Instantiate(bunnyPrefab, new Vector3(0, 1, 0), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
