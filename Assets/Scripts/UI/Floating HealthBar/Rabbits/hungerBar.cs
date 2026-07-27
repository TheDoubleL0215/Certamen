using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class hungerBar : MonoBehaviour
{
    [SerializeField] private Slider slider;

    public GameObject gameObjectSelf;
    public rabbitManagerScript rabbit;
    private float hungerLevel;


    void Start(){
        rabbit = gameObjectSelf.GetComponent<rabbitManagerScript>();
    }

    void Update(){
        EnergyBarUpdate(rabbit.hungerMax, rabbit.hungerLevel);
    }

    public void EnergyBarUpdate(float maxHunger, float hungerLevel)
    {
        slider.value = hungerLevel / maxHunger;
    }

}
