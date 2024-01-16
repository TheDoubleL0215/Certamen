using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovementScript : MonoBehaviour
{
    //-------------------
    // V�LTOZ�K
    //-------------------

    public float novekedesSebesseg = 20f; // a magass�g v�ltoz�s�nak sebess�ge
    public float smoothTime = 1.5f; // a "sprintel�s" �s "gyalogl�s" k�zti �tmenet m�rt�ke
    private float currentVelocity = 0.0f; // reference v�ltoz� a C#-nak
    private float maxFlyHeight = 30f; // maximum magass�g
    private float currentSpeed; // sebess�g v�ltoz�
    private float smoothSpeed; // sim�tott gyorsul�s
    public CharacterController controller; // karakter ir�ny�t�ja
    public float speed = 20f; // "gyalogl�s" sebess�g


    //-------------------
    // INICIALIZ�CI�
    //-------------------
    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }


    void Update()
    {
        //-------------------
        // MOZG�S
        //-------------------

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Az objektum helyzete �s elfordul�sa alapj�n sz�moljuk az ir�nyt
        Vector3 move = transform.TransformDirection(new Vector3(x, 0, z));

        controller.Move(move * speed * Time.deltaTime);


        //-------------------
        // REP�L�S
        //-------------------

        if (Input.GetKey(KeyCode.Space)) //FELFEL�
        {
            // Jelenlegi poz�ci�
            Vector3 currentPosition = controller.transform.position;
            // �j poz�ci� felfel� mozg�s eset�n
            Vector3 newPosition = currentPosition + Vector3.up * novekedesSebesseg * Time.deltaTime;
            // Az �j poz�ci� Y koordin�t�j�t korl�tozzuk 30 �rt�kre
            newPosition.y = Mathf.Clamp(newPosition.y, 1f, maxFlyHeight);
            // Mozg�s a korl�tozott poz�ci�ra
            controller.Move(newPosition - currentPosition);

        }

        if (Input.GetKey(KeyCode.LeftControl)) //LEFEL�
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
            // A Mathf.SmoothDamp-al megadhatjuk hogy egy v�ltoz� �rt�ke mennyi id? alatt v�ltozzon meg, ami �gy
            // sim�bb mozg�st eredm�nyez
            currentSpeed = 80;
            smoothSpeed = Mathf.SmoothDamp(speed, currentSpeed, ref currentVelocity, smoothTime);
        }
        else
        {
            currentSpeed = 20;
            smoothSpeed = Mathf.SmoothDamp(speed, currentSpeed, ref currentVelocity, smoothTime);
        }
        // a sebess�g a sim�tott gyorsul�s lesz
        speed = smoothSpeed;
    }
}
