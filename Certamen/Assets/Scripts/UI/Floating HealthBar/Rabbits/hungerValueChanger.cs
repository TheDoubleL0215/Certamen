using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class hungerValueChanger : MonoBehaviour
{
    [SerializeField] private Text text;

    public GameObject gameObjectSelf;
    public rabbitManagerScript rabbit;


    void Start(){
        rabbit = gameObjectSelf.GetComponent<rabbitManagerScript>();
    }

    void Update(){
        SetEnergyValueOnFloater(rabbit.hungerLevel, rabbit.hungerMax);
    }

    public void SetEnergyValueOnFloater(float hungerLevel, float hungerMax)
    {
        text.text = ((int)hungerLevel) + "/" + ((int)hungerMax);
    }

}
