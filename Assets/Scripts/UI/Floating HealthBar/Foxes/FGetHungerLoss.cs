using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class FGetHungerLoss : MonoBehaviour
{
    [SerializeField] private Text text;

    public GameObject gameObjectSelf;
    public FoxManager fox;

    void Start(){
        if(gameObjectSelf != null) {
            fox = gameObjectSelf.GetComponent<FoxManager>();
            if(fox != null) {
                HLStatSetter(fox.hungerLoss);
            } else {
                Debug.LogError("Nincs FoxManager komponens a gameObjectSelf játékobjektumban!");
            }
        } else {
            Debug.LogError("Nincs érték rendelve a gameObjectSelf-hez a szkriptben!");
        }
    }
    public void HLStatSetter(float hungerLoss)
    {
        text.text = "" + hungerLoss;
    }
}
