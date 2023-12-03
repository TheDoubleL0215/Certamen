using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class rabbitBehaviourmechanic : MonoBehaviour
{
    Rigidbody rb;

    public float speed = 3f;

    public float jumpForce = 10f;

    public float groundDistance = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.MovePosition(new Vector3(transform.position.x, transform.position.y, transform.position.z + speed * Time.deltaTime));
        if(isGrounded()){
            rb.velocity = Vector3.up * jumpForce;
        }
    }

    bool isGrounded(){
        return Physics.Raycast(transform.position, Vector3.down, groundDistance);
    }


}
