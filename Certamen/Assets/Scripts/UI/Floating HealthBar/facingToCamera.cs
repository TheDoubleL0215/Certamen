using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class facingToCamera : MonoBehaviour
{
     public new Camera camera;

    // Update is called once per frame
    void Update()
    {
        transform.rotation = camera.transform.rotation;
    }
}
