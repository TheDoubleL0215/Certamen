using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class getRadius : MonoBehaviour
{
    [SerializeField] private Text text;

    public GameObject gameObjectSelf;
    public rabbitManagerScript rabbit;

    void Start(){
        if(gameObjectSelf != null) {
            rabbit = gameObjectSelf.GetComponent<rabbitManagerScript>();
            if(rabbit != null) {
                RadiusStatSetter(rabbit.radius);
            } else {
                Debug.LogError("Nincs rabbitManagerScript komponens a gameObjectSelf játékobjektumban!");
            }
        } else {
            Debug.LogError("Nincs érték rendelve a gameObjectSelf-hez a getRadius szkriptben!");
        }
    }

    public void RadiusStatSetter(float radius)
    {
        text.text = "" + radius;
    }

}