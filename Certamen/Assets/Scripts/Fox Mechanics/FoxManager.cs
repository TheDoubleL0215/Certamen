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

    [Header("ID")]
    public int id; // egyéni azonosító
    public int fatherId = 0; // örökölt azonosító
    public string foxName { get; set; }

    [Header("Reproduction")]
    public int fertility = 4; // ez határozza meg, hány kölyke lehet a nyúlnak
    public float maturity = 0f; // érettség, szaporodásban van szerepe
    public float maturityLimit = 16; // ezt az értéket elérve, végbe megy a szaporodás
    public List<string> genderList  = new List<string>{"male", "female"};
    public string gender;
    public float pregnancyTime;
    public bool isPregnant = false;
    private float elapsedTime = 0.0f;
    public float matingCooldown = 0f;

    [Header("Components")]

    public NavMeshAgent agent;
    public Transform centrePoint; 
    [SerializeField] private GameObject selectedRabbit;
    private GameObject selectedFox;
    public GameObject Fox;
    private FoxManager mateScript;

    [Header("Hunger")]

    public float hungerLevel = 75f;
    public float hungerLoss = 5f;
    public float hungerLimit = 75f;
    public float hungerMax = 100f;
    public float baseHungerMax;
    public float rabbitHungerLimit = 50f;
    //public float criticalpercent = 0.2f;

    [Header("Movement")]
    public float speed = 15f;
    public float baseSpeed;
    public float range = 2.5f;
    public float radius = 20f;
    public float baseRadius;

    [Header("Scale")]
    public float scaleValue;
    public float adultScale;
    public float newbornScale;

    [Header("Other")]
    public float age; // nyúl életkora
    public enum State{
        Idle,
        Scout,
        Chase,
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
        // Define radius
        state = State.Scout;

        id = Random.Range(10000, 99999);
        age = 0f;
        maturity = 0f;

        if(fatherId == 0){
            foxName = "F - " + GetRandomLetter();

            fertility = Random.Range(2, 4);
            maturityLimit = Random.Range(20f, 25f);
            maturity = Random.Range(0f, maturityLimit);

            hungerMax = Random.Range(90f, 110f);
            hungerLevel = Random.Range(70f, hungerMax);
            hungerLimit = Random.Range(70f, 80f);
            rabbitHungerLimit = Random.Range(40f, 60f);

            speed = Random.Range(15f, 20f);
            radius = Random.Range(30f, 40f);
        
            pregnancyTime = Random.Range(5f, 10f);
            gender = genderList[Random.Range(0, genderList.Count)];
        }

        // These will come handy at "ontogeny"
        baseHungerMax = hungerMax;
        baseRadius = radius;
        baseSpeed = speed;

        //Computing hungerLoss based on attributes
        hungerLoss = (hungerMax/25 + radius/10 + speed/8)/2;

        //To avoid too low hungarLoss and infinite energy
        if(hungerMax/hungerLoss > maturityLimit){
            maturityLimit = hungerMax/hungerLoss + 1f;
        }

        //Scales
        adultScale = hungerLoss * 0.9f;
        newbornScale = adultScale / 3;
        transform.localScale = new Vector3(newbornScale, newbornScale, newbornScale);
        

        gender = genderList[Random.Range(0, genderList.Count)];

        gameObject.name = foxName;
    }

    // Update is called once per frame
    void Update()
    {
        //To avoid too low hungarLoss and infinite energy
        if(hungerMax/hungerLoss > maturityLimit){
            hungerLoss = (hungerMax + 5)/maturityLimit;
        }

        hungerLevel -= Time.deltaTime * hungerLoss;
        age += Time.deltaTime;

        if(matingCooldown > 0){
            matingCooldown -= Time.deltaTime;
        }

        if(state != State.Reproduction){
            if(hungerLevel <= hungerLimit){
                if(selectedRabbit == null){
                    state = State.Scout;
                }
                else{
                    state = State.Chase;
                }
            }
            else{
                state = State.Idle;
            }
        }
        else{
            if (selectedFox == null){
                state = State.Idle;
            }
        }

        if (hungerLevel <= 0){
            Destroy(gameObject);
        }

        if (maturity  >= maturityLimit)
        {
            if(matingCooldown <= 0){
                if (selectedFox == null)
                {
                    DetectMate();
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
                matingCooldown = 15f;
                state = State.Scout;
            }
            else{
                elapsedTime += Time.deltaTime;
            }
        }

        switch (state){
            case State.Idle:
                IdleMovement();
                break;
            case State.Scout:
                Scouting();
                break;
            case State.Chase:
                Chasing();
                break;
            case State.Reproduction:
                ReproductionMovement();
                break;
        }
        
    }
    void DetectMate()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        for (int i = 0; i < colliders.Length; i++)
        {
            GameObject detectedFox = colliders[i].gameObject;
            
            if (detectedFox.CompareTag("Fox"))
            {
                FoxManager detFoxScript = detectedFox.GetComponent<FoxManager>();

                if(detFoxScript.maturity >= detFoxScript.maturityLimit && detFoxScript.state != State.Reproduction && detFoxScript.matingCooldown <= 0f)
                {
                    if(detFoxScript.gender != gender)
                    {
                        selectedFox = detectedFox;
                        detFoxScript.selectedFox = gameObject;
                        state = State.Reproduction;
                    }
            
                }
            }
            detectedFox = null;
        }
    }
    void ReproductionMovement()
    {
        if (agent.enabled)
        {
            if (agent.remainingDistance <= agent.stoppingDistance) 
            {
                agent.SetDestination(selectedFox.transform.position);
                Debug.DrawRay(selectedFox.transform.position, Vector3.up, Color.green, 5.0f);
                float distanceToFox = Vector3.Distance(transform.position, selectedFox.transform.position);

                if (distanceToFox < 10f)
                {
                    if (gender == "female")
                    {
                        mateScript = selectedFox.GetComponent<FoxManager>();
                        isPregnant = true;
                    }
                    else
                    {
                        matingCooldown = 20;
                    }

                    state = State.Scout;
                    selectedRabbit = null;
                }
            }
            else{
                selectedFox = null;
                state = State.Scout;
            }                
        }
    }
    void Reproduction()
    {            
        GameObject newFox = Instantiate(Fox, transform.position, transform.rotation); //klónozzuk a Rabbit objektumot

        Random.InitState(System.DateTime.Now.Millisecond);

        // Definiáld a pályaterület határait
        float minX = -75f;
        float maxX = 75f;
        float minZ = -75f;
        float maxZ = 75f;

        Vector3 offset = new Vector3(Random.Range(-1f, 1f), 0.0f, Random.Range(-1f, 1f)); // Távolság a szülő rókától
        Vector3 newPosition = transform.position + offset;

        // Korlátozd a kis róka pozícióját a pálya határai között
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.z = Mathf.Clamp(newPosition.z, minZ, maxZ);

        newFox.transform.position = newPosition;

        // Az új egyed megörökli a szülő értékeit kisebb módosulásokkal
        FoxManager newFoxManager = newFox.GetComponent<FoxManager>();

        FoxManager sourceParent = Random.Range(0f, 1f) < 0.5f ? mateScript : this;

        newFoxManager.gender = genderList[Random.Range(0, genderList.Count)];
        if(newFoxManager.gender == "male"){
            newFoxManager.foxName = mateScript.foxName + GetRandomLetter();
        }
        else{
            newFoxManager.foxName = foxName + GetRandomLetter();
        }
        newFoxManager.fatherId = mateScript.id;
        newFoxManager.hungerLevel = 80f;

        newFoxManager.fertility = sourceParent.fertility + Random.Range(-1, 1);
        newFoxManager.maturityLimit = sourceParent.maturityLimit + Random.Range(-3f, 3f);
        newFoxManager.pregnancyTime = sourceParent.pregnancyTime + Random.Range(-2f, 2f);

        newFoxManager.hungerLimit = sourceParent.hungerLimit + Random.Range(-5f, 5f);
        newFoxManager.hungerMax = sourceParent.hungerMax + Random.Range(-6f, 6f) - 3 + (pregnancyTime / 7 * 3);

        newFoxManager.speed = sourceParent.speed + Random.Range(-4f, 4f) - 2 + (pregnancyTime / 7 * 2);
        newFoxManager.radius = sourceParent.radius + Random.Range(-2f, 2f) - 2 + (pregnancyTime / 7 * 2);
        
    }

    void IdleMovement(){
        if(agent.remainingDistance <= agent.stoppingDistance) //done with path
            {
                Vector3 point;
                if (RandomPoint(centrePoint.position, radius, out point)) //pass in our centre point and radius of area
                {
                    Debug.DrawRay(point, Vector3.up, Color.black, 1.0f); //so you can see with gizmos
                    agent.SetDestination(point);
                }
            }
    }
    void Scouting()
    {
        if (agent.enabled)
        {
            if (agent.remainingDistance <= agent.stoppingDistance) //done with path
            {
                //Debug.Log("Nincs kiválasztott nyúl.");
                //int ignoreDetectionLayerMask = ~LayerMask.GetMask("Ignore Detect");
                Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

                for (int i = 0; i < colliders.Length; i++)
                {
                    if (selectedRabbit == null)
                    {
                        GameObject detectedRabbit = colliders[i].gameObject;
                        //Debug.Log(detectedRabbit);

                        if (detectedRabbit.CompareTag("Rabbit"))
                        {
                            rabbitManagerScript detRabbitScript = detectedRabbit.GetComponent<rabbitManagerScript>();
                            if(detRabbitScript.hungerLevel > rabbitHungerLimit){
                                selectedRabbit = detectedRabbit;
                                break;
                            }
                        }
                    }
                }
                        
                if (selectedRabbit == null)
                {
                    //Debug.Log("Nem érzékel nyulat");
                    IdleMovement();
                }                   
            }
           
        }
    }

    void Chasing()
    {
        if(selectedRabbit != null){
            Debug.DrawRay(selectedRabbit.transform.position, Vector3.up, Color.red, 3.0f);
            agent.SetDestination(selectedRabbit.transform.position);
        }
        if (selectedRabbit.activeSelf && Vector3.Distance(transform.position, selectedRabbit.transform.position) < 5f)
        {
            rabbitManagerScript rabbitScript = selectedRabbit.GetComponent<rabbitManagerScript>();
            hungerLevel += rabbitScript.hungerLevel;
            if (hungerLevel > hungerMax)
            {
                hungerLevel = hungerMax;
            }
            Destroy(selectedRabbit);
            selectedRabbit = null;
            //state = State.Idle;
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
