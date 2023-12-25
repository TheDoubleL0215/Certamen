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

    [Header("Components")]

    public NavMeshAgent agent;
    public Transform centrePoint; 
    [SerializeField] private GameObject selectedRabbit;
    public GameObject Fox;
    public Transform foxParentObj;

    [Header("Hunger")]

    public float hungerLevel = 75f;
    public float hungerLoss = 5f;
    public float hungerLimit = 75f;
    public float hungerMax = 100f;
    public float satiety = 80f;
    public float rabbitHungerLimit = 50f;

    [Header("Movement")]
    public float speed = 15f;
    public float range = 2.5f;
    public float radius = 20f;

    [Header("Other")]
    public float age; // nyúl életkora
    public enum State{
        Idle,
        Scout,
        Chase,
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

        id = Random.Range(10000, 99999);
        age = 0f;
        maturity = 0f;


        if(fatherId == 0){
            foxName = "F-" + GetRandomLetter();

            fertility = Random.Range(1, 2);
            maturityLimit = Random.Range(19f, 21f);
            maturity = Random.Range(0f, maturityLimit);

            hungerMax = Random.Range(90f, 110f);
            hungerLevel = Random.Range(70f, hungerMax);
            hungerLimit = Random.Range(70f, 80f);
            satiety = Random.Range(90f, 80f);
            rabbitHungerLimit = Random.Range(40f, 60f);

            speed = Random.Range(15f, 20f);
            radius = Random.Range(30f, 40f);
        }
        
        hungerLoss = (hungerMax/25 + radius/10 + speed/8)/2;
        
        agent.speed = speed;
        gameObject.name = foxName;

        IdleMovement();
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

        if (hungerLevel > hungerMax){
            hungerLevel = hungerMax;
        }

        if (hungerLevel <= 0){
            Destroy(gameObject);
        }

        if(selectedRabbit == null && hungerLevel < hungerLimit){
            state = State.Scout;
        }

        if (selectedRabbit != null && hungerLevel < hungerLimit)
        {
            state = State.Chase;
        }

        if (hungerLevel >= satiety)
        {
            state = State.Idle;
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
        }
        
    }

    void Reproduction()
    {
        GameObject newFox = Instantiate(Fox, transform.position, transform.rotation, foxParentObj); //kl�nozzuk a Rabbit objektumot

        // Defini�ld a p�lyater�let hat�rait
        float minX = -75f;
        float maxX = 75f;
        float minZ = -75f;
        float maxZ = 75f;

        Vector3 offset = new Vector3(Random.Range(-1f, 1f), 0.0f, Random.Range(-1f, 1f)); // T�vols�g a sz�l� ny�lt�l
        Vector3 newPosition = transform.position + offset;

        // Korl�tozd a kis ny�l poz�ci�j�t a p�lya hat�rai k�z�tt
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.z = Mathf.Clamp(newPosition.z, minZ, maxZ);

        newFox.transform.position = newPosition;

        // Az �j egyed meg�r�kli a sz�l� �rt�keit kisebb m�dosul�sokkal
        FoxManager newFoxManager = newFox.GetComponent<FoxManager>();
        newFoxManager.fatherId = id;
        newFoxManager.foxName = foxName + GetRandomLetter();
        newFoxManager.hungerLevel = 80f;

        newFoxManager.fertility = newFoxManager.fertility += Random.Range(-1, 1);
        newFoxManager.maturityLimit = newFoxManager.maturityLimit += Random.Range(-2f, 2f);

        newFoxManager.hungerLimit = newFoxManager.hungerLimit += Random.Range(-7f, 7f);
        newFoxManager.hungerMax = newFoxManager.hungerMax += Random.Range(-7f, 7f);
        newFoxManager.satiety = newFoxManager.satiety += Random.Range(-7f, 7f);
        newFoxManager.rabbitHungerLimit = newFoxManager.rabbitHungerLimit += Random.Range(-7f, 7f);

        newFoxManager.speed = newFoxManager.speed += Random.Range(-5f, 5f);
        newFoxManager.radius = newFoxManager.radius += Random.Range(-5f, 5f);
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
                    IdleMovement();
                }                   
            }
           
        }
    }

    void Chasing()
    {
        Debug.DrawRay(selectedRabbit.transform.position, Vector3.up, Color.red, 3.0f);
        agent.SetDestination(selectedRabbit.transform.position);
        if (selectedRabbit.activeSelf && Vector3.Distance(transform.position, selectedRabbit.transform.position) < 10f)
        {
            rabbitManagerScript rabbitScript = selectedRabbit.GetComponent<rabbitManagerScript>();
            hungerLevel += rabbitScript.hungerLevel;
            if (hungerLevel > hungerMax)
            {
                hungerLevel = hungerMax;
            }
            Debug.Log("megette");
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
