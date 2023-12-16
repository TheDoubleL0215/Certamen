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

    [Header("ID")]
    public int id; // egyéni azonosító
    public int fatherId; // örökölt azonosító
    //public string name;
    public string rabbitName { get; set; }

    [Header("Reproduction")]
    public int fertility = 4; // ez határozza meg, hány kölyke lehet a nyúlnak
    public float maturity = 0f; // érettség, szaporodásban van szerepe
    public float maturityLimit = 16; // ezt az értéket elérve, végbe megy a szaporodás


    [Header("Components")]

    public NavMeshAgent agent;
    public Transform centrePoint; 
    [SerializeField] private GameObject selectedPlant;
    public GameObject Rabbit;

    [Header("Hunger")]

    public float hungerLevel = 100f;
    public float hungerLoss = 5f;
    public float hungerLimit = 100f;
    public float hungerMax = 150f;
    public float satiety = 120f;
    public float resourceFromGrass = 30f;
    
    [Header("Movement")]

    public float speed = 10f;
    public float acceleration = 10f;
    public float range = 5f; //radius
    public float radius = 20f;

    [Header("Escaping Mechanics")]
    public int detectedFoxes;
    private List<Vector3> foxPositions = new List<Vector3>();

    [Header("Other")]
    public float age; // nyúl életkora
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
        state = State.Hunger;

        id = Random.Range(10000, 99999);
        age = 0f;
        maturity = 0f;

        if(fatherId == 0){
            rabbitName = "R" + GetRandomLetter();

            fertility = Random.Range(2, 4);
            maturityLimit = Random.Range(15f, 17f);
            //maturity = Random.Range(0f, maturityLimit);

            hungerLevel = Random.Range(85f, 150f);
            hungerLoss = Random.Range(10f, 15f);
            hungerLimit = Random.Range(95f, 85f);
            hungerMax = Random.Range(145f, 155f);
            satiety = Random.Range(115f, 125f);

            speed = Random.Range(8f, 12f);
            acceleration = Random.Range(8f, 12f);
            radius = Random.Range(18f, 22f);
        }

        gameObject.name = rabbitName;
    }

    // Update is called once per frame
    void Update()
    {
        hungerLevel -= Time.deltaTime * hungerLoss;
        maturity += Time.deltaTime;
        age += Time.deltaTime;

        if (maturity >= maturityLimit)
        {
            for (int i = 0; i < fertility; i++) // "fertility" változó értékeszer meghívja a "Reproduction()" függvényt
            {
                Reproduction();
            }
            maturity = 0f; //nullázódik a maturity
        }

        if(hungerLevel > hungerMax){
            hungerLevel = hungerMax;
        }

        if (hungerLevel <= 0){
            Destroy(gameObject);
        }

        if (hungerLevel <= hungerLimit && state != State.Escape)
        {
            state = State.Hunger;
        }

        else
        {
            if (hungerLevel >= satiety && state == State.Hunger)
            {
                state = State.Idle;
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

        Debug.Log(agent.speed);

    }

    void Reproduction()
    {
        GameObject newRabbit = Instantiate(Rabbit, transform.position, transform.rotation); //klónozzuk a Rabbit objektumot

        Random.InitState(System.DateTime.Now.Millisecond);

        // Definiáld a pályaterület határait
        float minX = -75f;
        float maxX = 75f;
        float minZ = -75f;
        float maxZ = 75f;

        Vector3 offset = new Vector3(Random.Range(-1f, 1f), 0.0f, Random.Range(-1f, 1f)); // Távolság a szülő nyúltól
        Vector3 newPosition = transform.position + offset;

        // Korlátozd a kis nyúl pozícióját a pálya határai között
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.z = Mathf.Clamp(newPosition.z, minZ, maxZ);

        newRabbit.transform.position = newPosition;

        // Az új egyed megörökli a szülő értékeit kisebb módosulásokkal
        rabbitManagerScript newRabbitManager = newRabbit.GetComponent<rabbitManagerScript>();

        newRabbitManager.fatherId = id;
        newRabbitManager.rabbitName = rabbitName + GetRandomLetter();
        newRabbitManager.hungerLevel = 90f;

        newRabbitManager.fertility = newRabbitManager.fertility += Random.Range(-1, 1);
        newRabbitManager.maturityLimit = newRabbitManager.maturityLimit += Random.Range(-2f, 2f);

        newRabbitManager.hungerLimit = newRabbitManager.hungerLimit += Random.Range(-5f, 5f);
        newRabbitManager.hungerLoss = newRabbitManager.hungerLoss += Random.Range(-1f, 1f);
        newRabbitManager.hungerMax = newRabbitManager.hungerMax += Random.Range(-5f, 5f);
        newRabbitManager.satiety = newRabbitManager.satiety += Random.Range(-5f, 5f);

        newRabbitManager.speed = newRabbitManager.speed += Random.Range(-2f, 2f);
        newRabbitManager.acceleration = newRabbitManager.acceleration += Random.Range(-2f, 2f);
        newRabbitManager.radius = newRabbitManager.radius += Random.Range(-2f, 2f);
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
                        hungerLevel += resourceFromGrass;
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

    char GetRandomLetter()
    {
        const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"; // All possible letters
        System.Random random = new System.Random();
        int index = random.Next(letters.Length);
        return letters[index];
    }


   #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
       Handles.DrawWireArc(transform.position, Vector3.up, Vector3.forward, 360, radius);
    }
   #endif

}
