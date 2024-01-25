using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FhungerBar : MonoBehaviour
{
    [SerializeField] private Slider slider;

    public GameObject gameObjectSelf;
    public FoxManager fox;
    private float hungerLevel;


    void Start(){
        fox = gameObjectSelf.GetComponent<FoxManager>();
    }

    void Update(){
        EnergyBarUpdate(fox.hungerMax, fox.hungerLevel);
    }

    public void EnergyBarUpdate(float maxHunger, float hungerLevel)
    {
        slider.value = hungerLevel / maxHunger;
    }

}
