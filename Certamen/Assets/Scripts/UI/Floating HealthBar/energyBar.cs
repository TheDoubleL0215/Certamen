using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class energyBar : MonoBehaviour
{
    [SerializeField] private Slider slider;

    public void EnergyBarUpdate(float maxEnergy, float energy)
    {
        slider.value = energy / maxEnergy;
    }

}
