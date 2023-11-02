using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class rabbitMove : MonoBehaviour
{
    public float radius;
    public float energy;
    public float energyLimit;
    public float energyLoss;
    public float ugrasEro = 5f;
    public float eloreSebesseg = 2f;
    public float maxElteresSzog = 45f;
    public float varakozasiIdo = 2f;
    public float headingTurnSpeed = 0.3f;
    private float headingChange = 0.0f;
    private Rigidbody rb;
    public bool lehetUgrani = true;
    public bool lehetFordulni = true;
    public rabbitMove rabbitMovement;
    public GameObject seletedPlant;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        energy = Random.Range(70f, 90f);
        energyLimit = Random.Range(90f, 100f);
        energyLoss = Random.Range(5f, 20f);
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
        if (energy > energyLimit)
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
        if (lehetUgrani == true)
        {
            if (lehetFordulni)
            {
                float randomHeading = Random.Range(-90f, 90f);
                float smoothedHeading = Mathf.SmoothDampAngle(transform.eulerAngles.y, randomHeading, ref headingChange, headingTurnSpeed);
                transform.rotation = Quaternion.Euler(0, smoothedHeading * 100, 0);
            }

            Vector3 eloreMozgas = transform.forward * eloreSebesseg;
            rb.velocity = eloreMozgas;

            rb.AddForce(Vector3.up * ugrasEro, ForceMode.Impulse);
            energy -= energyLoss;
            varakozasiIdo = Random.Range(1, 4);
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
        if (seletedPlant == null)
        {
            int inoreDetectionLayerMask = ~LayerMask.GetMask("Ignore Detect");
            Collider[] colliders = Physics.OverlapSphere(transform.position, radius, inoreDetectionLayerMask);

            if (colliders.Length > 0)
            {
                seletedPlant = colliders[0].gameObject;
                MoveTowardsTarget();
            }
            else
            {
                Invoke("ReDetekt", 2);
            }
        }
        else
        {
            CheckIfNearEnough();
        }

    }

    void ReDetekt()
    {
        Detektalas();
    }

    void MoveTowardsTarget()
    {
        if (seletedPlant != null)
        {
            lehetFordulni = false;
            transform.LookAt(seletedPlant.transform.position);
            CheckIfNearEnough();

        }
        else
        {
            seletedPlant = null;
            Detektalas();
        }

    }


    void CheckIfNearEnough()
    {
        if (seletedPlant != null)
        {
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
                Invoke("MoveTowardsTarget", 1);
            }
        }
        else
        {
            seletedPlant = null;
            Detektalas();
        }
    }
}