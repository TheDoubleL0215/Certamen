using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class FHungerBar : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Slider slider;

    public GameObject gameObjectSelf;
    public FoxManager fox;
    private float hungerLevel;


    void Awake(){
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
