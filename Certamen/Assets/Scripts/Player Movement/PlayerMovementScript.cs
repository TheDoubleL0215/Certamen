using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovementScript : MonoBehaviour
{
    //-------------------
    // VÁLTOZÓK
    //-------------------

    public float novekedesSebesseg = 20f; // a magasság változásának sebessége
    public float smoothTime = 1.5f; // a "sprintelés" és "gyaloglás" közti átmenet mértéke
    private float currentVelocity = 0.0f; // reference változó a C#-nak
    private float maxFlyHeight = 90f; // maximum magasság
    private float currentSpeed; // sebesség változó
    private float smoothSpeed; // simított gyorsulás
    public CharacterController controller; // karakter irányítója
    public float speed = 20f; // "gyaloglás" sebesség


    //-------------------
    // INICIALIZÁCIÓ
    //-------------------
    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }


    void Update()
    {
        //-------------------
        // MOZGÁS
        //-------------------

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Az objektum helyzete és elfordulása alapján számoljuk az irányt
        Vector3 move = transform.TransformDirection(new Vector3(x, 0, z));

        controller.Move(move * speed * Time.deltaTime);


        //-------------------
        // REPÜLÉS
        //-------------------

        if (Input.GetKey(KeyCode.Space)) //FELFELÉ
        {
            // Jelenlegi pozíció
            Vector3 currentPosition = controller.transform.position;
            // új pozíció felfelé mozgás esetén
            Vector3 newPosition = currentPosition + Vector3.up * novekedesSebesseg * Time.deltaTime;
            // Az új pozíció Y koordinátáját korlátozzuk 30 értékre
            newPosition.y = Mathf.Clamp(newPosition.y, 1f, maxFlyHeight);
            // Mozgás a korlátozott pozícióra
            controller.Move(newPosition - currentPosition);

        }

        if (Input.GetKey(KeyCode.LeftControl)) //LEFELÉ
        {
            Vector3 currentPosition = controller.transform.position;
            Vector3 newPosition = currentPosition - Vector3.up * novekedesSebesseg * Time.deltaTime;
            newPosition.y = Mathf.Clamp(newPosition.y, 1f, maxFlyHeight);
            controller.Move(newPosition - currentPosition);

        }

        //-------------------
        // SPRINT
        //-------------------

        if (Input.GetKey(KeyCode.LeftShift))
        {
            // A Mathf.SmoothDamp-al megadhatjuk hogy egy változó értéke mennyi idő alatt változzon meg, ami úgy
            // simább mozgást eredményez
            currentSpeed = 80;
            smoothSpeed = Mathf.SmoothDamp(speed, currentSpeed, ref currentVelocity, smoothTime);
        }
        else
        {
            currentSpeed = 20;
            smoothSpeed = Mathf.SmoothDamp(speed, currentSpeed, ref currentVelocity, smoothTime);
        }
        // a sebesség a simított gyorsulás lesz
        speed = smoothSpeed;
    }
}


