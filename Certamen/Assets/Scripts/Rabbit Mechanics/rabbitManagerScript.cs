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
    public float range; //radius
    public float radius = 0f;
    public NavMeshAgent agent;
    public Transform centrePoint; 
    [SerializeField] private GameObject selectedPlant;
    public float hungerLevel = 100f;
    public float hungerLoss = 5f;
    
    public int detectedFoxes;

    private List<Vector3> foxPositions = new List<Vector3>();

    public enum State{
        Idle,
        Hunger,
        Escape
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

        state = State.Idle;
        
    }

    // Update is called once per frame
    void Update()
    {
        if(hungerLevel > 150){
            hungerLevel = 150;
        }

        hungerLevel -= Time.deltaTime * hungerLoss;

        if (hungerLevel <= 0){
            Destroy(gameObject);
        }

        if (hungerLevel <= 100 && state != State.Escape)
        {
            state = State.Hunger;
        }

        else
        {
            if (hungerLevel >= 120 && state == State.Hunger)
            {
                state = State.Idle;
                print("You are a chowhound! Achievement earned!");
            }
        }


        switch (state){
            case State.Idle:
                IdleMovement();
                DetectingPredators(State.Idle);
                break;
            case State.Hunger:
                FoodMovement();
                DetectingPredators(State.Hunger);
                break;
            case State.Escape:
                DetectingPredators(State.Escape);
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
                    //int ignoreDetectionLayerMask = ~LayerMask.GetMask("Ignore Detect");
                    Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
                    //Debug.Log(colliders.Length);
                    for (int i = 0; i < colliders.Length; i++)
                    {
                        if (selectedPlant == null)
                        {
                            GameObject detectedPlant = colliders[i].gameObject;

                            if (detectedPlant.CompareTag("Grass"))
                            {
                                selectedPlant = detectedPlant;
                                //Debug.Log(selectedPlant.name);
                                break;
                            }
                        }
                    }

                    if (selectedPlant == null)
                    {
                        IdleMovement();
                    }
                }
                else
                {
                    Debug.DrawRay(selectedPlant.transform.position, Vector3.up, Color.green, 3.0f);
                    agent.SetDestination(selectedPlant.transform.position);
                    if (selectedPlant.activeSelf && Vector3.Distance(transform.position, selectedPlant.transform.position) < 5f)
                    {
                        Destroy(selectedPlant);
                        hungerLevel += 30f;
                        selectedPlant = null;
                        state = State.Idle;
                    }
                    else if (!selectedPlant.activeSelf) // If the selected rabbit doesn't exist anymore
                    {
                        selectedPlant = null;
                        state = State.Idle;
                    }
                }
            }
        }
    }

    void EscapeMovement(int foxNumber, List<Vector3> positions)
    {
        Vector3 escapeDestination = Vector3.zero;

        // Ha csak egy róka van, akkor az összes nyúl meneküljön az ellenkező irányba
        if (foxNumber == 1)
        {
            for (int i = 0; i < foxNumber; i++)
            {
                Vector3 escapeDirection = transform.position - positions[i];
                escapeDirection.Normalize();

                escapeDestination = positions[i] + (escapeDirection * 2 * radius);
                Debug.DrawRay(escapeDestination, Vector3.up, Color.blue, 5f);
                agent.SetDestination(escapeDestination);
            }
        }
        else // Ha több róka van, a nyulak a legkevesebb róka felé meneküljenek
        {
            Vector3 closestFoxPosition = Vector3.zero;
            float closestFoxDistance = Mathf.Infinity;

            // Megkeressük a legközelebbi róka pozícióját a nyúlhoz
            for (int i = 0; i < foxNumber; i++)
            {
                float distanceToFox = Vector3.Distance(transform.position, positions[i]);
                if (distanceToFox < closestFoxDistance)
                {
                    closestFoxDistance = distanceToFox;
                    closestFoxPosition = positions[i];
                }
            }

            // Az összes nyúl meneküljön a legközelebbi róka felé
            Vector3 escapeDirection = transform.position - closestFoxPosition;
            escapeDirection.Normalize();

            escapeDestination = closestFoxPosition + (escapeDirection * 2 * radius);
            Debug.DrawRay(escapeDestination, Vector3.up, Color.blue, 5f);
            agent.SetDestination(escapeDestination);
        }
    }




    void DetectingPredators(State currentState)
    {
        Collider[] objects = Physics.OverlapSphere(transform.position, radius);

        for (int i = 0; i < objects.Length; i++)
        {
            GameObject detectedObject = objects[i].gameObject;
            if (detectedObject.CompareTag("Fox"))
            {
                detectedFoxes++;
                foxPositions.Add(detectedObject.transform.position); // Store fox positions
                //Debug.Log(detectedFoxes);
            }
        }
        if (detectedFoxes > 0)
        {
            state = State.Escape;
            EscapeMovement(detectedFoxes, foxPositions);
            detectedFoxes = 0;
            foxPositions.Clear();
        }
        else
        {
            if(currentState != State.Escape){
                state = currentState;
                //Debug.Log(currentState);
            }
            else{
                state = State.Idle;
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
