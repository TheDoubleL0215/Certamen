using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class energyValueChanger : MonoBehaviour
{
    [SerializeField] private Text text;

    public void SetEnergyValueOnFloater(float maxEnergy, float energy)
    {
        text.text = ((int)energy) + "/" + ((int)maxEnergy);
    }

}
