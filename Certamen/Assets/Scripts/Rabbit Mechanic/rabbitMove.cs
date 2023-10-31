using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class rabbitMove : MonoBehaviour
{
    public float ugrasEro = 5f; // Az ugr�s er�ss�ge
    public float eloreSebesseg = 2f; // Az el�re mozg�s sebess�ge
    public float maxElteresSzog = 45f; // A maxim�lis elt�r�si sz�g az aktu�lis ir�nyhoz k�pest
    public float varakozasiIdo = 2f; // V�rakoz�si id� az ugr�sok k�z�tt
    public float headingTurnSpeed = 0.3f; // A forgat�si sebess�g
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

            // Ugr�s hozz�ad�sa
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
