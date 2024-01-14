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
    public Transform rabbitParentObj;

    [Header("Hunger")]

    public float hungerLevel = 100f;
    public float hungerLoss = 5f;
    public float hungerLimit = 100f;
    public float hungerMax = 150f;
    public float baseHungerMax;
    public float hungerMinimum = 40f;
    public float resourceFromGrass = 30f;
    
    [Header("Movement")]

    public float speed = 10f;
    public float baseSpeed;
    public float range = 2.5f; //radius
    public float radius = 20f;
    public float baseRadius;

    [Header("Escaping Mechanics")]
    public int detectedFoxes;
    private List<Vector3> foxPositions = new List<Vector3>();

    [Header("Scale")]
    public float scaleValue;
    public float adultScale;
    public float newbornScale;

    [Header("Other")]
    public float age; // nyúl életkora

    [Header("Teszt")]
    public bool canHaveChildren = true;
    public float timeSinceLastChildren;


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
            rabbitName = "R-" + GetRandomLetter();

            fertility = Random.Range(2, 4);
            maturityLimit = Random.Range(35f, 45f);
            maturity = Random.Range(0f, maturityLimit);

            hungerMax = Random.Range(140f, 160f);
            hungerMinimum = Random.Range(30f, 40f);
            hungerLevel = Random.Range(85f, hungerMax);
            hungerLimit = Random.Range(100f, 80f);

            speed = Random.Range(5f, 15f);
            radius = Random.Range(15f, 25f);
        }


        baseHungerMax = hungerMax;
        baseSpeed = speed;
        baseRadius = radius;

        
        //Computing hungerLost
        hungerLoss = (hungerMax/38 + radius/5 + speed/5)/2;
        //To avoid too low hungarLoss and infinite energy
        if(hungerMax/hungerLoss > maturityLimit){
            hungerLoss = (hungerMax + 5)/maturityLimit;
            //Debug.Log(hungerLoss);
        }

        //Scales
        adultScale = hungerLoss * 20;
        newbornScale = adultScale / 3;
        transform.localScale = new Vector3(newbornScale, newbornScale, newbornScale);

        agent.speed = speed;
        gameObject.name = rabbitName;

        IdleMovement();
    }

    // Update is called once per frame
    void Update()
    {
        hungerLevel -= Time.deltaTime * hungerLoss;
        
        age += Time.deltaTime;
        //TESZT
        if(canHaveChildren == false){
            if(timeSinceLastChildren >= 20f){
                canHaveChildren = true;
                timeSinceLastChildren = 0f;
            }
            else{
                timeSinceLastChildren += Time.deltaTime;
            }
        }

        if (maturity >= maturityLimit)
        {
            for (int i = 0; i < fertility; i++) // "fertility" változó értékeszer meghívja a "Reproduction()" függvényt
            {
                //TESZTELÉSHEZ
                if(canHaveChildren){
                    //maturity = 0f; //nullázódik a maturity

                    Reproduction();
                }
            }
            canHaveChildren = false;
            
        }
        else{
            //Increasing scale
            scaleValue = newbornScale + ((adultScale - newbornScale) / maturityLimit) * maturity;
            transform.localScale = new Vector3(scaleValue, scaleValue, scaleValue);
            
            //Increasing attributes based maturity
            hungerMax = baseHungerMax * (0.6f + maturity/100);
            radius = baseRadius * (0.6f + maturity/100);
            speed = baseSpeed * (0.6f + maturity/100);
            agent.speed = speed;
            //teszt miatt áthelyezve
            maturity += Time.deltaTime;
        }


        //To avoid too low hungarLoss and infinite energy
        if(hungerMax/hungerLoss > maturityLimit){
            hungerLoss = (hungerMax + 5)/maturityLimit;
            //Debug.Log(hungerLoss);
        }

        if (hungerLevel <= 0){
            Destroy(gameObject);
        }

        if(hungerLevel <= hungerLimit){
            state = State.Hunger;
        }
        else{
            state = State.Idle;
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

    void Reproduction()
    {
        GameObject newRabbit = Instantiate(Rabbit, transform.position, transform.rotation, rabbitParentObj); //klónozzuk a Rabbit objektumot

        Random.InitState(System.DateTime.Now.Millisecond);

        // Definiáld a pályaterület határait
        float minX = -75f;
        float maxX = 75f;
        float minZ = -75f;
        float maxZ = 75f;

        float distanceFactor = 5f; // Adjust this factor to determine the separation distance

        Vector3 offset = new Vector3(Random.Range(-1f, 1f), 0.0f, Random.Range(-1f, 1f)) * distanceFactor;
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

        newRabbitManager.fertility = newRabbitManager.fertility += Random.Range(-2, 2);
        if(newRabbitManager.fertility < 0){
            newRabbitManager.fertility = 0;
        }
        newRabbitManager.maturityLimit = newRabbitManager.maturityLimit += Random.Range(-2f, 2f);

        newRabbitManager.hungerLimit = newRabbitManager.hungerLimit += Random.Range(-10f, 10f);
        newRabbitManager.hungerMax = newRabbitManager.hungerMax += Random.Range(-10f, 10f);
        newRabbitManager.hungerMinimum = newRabbitManager.hungerMinimum += Random.Range(-10f, 10f);

        newRabbitManager.speed = newRabbitManager.speed += Random.Range(-5f, 5f);
        newRabbitManager.radius = newRabbitManager.radius += Random.Range(-5f, 5f);
    
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
                        if(hungerLevel > hungerMax){
                            hungerLevel = hungerMax;
                        }
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
            if(hungerMinimum < hungerLevel){
                state = State.Escape;
                EscapeMovement(detectedFoxes, foxPositions);
                detectedFoxes = 0;
                foxPositions.Clear();
            }
            else{
                state = State.Idle;
            }
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
