using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AttrDropDown : MonoBehaviour
{
    void HandleInputData(int val)
    {
        if(val == 0){
            print("speed");
        }
        if(val == 1){
            print("hungerLimit");
        }
        if(val == 2 ){
            print("radius");
        }
    }
}
