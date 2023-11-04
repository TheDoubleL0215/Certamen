using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class rabbitBehaviour : MonoBehaviour
{
    //VÁLTOZÓK

    public float energy;
    public float energyLimit;
    public float energyLoss;

    Rigidbody rb; // RigidBody komponens.
    public float radius = 0f; // Az érzékelésének a rádiusza.
    public float jumpForce = 5f; // Az ugrás magassága.
    public float forwardForce = 5f; // Az ugrás hossza.
    private bool turning = true; // Ez szabélyozza a random fordulások ki- és bekapcsolását. 
    [SerializeField] private GameObject selectedPlant; // A kiválasztott növény GameObject-je.

    [SerializeField] energyBar energiaBar;

    [SerializeField] getRadius radiusGetter;


    private void Awake()
    {
        energiaBar = GetComponentInChildren<energyBar>();
        radiusGetter = GetComponentInChildren<getRadius>();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>(); //rb inicializálás

        energy = Random.Range(70f, 90f);
        energyLimit = Random.Range(90f, 100f);
        energyLoss = Random.Range(5f, 20f);
        radius = Random.Range(10, 20); // érzékelõ sugara
        energiaBar.EnergyBarUpdate(energyLimit, energy);
        radiusGetter.RadiusStatSetter(radius);
        StartCoroutine(JumpMovement()); //Cooroutine indítása

    }

    void Update()
    {
        
    }

    private IEnumerator JumpMovement()
    {
        while (true)
        {
            Detect(); // Az érzékelési folyamat megkezdése

            if (turning)
            {
                // random fordulás kezelése
                float randomHeading = Random.Range(-90f, 90f);
                transform.rotation = Quaternion.Euler(0f, randomHeading, 0f);
            }
            else
            {
                if (selectedPlant != null)
                {
                    transform.LookAt(selectedPlant.transform.position);
                }
            }


            Vector3 eloreMozgas = transform.forward * forwardForce;
            rb.velocity = eloreMozgas;

            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            energy -= energyLoss;
            energiaBar.EnergyBarUpdate(energyLimit, energy);

            if (energy <= 0f)
            {
                Destroy(gameObject);
            }
            if (energy > energyLimit)
            {
                energy = energyLimit;
            }

            float waiting = Random.Range(1.5f, 3f);

            yield return new WaitForSeconds(waiting);
        }
    }

    void Detect()
    {
        if (selectedPlant == null)
        {
            int inoreDetectionLayerMask = ~LayerMask.GetMask("Ignore Detect");
            Collider[] colliders = Physics.OverlapSphere(transform.position, radius, inoreDetectionLayerMask);

            if (colliders.Length > 0)
            {
                turning = false;
                selectedPlant = colliders[0].gameObject;
                //Debug.Log("Növény kiválasztva: " + selectedPlant);
                MoveTowardsTarget();
            }

        }
        else
        {
            MoveTowardsTarget();
        }
    }

    void MoveTowardsTarget()
    {
        if (selectedPlant != null)
        {
            if (Vector3.Distance(transform.position, selectedPlant.transform.position) > 3.5f)
            {
                transform.LookAt(selectedPlant.transform.position);
            }
            else
            {
                Destroy(selectedPlant);
                energy += 25f;
                energiaBar.EnergyBarUpdate(energyLimit, energy);
                selectedPlant = null;
                turning = true;
            }
        }
        else
        {
            turning = true;
            selectedPlant = null;
        }



    }


    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.DrawWireArc(transform.position, Vector3.up, Vector3.forward, 360, radius);
    }
    #endif

}
