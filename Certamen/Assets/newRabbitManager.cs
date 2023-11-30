using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class newRabbitManager : MonoBehaviour
{
    Rigidbody rb;

    public float speed = 5f;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
    }

    // Update is called once per frame
    void Update()
    {
        rb.MovePosition(transform.position + transform.forward * Time.deltaTime * speed);
    }
}
