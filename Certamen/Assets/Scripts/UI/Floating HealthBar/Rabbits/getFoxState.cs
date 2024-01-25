using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class getFoxState : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] public TextMeshProUGUI stateTexter;
    public GameObject gameObjectSelf;
    public FoxManager fox;
    private Enum state;


    void Start(){
        fox = gameObjectSelf.GetComponent<FoxManager>();
    }

    void Update(){
        FoxManager.State CurrentState = fox.CurrentState;
        stateTexter.text = CurrentState.ToString();
    }
}
