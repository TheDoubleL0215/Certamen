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
    public List<string> genderList  = new List<string>{"male", "female"};
    public string gender;
    public float pregnancyTime;
    public bool isPregnant = false;
    private float elapsedTime = 0f;
    public float matingCooldown = 0f;

    [Header("Components")]
    public Transform rabbitParentObject;
    public NavMeshAgent agent;
    public Transform centrePoint; 
    [SerializeField] private GameObject selectedPlant;
    public GameObject selectedRabbit;
    public GameObject Rabbit;
    private rabbitManagerScript mateScript;

    [Header("Hunger")]

    public float hungerLevel = 100f;
    public float hungerLoss = 5f;
    public float hungerLimit = 100f;
    public float baseHungerLimit;
    public float hungerMax = 150f;
    public float baseHungerMax;
    public float resourceFromGrass = 30f;
    public int foodExpectation = 15;
    public bool wantToExplore = false;

    [Header("Movement")]

    public float speed = 10f;
    public float baseSpeed;
    public float range = 5f; //radius
    public float radius = 20f;
    public float baseRadius;

    [Header("Escaping Mechanics")]
    public int detectedFoxes;
    public bool detectedPredators = false;
    private List<Vector3> foxPositions = new List<Vector3>();

    [Header("Scale")]
    public float scaleValue;
    public float adultScale;
    public float newbornScale;

    [Header("Other")]
    public float age; // nyúl életkora
    public enum State
    {
        Idle,
        Hunger,
        Escape,
        Reproduction
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
            rabbitName = "R - " + GetRandomLetter();

            fertility = Random.Range(3, 5);
            maturityLimit = Random.Range(20f, 25f);
            maturity = Random.Range(19f, maturityLimit);

            hungerMax = Random.Range(145f, 155f);
            hungerLevel = Random.Range(120f, hungerMax);
            foodExpectation = Random.Range(12, 16);

            speed = Random.Range(8f, 12f);
            radius = Random.Range(18f, 22f);
            
            pregnancyTime = Random.Range(5f, 10f);
            gender = genderList[Random.Range(0, genderList.Count)];
        }

        if (gender == "female")
        {
            GetComponent<Renderer>().material.color = Color.white; // Set female color
        }
        else
        {
            GetComponent<Renderer>().material.color = Color.grey; // Set male color
        }


        // These will come handy at "ontogeny"
        baseHungerMax = hungerMax;
        baseRadius = radius;
        baseSpeed = speed;
        baseHungerLimit = hungerLimit;

        //Computing hungerLoss based on attributes
        hungerLoss = (hungerMax/30 + radius/8 + speed/6)/2;

        //To avoid too low hungarLoss and infinite energy
        if(hungerMax/hungerLoss > maturityLimit){
            maturityLimit = hungerMax/hungerLoss + 1f;
        }

        //Scales
        adultScale = hungerLoss * 20f;
        newbornScale = adultScale / 3;
        transform.localScale = new Vector3(newbornScale, newbornScale, newbornScale);

        gameObject.name = rabbitName;
    }

    // Update is called once per frame
    void Update()
    {
        //To avoid too low hungarLoss and infinite energy
        if(hungerMax/hungerLoss > maturityLimit){
            hungerLoss = (hungerMax + 5)/maturityLimit;
        }

        hungerLimit = hungerMax * 0.7f;
        hungerLevel -= Time.deltaTime * hungerLoss;
        age += Time.deltaTime;

        if(matingCooldown > 0){
            matingCooldown -= Time.deltaTime;
        }


        if(state != State.Reproduction){
            if(hungerLevel <= hungerLimit){
                state = State.Hunger;
            }
            else{
                state = State.Idle;
            }
        }
        else{
            if (selectedRabbit == null){
                state = State.Idle;
            }
        }

        if (hungerLevel <= 0){
            Destroy(gameObject);
        }


        if (maturity  >= maturityLimit)
        {
            if(matingCooldown <= 0){
                if (selectedRabbit == null)
                {
                    if (hungerLevel > hungerLimit * 0.5f){
                        DetectMate();
                    }
                }
                else{
                    state = State.Reproduction;
                }
            }
        }
        else{
            //Inscreasing scale
            scaleValue = newbornScale + ((adultScale - newbornScale) / maturityLimit) * maturity;
            transform.localScale = new Vector3(scaleValue, scaleValue, scaleValue);
            //Increasing attributes based on maturity
            hungerMax = baseHungerMax * (0.8f + maturity/100);
            radius = baseRadius * (0.8f + maturity/100);
            speed = baseSpeed * (0.8f + maturity/100);
            hungerLimit = baseHungerLimit * (0.8f + maturity/100);
            agent.speed = speed;
            
            maturity += Time.deltaTime;
        }

        if (isPregnant == true)
        {
            if (elapsedTime >= pregnancyTime)
            {   
                for (int i = 0; i < fertility; i++) // "fertility" változó értékeszer meghívja a "Reproduction()" függvényt
                {
                    isPregnant = false;
                    Reproduction();
                }
                isPregnant = false;
                elapsedTime = 0f;
                matingCooldown = 15f;
                state = State.Idle;
            }
            else{
                elapsedTime += Time.deltaTime;
            }
        }
        
        if(detectedPredators == true)
        {
            state = State.Escape;
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
            case State.Reproduction:
                ReproductionMovement();
                DetectingPredators(State.Reproduction);
                break;
        }

    }
    void DetectMate()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        for (int i = 0; i < colliders.Length; i++)
        {
            GameObject detectedRabbit = colliders[i].gameObject;
            
            if (detectedRabbit.CompareTag("Rabbit"))
            {
                rabbitManagerScript detRabbitScript = detectedRabbit.GetComponent<rabbitManagerScript>();
                
                if(detRabbitScript.maturity >= detRabbitScript.maturityLimit 
                && detRabbitScript.state != State.Reproduction && detRabbitScript.matingCooldown <= 0f 
                && detRabbitScript.hungerLevel > detRabbitScript.hungerLimit * 0.5f)
                {
                    if(detRabbitScript.gender != gender)
                    {
                        selectedRabbit = detectedRabbit;
                        detRabbitScript.selectedRabbit = gameObject;
                        state = State.Reproduction;
                    }
            
                }
            }
            detectedRabbit = null;
        }
    }

    void ReproductionMovement()
    {
        if (agent.enabled && selectedRabbit != null && selectedRabbit.activeSelf)
        {
            agent.SetDestination(selectedRabbit.transform.position);
            Debug.DrawRay(selectedRabbit.transform.position, Vector3.up, Color.green, 5.0f);
            float distanceToRabbit = Vector3.Distance(transform.position, selectedRabbit.transform.position);

            if (distanceToRabbit < 10f)
            {
                if (gender == "female")
                {
                    mateScript = selectedRabbit.GetComponent<rabbitManagerScript>();
                    isPregnant = true;
                }
                else
                {
                    matingCooldown = 20;
                }

                state = State.Hunger;
                selectedRabbit = null;
            }
        }
        else
        {
            // If the selected rabbit doesn't exist anymore or is not active, reset the state
            selectedRabbit = null;
            state = State.Idle;
        }
    }


    void Reproduction()
    {
        GameObject newRabbit = Instantiate(Rabbit, transform.position, transform.rotation, rabbitParentObject); 
        Random.InitState(System.DateTime.Now.Millisecond);



        rabbitManagerScript newRabbitManager = newRabbit.GetComponent<rabbitManagerScript>();
        
        // Randomly choose between father and mother
        rabbitManagerScript sourceParent = Random.Range(0f, 1f) < 0.5f ? mateScript : this;

        newRabbitManager.gender = genderList[Random.Range(0, genderList.Count)];
        if(newRabbitManager.gender == "male"){
            newRabbitManager.rabbitName = mateScript.rabbitName + GetRandomLetter();
        }
        else{
            newRabbitManager.rabbitName = rabbitName + GetRandomLetter();
        }
        newRabbitManager.fatherId = mateScript.id;


        // Apply mutations to the selected attributes
        newRabbitManager.fertility = sourceParent.fertility + Random.Range(-1, 1);
        newRabbitManager.maturityLimit = sourceParent.maturityLimit + Random.Range(-3f, 3f);
        newRabbitManager.pregnancyTime = sourceParent.pregnancyTime + Random.Range(-2f, 2f);

        newRabbitManager.hungerMax = sourceParent.hungerMax + Random.Range(-6f, 6f) - 3 + (pregnancyTime / 7 * 3);
        newRabbitManager.foodExpectation = sourceParent.foodExpectation + Random.Range(-3, 3);
        
        newRabbitManager.hungerLevel = hungerMax;
        newRabbitManager.speed = sourceParent.speed + Random.Range(-4f, 4f) - 2 + (pregnancyTime / 7 * 2);
        newRabbitManager.radius = sourceParent.radius + Random.Range(-2f, 2f) - 2 + (pregnancyTime / 7 * 2);
        newRabbitManager.state = State.Idle;
    }


   void IdleMovement()
{
    if (agent.remainingDistance <= agent.stoppingDistance) // done with path
    {
        Vector3 point;
        if (RandomPoint(centrePoint.position, radius, out point)) // pass in our centre point and radius of area
        {

            Vector3 direction = (point - centrePoint.position).normalized;
            float distance = 10f;
            if (wantToExplore == true){
                distance = scaleValue * 0.5f; // Set minDistance and maxDistance as needed
                wantToExplore = false;
            }
            Vector3 newPosition = centrePoint.position + direction * distance;

            Debug.DrawRay(newPosition, Vector3.up, Color.black, 1.0f); // so you can see with gizmos
            agent.SetDestination(newPosition);
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
                    int grassNum = 0;
                    Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
                    for (int i = 0; i < colliders.Length; i++)
                    {
                        GameObject detectedPlant = colliders[i].gameObject;

                        if (detectedPlant.CompareTag("Grass"))
                        {
                            grassNum += 1;
                        }
                    }
                    if(grassNum < foodExpectation){
                        wantToExplore = true;
                    }
                    for (int i = 0; i < colliders.Length; i++)
                    {
                        if (selectedPlant == null)
                        {
                            GameObject detectedPlant = colliders[i].gameObject;

                            if (detectedPlant.CompareTag("Grass"))
                            {
                                selectedPlant = detectedPlant;
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
                    Debug.DrawRay(selectedPlant.transform.position, Vector3.up, Color.black, 2.0f);
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
            }
        }
        if (detectedFoxes > 0)
        {
            state = State.Escape;
            detectedPredators = true;
            EscapeMovement(detectedFoxes, foxPositions);
            detectedFoxes = 0;
            foxPositions.Clear();
        }
        else
        {
            if(currentState != State.Escape){
                state = currentState;
            }
            else{
                state = State.Idle;
                detectedPredators = false;
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
   void OnDrawGizmos()
    {
       Handles.DrawWireArc(transform.position, Vector3.up, Vector3.forward, 360, radius);
    }
   #endif
}