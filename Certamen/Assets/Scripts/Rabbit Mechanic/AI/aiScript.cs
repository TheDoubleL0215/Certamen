using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class aiScript : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    public float radius = 10f;
    [SerializeField] private bool searching = true;
    public rabbitMove rabbitMovement;

    private void OnDrawGizmos()
    {
        Handles.DrawWireArc(transform.position, Vector3.up, Vector3.forward, 360, radius);
        
    }

    private void Update()
    {
        while (searching)
        { 
            Detektalas();
        }
    }


    void Detektalas()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject != gameObject && collider.gameObject.name != "World Plane")
            {
                Debug.Log("Detektált objektum: " + collider.gameObject.name);
                searching = false;
                rabbitMovement.lehetFordulni = false;
                transform.LookAt(collider.transform.position);
            }
        }
    }
}
