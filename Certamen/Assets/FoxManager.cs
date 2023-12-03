using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using Vector3 = UnityEngine.Vector3;
using System.Runtime.CompilerServices;

public class FoxManager : MonoBehaviour
{
    Rigidbody rb; 
    public float forwardForce = 10f; 
    public float range; //radius
    public float radius = 0f;

    public Vector3 newDirection;

    public NavMeshAgent agent;
    public Transform centrePoint; 
    [SerializeField] private GameObject selectedRabbit;

    public float hungerLevel = 120f;
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
        state = State.Idle;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (hungerLevel > 100)
        {
            hungerLevel = 100;
        }

        hungerLevel -= hungerLoss / 1000;

        if(hungerLevel <= 0){
            Destroy(gameObject);
        }

        if(hungerLevel < 90){
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
                if (selectedRabbit == null)
                {
                    //Debug.Log("Nincs kiválasztott nyúl.");
                    //int ignoreDetectionLayerMask = ~LayerMask.GetMask("Ignore Detect");
                    Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

                    for (int i = 0; i < colliders.Length; i++)
                    {
                        GameObject detectedRabbit = colliders[i].gameObject;
                        //Debug.Log(detectedRabbit);

                        if (detectedRabbit.CompareTag("Rabbit"))
                        {
                            selectedRabbit = detectedRabbit;
                            //Debug.Log("Sikerült!", selectedRabbit);
                            //Debug.DrawRay(selectedRabbit.transform.position, Vector3.up, Color.red, 3.0f); //so you can see with gizmos
                            //agent.SetDestination(selectedRabbit.transform.position);
                        }
                    }
                        
                    if (selectedRabbit == null)
                    {
                        //Debug.Log("Nem érzékel nyulat");
                        IdleMovement();
                    }                   
                }
                else
                {
                    Debug.DrawRay(selectedRabbit.transform.position, Vector3.up, Color.red, 3.0f);
                    agent.SetDestination(selectedRabbit.transform.position);
                    //Debug.Log("Rajta az ügyön");
                    if (Vector3.Distance(transform.position, selectedRabbit.transform.position) < 10f)
                    {
                        Destroy(selectedRabbit);
                        Debug.Log("Hamm megettem");
                        Debug.Log("Hamm megettem");
                        hungerLevel += 30f;
                        selectedRabbit = null;
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
        if (selectedRabbit != null && NavMesh.SamplePosition(selectedRabbit.transform.position, out hit, 1.0f, NavMesh.AllAreas))
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
