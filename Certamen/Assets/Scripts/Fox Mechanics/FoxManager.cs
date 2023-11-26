using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FoxManager : MonoBehaviour
{
    public float timer = 0f;
    public float death = 10f;
    // Start is called before the first frame update
    void Start()
    {
        death = Random.Range(0f, 30f);
    }

    // Update is called once per frame
    void Update()
    {
        //TEST CODE//
        //Fell free to delete//
        if (timer >= death)
        {
            Destroy(gameObject);
        }
        else
        {
            timer += Time.deltaTime;
        }
    }
}
