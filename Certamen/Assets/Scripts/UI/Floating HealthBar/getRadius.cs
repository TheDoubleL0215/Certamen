using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class getRadius : MonoBehaviour
{
    [SerializeField] private Text text;

    public void RadiusStatSetter(float radius)
    {
        text.text = "" + radius;
    }

}