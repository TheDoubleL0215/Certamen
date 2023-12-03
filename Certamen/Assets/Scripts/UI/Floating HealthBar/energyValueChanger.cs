using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class energyValueChanger : MonoBehaviour
{
    [SerializeField] private Text text;

    public GameObject gameObjectSelf;
    public rabbitManagerScript rabbi;
    public float hungerLevel;


    void Awake(){
        rabbi = gameObjectSelf.GetComponent<rabbitManagerScript>();
    }

    void Update(){
        SetEnergyValueOnFloater(100,rabbi.hungerLevel);
    }

    public void SetEnergyValueOnFloater(float maxEnergy, float energy)
    {
        text.text = ((int)energy) + "/" + ((int)maxEnergy);
    }

}
