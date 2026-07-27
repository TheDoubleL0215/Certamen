using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GetFoxStatus : MonoBehaviour
{
   [SerializeField] public TextMeshProUGUI stateTexter;
    public GameObject gameObjectSelf;
    public FoxManager fox;
    private Enum state;


    void Awake(){
        fox = gameObjectSelf.GetComponent<FoxManager>();
    }

    void Update(){
        FoxManager.State CurrentState = fox.CurrentState;
        stateTexter.text = CurrentState.ToString();
    }
}
