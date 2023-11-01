using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
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

    public float radius = 10f;
    //[SerializeField] private bool headingToTarget = false;
    //Collider firstDetectedCollider = null;
    public rabbitMove rabbitMovement;
    public GameObject seletedPlant;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
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

    }
    private void OnDrawGizmos()
    {
        Handles.DrawWireArc(transform.position, Vector3.up, Vector3.forward, 360, radius);

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

    void Detektalas()
    {
        if(seletedPlant == null)
        {
            ////Debug.Log("Fut a Detektalas");
            int inoreDetectionLayerMask = ~LayerMask.GetMask("Ignore Detect");
            Collider[] colliders = Physics.OverlapSphere(transform.position, radius, inoreDetectionLayerMask);
            //Debug.Log("Ezeket detektálta: " + colliders + "ez a hossza: " + colliders.Length);

            if (colliders.Length > 0)
            {
                seletedPlant = colliders[0].gameObject;

                //Debug.Log("FirstDetected" + seletedPlant);

                //Debug.Log("Elsõ detektált objektum: " + seletedPlant.name);

                MoveTowardsTarget();

                ////Debug.Log("Fut CheckIfNear");
                ////Debug.Log("ENNYI A TÁVOLSÁG:  " + Vector3.Distance(transform.position, seletedPlant.transform.position));


            } else {
                //Debug.Log("Nincs eredmény!");
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
