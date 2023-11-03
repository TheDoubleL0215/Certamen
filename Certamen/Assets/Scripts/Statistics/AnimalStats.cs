using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimalStats : MonoBehaviour
{
    public Transform target; // Nyúl Transform komponense
    private rabbitBehaviour rabbitMoveScript; // Deklaráljuk az osztály szintjén

    private void Start()
    {
        rabbitMoveScript = target.GetComponent<rabbitBehaviour>(); // Inicializáljuk a Start metódusban
    }

    private void Update()
    {
        if (target != null)
        {
            transform.position = target.position;
            string statText = "Energia: " + rabbitMoveScript.energy + "\n" +
                "Energia Limit: " + rabbitMoveScript.energyLimit + "\n" +
                "Energia Veszteség: " + rabbitMoveScript.energyLoss + "\n" +
                "Sebesség: " + rabbitMoveScript.forwardForce + "\n" +
                "Látósugár: " + rabbitMoveScript.radius;

            // Most állítsd be a szöveget a Text komponensen
            GetComponent<Text>().text = statText;
            // Hozz létre egy forgatási célpontot, amely mindig a kamera pozíciójában van
            Vector3 cameraPosition = Camera.main.transform.position;

            // Fordítsd el az objektumot a kamera felé
            transform.LookAt(2 * transform.position - cameraPosition);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
