using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class getSpeed : MonoBehaviour
{
    [SerializeField] private Text text;

    public GameObject gameObjectSelf;
    public rabbitManagerScript rabbit;

    void Start(){
        if(gameObjectSelf != null) {
            rabbit = gameObjectSelf.GetComponent<rabbitManagerScript>();
            if(rabbit != null) {
                SpeedStatSetter(rabbit.speed);
            } else {
                Debug.LogError("Nincs rabbitManagerScript komponens a gameObjectSelf játékobjektumban!");
            }
        } else {
            Debug.LogError("Nincs érték rendelve a gameObjectSelf-hez a szkriptben!");
        }
    }

    public void SpeedStatSetter(float speed)
    {
        text.text = "" + speed;
    }

}