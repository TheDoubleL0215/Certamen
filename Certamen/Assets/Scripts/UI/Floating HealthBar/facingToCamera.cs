using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class facingToCamera : MonoBehaviour

{
    [SerializeField] private Camera camera;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = camera.transform.rotation;
    }
}