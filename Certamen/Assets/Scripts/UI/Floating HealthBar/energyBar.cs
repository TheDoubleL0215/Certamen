using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class energyBar : MonoBehaviour
{
    [SerializeField] private Slider slider;

    public GameObject gameObjectSelf;
    public rabbitManagerScript rabbi;
    private float hungerLevel;


    void Awake(){
        rabbi = gameObjectSelf.GetComponent<rabbitManagerScript>();
    }

    void Update(){
        EnergyBarUpdate(100, rabbi.hungerLevel);
    }

    public void EnergyBarUpdate(float maxHunger, float hungerLevel)
    {
        slider.value = hungerLevel / maxHunger;
    }

}
