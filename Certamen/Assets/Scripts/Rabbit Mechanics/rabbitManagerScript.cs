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

    //TULAJDONSÁGOK//
    [Header("ID")]
    public int id; // egyéni azonosító
    public int fatherId; // örökölt azonosító
    public string rabbitName { get; set; }

    [Header("Reproduction")]
    public int fertility; // ez határozza meg, hány kölyke lehet a nyúlnak
    public float maturity; // érettség, szaporodásban van szerepe
    public float maturityLimit; // ivarérettségi szint
    public List<string> genderList  = new List<string>{"male", "female"};
    public string gender; // állat neme
    public float pregnancyTime; // vemhesség időtartama
    public bool isPregnant = false;
    private float elapsedTime;
    public float matingCooldown;

    [Header("Components")]
    public Transform rabbitParentObject;
    public NavMeshAgent agent;
    public Transform centrePoint; 
    [SerializeField] private GameObject selectedPlant;
    public GameObject selectedRabbit;
    public GameObject Rabbit;
    private rabbitManagerScript mateScript;

    [Header("Hunger")]

    public float hungerLevel; // jelenlegi éhségi szint
    public float hungerLoss; // másodpercenkénti éhezés
    public float baseHungerLoss;
    public float hungerLimit; // jóllakottsági érték
    public float baseHungerLimit;
    public float hungerMax; // maximális éhségi szint
    public float baseHungerMax;
    public float resourceFromGrass;
    public int foodExpectation;
    public bool wantToExplore;

    [Header("Movement")]

    public float speed; // sebesség
    public float baseSpeed;
    public float radius; // látótávolság
    public float baseRadius;

    [Header("Escaping Mechanics")]
    public int detectedFoxes;
    public bool detectedPredators;
    private List<Vector3> foxPositions = new List<Vector3>();

    [Header("Scale")] // méretezés
    public float scaleValue;
    public float adultScale;
    public float newbornScale;

    [Header("Other")]
    public float age; // nyúl életkora
    private float dyingAge;
    private bool isDead = false;
    public enum State // fázisok
    {
        Idle,
        Hunger,
        Escape,
        Reproduction,
        Death
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

        // Egyedi tulajdonságok kezdéskor
        id = Random.Range(10000, 99999);
        age = 0f;
        maturity = 0f;

        if(fatherId == 0){
            rabbitName = "R - " + GetRandomLetter();

            fertility = Random.Range(3, 5);
            maturityLimit = Random.Range(40f, 45f);
            maturity = Random.Range(3f, maturityLimit);

            hungerMax = Random.Range(110f, 120f);
            hungerLimit = Random.Range(90f, hungerMax);
            hungerLevel = Random.Range(100f, hungerMax);
            foodExpectation = Random.Range(12, 16);

            speed = Random.Range(7f, 11f);
            radius = Random.Range(15f, 20f);
            
            pregnancyTime = Random.Range(5f, 10f);
            gender = genderList[Random.Range(0, genderList.Count)];
        }

        if (gender == "female")
        {
            gameObject.GetComponent<Renderer>().material.color = new Color32(246, 246, 246, 1);;
        }
        else
        {
            gameObject.GetComponent<Renderer>().material.color = new Color32(100, 100, 100, 1);
        }


        
        baseHungerMax = hungerMax;
        baseRadius = radius;
        baseSpeed = speed;
        baseHungerLimit = hungerLimit;

        //hungerLoss kiszámolása
        hungerLoss = (hungerMax/30 + radius/8 + speed/6)/2;
        baseHungerLoss = hungerLoss;

        if(baseHungerMax/(hungerLoss * 0.8f) > maturityLimit){
            maturityLimit = baseHungerMax/(hungerLoss * 0.8f) + 1f;
        }

        //Scales
        adultScale = hungerLoss * 20f;
        newbornScale = adultScale / 3;
        transform.localScale = new Vector3(newbornScale, newbornScale, newbornScale);

        gameObject.name = rabbitName;
    }

    void Update()
    {
        if(baseHungerMax/(hungerLoss * 0.8f) > maturityLimit){
            maturityLimit = baseHungerMax/(hungerLoss * 0.8f) + 1f;
        }
        hungerLevel -= Time.deltaTime * hungerLoss;
        age += Time.deltaTime;

        if(matingCooldown > 0){
            matingCooldown -= Time.deltaTime;
        }

        // Viselkedési fázis meghatározása
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
        // Állat halála
        if (hungerLevel <= 0 && isDead == true){
            state = State.Death;
            Death();
        }
        if (hungerLevel <= 0 && isDead != true){
           dyingAge = age;
           isDead = true;
           state = State.Death;
           agent.SetDestination(gameObject.transform.position);
           speed = 0;
           agent.speed = 0;
           agent.acceleration = 0;
           radius = 0;
           if (selectedPlant != null){
                selectedPlant.tag = "Grass";
           }
           gameObject.tag = "DeadRabbit";
           gameObject.GetComponent<Renderer>().material.color = new Color32(20, 110, 35, 1);
           Death();
        }
        

        // Szaporodással kapcsolatos ellenőrzések
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
            // Méret növelése
            scaleValue = newbornScale + ((adultScale - newbornScale) / maturityLimit) * maturity;
            transform.localScale = new Vector3(scaleValue, scaleValue, scaleValue);
            // tulajdonságok növelése az ivarérettség függvényében
            hungerMax = baseHungerMax * (0.7875f + maturity/200);
            radius = baseRadius * (0.7875f + maturity/200);
            speed = baseSpeed * (0.7875f + maturity/200);
            hungerLimit = baseHungerLimit * (0.7875f + maturity/200);
            agent.speed = speed;
            
            maturity += Time.deltaTime;
        }

        if (isPregnant == true && isDead == false)
        {
            if (elapsedTime >= pregnancyTime)
            {   
                for (int i = 0; i < fertility; i++) // "fertility" változó értékeszer meghívja a "Reproduction()" függvényt, kis nyulak születése
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

        // Fázisok
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
            case State.Death:
                Death();
                break;
        }

    }

    // Ha a látóterén belül található olyan nyúl, amely alkalmas pár lehet, kiválasztja magának
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
                && detRabbitScript.hungerLevel > detRabbitScript.hungerLimit * 0.5f
                && detRabbitScript.state != State.Death)
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

    // Ha van kiválasztott pár, akkor elkezd felé közelíteni és ha bizonyos távolságon belül vannak egymástól, megtermékenyül a nőstény
    void ReproductionMovement()
    {
        hungerLoss = baseHungerLoss;
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
            selectedRabbit = null;
            state = State.Idle;
        }
    }

    // Itt születik meg az új egyed és kapja meg kezdeti értékeit
    void Reproduction()
    {
        GameObject newRabbit = Instantiate(Rabbit, transform.position, transform.rotation, rabbitParentObject); 
        Random.InitState(System.DateTime.Now.Millisecond);

        rabbitManagerScript newRabbitManager = newRabbit.GetComponent<rabbitManagerScript>();
        
        rabbitManagerScript sourceParent = Random.Range(0f, 1f) < 0.5f ? mateScript : this;

        newRabbitManager.gender = genderList[Random.Range(0, genderList.Count)];
        if(newRabbitManager.gender == "male"){
            newRabbitManager.rabbitName = mateScript.rabbitName + GetRandomLetter();
        }
        else{
            newRabbitManager.rabbitName = rabbitName + GetRandomLetter();
        }
        newRabbitManager.fatherId = mateScript.id;


        // Mutációk
        newRabbitManager.fertility = sourceParent.fertility + Random.Range(-1, 1);
        newRabbitManager.maturityLimit = sourceParent.maturityLimit + Random.Range(-5f, 5f);
        newRabbitManager.pregnancyTime = sourceParent.pregnancyTime + Random.Range(-3f, 3f);

        newRabbitManager.hungerMax = sourceParent.hungerMax + Random.Range(-6f, 6f) - 3 + (pregnancyTime / 7.5f * 3);
        newRabbitManager.hungerLimit = sourceParent.hungerLimit + Random.Range(-6f, 6f);
        newRabbitManager.foodExpectation = sourceParent.foodExpectation + Random.Range(-3, 3);
        
        newRabbitManager.hungerLevel = hungerMax;
        newRabbitManager.speed = sourceParent.speed + Random.Range(-4f, 4f) - 2 + (pregnancyTime / 7.5f * 2);
        newRabbitManager.radius = sourceParent.radius + Random.Range(-2f, 2f) - 2 + (pregnancyTime / 7.5f * 2);
        newRabbitManager.state = State.Idle;
    }

    // Véletlenszerűen lézeng ilyenkor az állat
   void IdleMovement()
    {
        hungerLoss = baseHungerLoss * 0.8f;
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            Vector3 point;
            if (RandomPoint(centrePoint.position, radius, out point))
            {

                Vector3 direction = (point - centrePoint.position).normalized;
                float distance = 10f;
                if (wantToExplore == true){
                    distance = scaleValue * 0.5f;
                    wantToExplore = false;
                }
                Vector3 newPosition = centrePoint.position + direction * distance;

                Debug.DrawRay(newPosition, Vector3.up, Color.black, 1.0f);
                agent.SetDestination(newPosition);
            }
        }
    }


    // Fűcsomók után kutat az állat és ha talált egyet a látóterületén belül, akkor elkezd felé haladni, majd megeszi
    void FoodMovement()
    {
        hungerLoss = baseHungerLoss;
        if (agent.enabled)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (selectedPlant == null)
                {
                    List<GameObject> detectedGrass = new List<GameObject>();
                    Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
                    for (int i = 0; i < colliders.Length; i++)
                    {
                        GameObject detectedPlant = colliders[i].gameObject;

                        if (detectedPlant.CompareTag("Grass"))
                        {
                            detectedGrass.Add(detectedPlant);;
                        }
                    }
                    if(detectedGrass.Count < foodExpectation){
                        wantToExplore = true;
                    }
                    if (detectedGrass.Count > 0)
                    {
                        selectedPlant = detectedGrass[0];
                        selectedPlant.tag = "Untagged";
                    }
                    else{
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
                    else if (!selectedPlant.activeSelf)
                    {
                        selectedPlant = null;
                        state = State.Idle;
                    }
                }
            }
        }
    }

    // Menekül az észlelt rókák elől
    void EscapeMovement(int foxNumber, List<Vector3> positions)
    {
        hungerLoss = baseHungerLoss;
        Vector3 escapeDestination = Vector3.zero;

        // Ha csak egy róka van, a nyúl meneküljön az ellenkező irányba
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
        else
        {
            Vector3 closestFoxPosition = Vector3.zero;
            float closestFoxDistance = Mathf.Infinity;
            
            for (int i = 0; i < foxNumber; i++)
            {
                float distanceToFox = Vector3.Distance(transform.position, positions[i]);
                if (distanceToFox < closestFoxDistance)
                {
                    closestFoxDistance = distanceToFox;
                    closestFoxPosition = positions[i];
                }
            }

            Vector3 escapeDirection = transform.position - closestFoxPosition;
            escapeDirection.Normalize();

            escapeDestination = closestFoxPosition + (escapeDirection * 2 * radius);
            Debug.DrawRay(escapeDestination, Vector3.up, Color.blue, 5f);
            agent.SetDestination(escapeDestination);
        }
    }

    // Ez a fázis a menekülés kivételével minden fázisban meg van hívva és ellenőrzi, hogy van-e róka látótávon belül. Ha van akkor menkülés fázisba kapcsol
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

    // Ennek a segítségével generálunk véletlenszerű pozíciókat, ha szükséges
    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {

        Vector3 randomPoint = center + Random.insideUnitSphere * range; //random point in a sphere 
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)) //documentation: https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html
        { 
            
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }

    void Death(){
        if(age >= dyingAge + 10f){
            Destroy(gameObject);
        }
    }
    
    // Véletlen betű generátor, az állatok elnevezésénél használjuk
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