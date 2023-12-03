using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using Vector3 = UnityEngine.Vector3;
using System.Runtime.CompilerServices;

public class rabbitManagerScript : MonoBehaviour
{
    Rigidbody rb; 
    public float jumpForce = 5f; // Az ugrás magassága.
    public float forwardForce = 5f; 
    public float moveSpeed = 5f;
    public float range; //radius
    public float radius = 0f;
    public Vector3 newDirection;

    public float speed = 3f;

    public float groundDistance = 0.5f;
    
    public NavMeshAgent agent;
    public Transform centrePoint; 
    [SerializeField] private GameObject selectedPlant;
    public float hungerLevel = 100f;
    public float hungerLoss = 5f;
    public enum State{
        Idle,
        Hunger,
    }
    [SerializeField] public State state;
    public State CurrentState
    {
        get { return state; }
    }

    void Start()
    {

        rb = GetComponent<Rigidbody>();

        agent = GetComponent<NavMeshAgent>();

        // Define radius
        radius = Random.Range(10, 20); // érzékelõ sugara
        state = State.Idle;
        
    }

    // Update is called once per frame
    void Update()
    {
        if(hungerLevel > 100){
            hungerLevel = 100;
        }

        hungerLevel -= hungerLoss / 1000;

        if(hungerLevel <= 0){
            Destroy(gameObject);
        }

        if (hungerLevel <= 90)
        {
            state = State.Hunger;
        }
        else
        {
            state = State.Idle;
        }


        switch (state){
            case State.Idle:
                IdleMovement();
                break;
            case State.Hunger:
                FoodMovement();
                break;
        }

        
    }


    void IdleMovement(){
        if(agent.remainingDistance <= agent.stoppingDistance) //done with path
            {
                Vector3 point;
                if (RandomPoint(centrePoint.position, radius, out point)) //pass in our centre point and radius of area
                {
                    Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f); //so you can see with gizmos
                    agent.SetDestination(point);
                }
            }
    }


    void FoodMovement()
    {
        if (agent.enabled)
        {
            if (agent.remainingDistance <= agent.stoppingDistance) //done with path
            {
                if (selectedPlant == null)
                {
                    int ignoreDetectionLayerMask = ~LayerMask.GetMask("Ignore Detect");
                    Collider[] colliders = Physics.OverlapSphere(transform.position, radius, ignoreDetectionLayerMask);

                    if (colliders.Length > 0)
                    {
                        GameObject detectedPlant = colliders[0].gameObject;

                        if (detectedPlant.CompareTag("Grass"))
                        {
                            selectedPlant = detectedPlant;
                            Debug.DrawRay(selectedPlant.transform.position, Vector3.up, Color.blue, 1.0f); //so you can see with gizmos
                            agent.SetDestination(selectedPlant.transform.position);
                            //Debug.Log("Növény kiválasztva: " + selectedPlant);
                        }
                    }
                    else
                    {
                        IdleMovement();
                    }
                }
                else
                {
                    if (Vector3.Distance(transform.position, selectedPlant.transform.position) < 5f)
                    {
                        Destroy(selectedPlant);
                        hungerLevel += 30f;
                        selectedPlant = null;
                        state = State.Idle;
                    }
                }
            }
        }
    }



    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {

        Vector3 randomPoint = center + Random.insideUnitSphere * range; //random point in a sphere 
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)) //documentation: https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html
        { 
            //the 1.0f is the max distance from the random point to a point on the navmesh, might want to increase if range is big
            //or add a for loop like in the documentation
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }

    bool FoodPoint(Vector3 center, float range, out Vector3 result){

        NavMeshHit hit;
        if (selectedPlant != null && NavMesh.SamplePosition(selectedPlant.transform.position, out hit, 1.0f, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.DrawWireArc(transform.position, Vector3.up, Vector3.forward, 360, radius);
    }
    #endif

}
