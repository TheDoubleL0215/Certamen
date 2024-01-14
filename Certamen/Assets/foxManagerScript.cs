using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using Vector3 = UnityEngine.Vector3;
using UnityEngine.UIElements;

public class foxManagerScript : MonoBehaviour
{
    Rigidbody rb; 
    public float forwardForce = 10f; 
    public float range; //radius
    public float radius = 0f;
    
    public NavMeshAgent agent;
    public Transform centrePoint; 
    [SerializeField] private GameObject selectedRabbit;

    public float hungerLevel = 120f;
    public float hungerLoss = 5f;
    public enum State{
        Idle,
        Hunger,
    }

    [SerializeField] private State state;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        agent = GetComponent<NavMeshAgent>();
        // Define radius
        radius = Random.Range(40, 80); // érzékelõ sugara
        state = State.Idle;
        
    }

    // Update is called once per frame
    void Update()
    {

        hungerLevel -= hungerLoss / 1000;

        if(hungerLevel <= 0){
            Destroy(gameObject);
        }

        if(hungerLevel < 90){
            state = State.Hunger;
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
                    //int ignoreDetectionLayerMask = ~LayerMask.GetMask("Ignore Detect");
                    Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

                    List<Collider> rabbitColliders = new List<Collider>();

                    foreach (Collider collider in colliders)
                    {
                        Debug.Log("Találatok: ", collider.gameObject);
                        if (collider.tag == "Rabbit")
                        {
                            Debug.Log("Hozzáadott nyúl: ", collider);
                            rabbitColliders.Add(collider);
                        }else{
                            Debug.Log("Nincs ilyen");
                        }
                    }
                    if(rabbitColliders.Count != 0){
                        selectedRabbit = rabbitColliders[0].gameObject;
                        Debug.Log("Selected rabbit: ", selectedRabbit);
                        Debug.DrawRay(selectedRabbit.transform.position, Vector3.up, Color.red, 1.0f); //so you can see with gizmos
                        agent.SetDestination(selectedRabbit.transform.position);
                    }else{
                        IdleMovement();
                    }
                }
                else
                {
                    if (Vector3.Distance(transform.position, selectedRabbit.transform.position) < 5f)
                    {
                        Destroy(selectedRabbit);
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
