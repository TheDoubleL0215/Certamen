using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimalStats : MonoBehaviour
{
    public Transform target; // Ny�l Transform komponense
    private rabbitManagerScript rabbitMoveScript; // Deklar�ljuk az oszt�ly szintj�n

    private void Start()
    {
        rabbitMoveScript = target.GetComponent<rabbitManagerScript>(); // Inicializ�ljuk a Start met�dusban
    }

    private void Update()
    {
        if (target != null)
        {
            transform.position = target.position;
            //string statText = "Energia: " + rabbitMoveScript.energy + "\n" +
                //"Energia Limit: " + rabbitMoveScript.energyLimit + "\n" +
               // "Energia Vesztes�g: " + rabbitMoveScript.energyLoss + "\n" +
                //"Sebess�g: " + rabbitMoveScript.forwardForce + "\n" +
               // "L�t�sug�r: " + rabbitMoveScript.radius;

            // Most �ll�tsd be a sz�veget a Text komponensen
            //GetComponent<Text>().text = statText;
            // Hozz l�tre egy forgat�si c�lpontot, amely mindig a kamera poz�ci�j�ban van
            Vector3 cameraPosition = Camera.main.transform.position;

            // Ford�tsd el az objektumot a kamera fel�
            transform.LookAt(2 * transform.position - cameraPosition);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
