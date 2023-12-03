using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class getRabbitStateForFloater : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] public TextMeshProUGUI stateTexter;
    public GameObject gameObjectSelf;
    public rabbitManagerScript rabbi;
    private Enum state;


    void Awake(){
        rabbi = gameObjectSelf.GetComponent<rabbitManagerScript>();
    }

    void Update(){
        rabbitManagerScript.State CurrentState = rabbi.CurrentState;
        stateTexter.text = CurrentState.ToString();
    }



}
