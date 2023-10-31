using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class rabbitMove : MonoBehaviour
{
    public float ugrasEro = 5f; // Az ugrás erõssége
    public float eloreSebesseg = 2f; // Az elõre mozgás sebessége
    public float maxElteresSzog = 45f; // A maximális eltérési szög az aktuális irányhoz képest
    public float varakozasiIdo = 2f; // Várakozási idõ az ugrások között
    public float headingTurnSpeed = 0.3f; // A forgatási sebesség
    private float headingChange = 0.0f;

    private Rigidbody rb;
    public bool lehetUgrani = true;
    public bool lehetFordulni = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Ugras();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.P))
        {
            lehetUgrani = false;
        }
        if (Input.GetKey(KeyCode.L))
        {
            lehetUgrani = true;
        }
    }

    void Ugras()
    {
        if (lehetUgrani == true) {
            if (lehetFordulni) { 
            float randomHeading = Random.Range(-360f, 360f);
            float smoothedHeading = Mathf.SmoothDampAngle(transform.eulerAngles.y, randomHeading, ref headingChange, headingTurnSpeed);
            transform.rotation = Quaternion.Euler(0, smoothedHeading * 100, 0);
            }

            Vector3 eloreMozgas = transform.forward * eloreSebesseg;
            rb.velocity = eloreMozgas;

            // Ugrás hozzáadása
            rb.AddForce(Vector3.up * ugrasEro, ForceMode.Impulse);
            varakozasiIdo = Random.Range(1,4);
        }
        Invoke("UjraUgras", varakozasiIdo);
    }

    void UjraUgras()
    {
            Ugras();     
    }

    public void FordulasLetiltas()
    {
        lehetFordulni = false;
    }


}
