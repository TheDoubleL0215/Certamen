using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class rabbitMove : MonoBehaviour
{
    public float energy;
    public float energyLimit;
    public float energyLoss;
    public float ugrasEro = 5f; // Az ugr�s er�ss�ge
    public float eloreSebesseg = 2f; // Az el�re mozg�s sebess�ge
    public float maxElteresSzog = 45f; // A maxim�lis elt�r�si sz�g az aktu�lis ir�nyhoz k�pest
    public float varakozasiIdo = 2f; // V�rakoz�si id� az ugr�sok k�z�tt
    public float headingTurnSpeed = 0.3f; // A forgat�si sebess�g
    private float headingChange = 0.0f;
    

    private Rigidbody rb;
    public bool lehetUgrani = true;
    public bool lehetFordulni = true;

    public float radius;
    //[SerializeField] private bool headingToTarget = false;
    //Collider firstDetectedCollider = null;
    public rabbitMove rabbitMovement;
    public GameObject seletedPlant;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        energy = Random.Range(75f, 90f);
        energyLimit = Random.Range(90f, 100f);
        energyLoss = Random.Range(5f, 10f);
        radius = Random.Range(5f, 15f);
        Ugras();
        Detektalas();
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
        if (energy <= 0f)
        {
            Destroy(gameObject);
        }
        if(energy > energyLimit)
        {
            energy = energyLimit;
        }
    }
    private void OnDrawGizmos()
    {
        Handles.DrawWireArc(transform.position, Vector3.up, Vector3.forward, 360, radius);

    }

    void Ugras()
    {
        if (lehetUgrani == true) {
            if (lehetFordulni) { 
            float randomHeading = Random.Range(-90f, 90f); 
            float smoothedHeading = Mathf.SmoothDampAngle(transform.eulerAngles.y, randomHeading, ref headingChange, headingTurnSpeed);
            transform.rotation = Quaternion.Euler(0, smoothedHeading * 100, 0);
            }

            Vector3 eloreMozgas = transform.forward * eloreSebesseg;
            rb.velocity = eloreMozgas;

            // Ugr�s hozz�ad�sa
            rb.AddForce(Vector3.up * ugrasEro, ForceMode.Impulse);
            energy -= energyLoss;
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

    void Detektalas()
    {
        if(seletedPlant == null)
        {
            ////Debug.Log("Fut a Detektalas");
            int inoreDetectionLayerMask = ~LayerMask.GetMask("Ignore Detect");
            Collider[] colliders = Physics.OverlapSphere(transform.position, radius, inoreDetectionLayerMask);
            //Debug.Log("Ezeket detekt�lta: " + colliders + "ez a hossza: " + colliders.Length);

            if (colliders.Length > 0)
            {
                seletedPlant = colliders[0].gameObject;

                //Debug.Log("FirstDetected" + seletedPlant);

                //Debug.Log("Els� detekt�lt objektum: " + seletedPlant.name);

                MoveTowardsTarget();

                ////Debug.Log("Fut CheckIfNear");
                ////Debug.Log("ENNYI A T�VOLS�G:  " + Vector3.Distance(transform.position, seletedPlant.transform.position));


            } else {
                //Debug.Log("Nincs eredm�ny!");
                Invoke("ReDetekt", 2);
            }
        }
        else
        {
            CheckIfNearEnough();
        }

    }

    void MoveTowardsTarget()
    {
        //Debug.Log("Fut a MoveTowards!!!!");

        lehetFordulni = false;
        transform.LookAt(seletedPlant.transform.position);
        CheckIfNearEnough();


    }

    void ReDetekt()
    {
        Detektalas();
    }

    void CheckIfNearEnough()
    {
        //Debug.Log("Fut CheckIfNear");
        //Debug.Log("Tavolsag: " + Vector3.Distance(transform.position, seletedPlant.transform.position));
        if (Vector3.Distance(transform.position, seletedPlant.transform.position) < 3.5f)
        {
            energy += 25f;
            lehetUgrani = false;
            Destroy(seletedPlant);
            seletedPlant = null;
            lehetUgrani = true;
            lehetFordulni = true;
            Invoke("Detektalas", 1);
        }
        else
        {
            //Debug.Log("Fut CheckIfNear else ag");
            Invoke("MoveTowardsTarget", 1);
        }
    }
}
