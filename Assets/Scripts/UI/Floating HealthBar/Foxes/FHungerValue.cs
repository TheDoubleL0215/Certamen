using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FHungerValue : MonoBehaviour
{
   [SerializeField] private Text text;

    public GameObject gameObjectSelf;
    public FoxManager fox;


    void Awake(){
        fox = gameObjectSelf.GetComponent<FoxManager>();
    }

    void Update(){
        SetEnergyValueOnFloater(fox.hungerLevel, fox.hungerMax);
    }

    public void SetEnergyValueOnFloater(float hungerLevel, float hungerMax)
    {
        text.text = ((int)hungerLevel) + "/" + ((int)hungerMax);
    }
}
