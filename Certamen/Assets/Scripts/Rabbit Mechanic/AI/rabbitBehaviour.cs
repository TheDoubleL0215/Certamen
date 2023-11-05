using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class rabbitBehaviour : MonoBehaviour
{
    //VÁLTOZÓK
    public int id; // egyéni azonosító
    public int fatherId = 0; // örökölt azonosító
    public float energy; // energiaszint
    public float energyLimit; //energia maximum
    public float energyLoss; // ugrásonkénti energia veszteség
    public float radius = 0f; // Az érzékelésének a rádiusza.
    public float maturity = 0f; // érettség, szaporodásban van szerepe
    public float maturityLimit; // ezt az értéket elérve, végbe megy a szaporodás
    public int fertility; // ez határozza meg, hány kölyke lehet a nyúlnak
    public float birthEnergyLimit; // ez a szint a minimum egy utódhoz

    public GameObject Rabbit; // ezt az objektumot fogjuk klónozno szapodrodásnál
    
    Rigidbody rb; // RigidBody komponens.
    public float jumpForce = 5f; // Az ugrás magassága.
    public float forwardForce; // Az ugrás hossza.


    private bool turning = true; // Ez szabélyozza a random fordulások ki- és bekapcsolását. 
    [SerializeField] private GameObject selectedPlant; // A kiválasztott növény GameObject-je.


    [Header("Energy Bar")]
    //floater energy kijelzése
    [SerializeField] energyBar energiaBar;


    [Header("Energy Value")]
    //floater energyValue kijelzése
    public GameObject energyValueObject;
    [SerializeField] private energyValueChanger changeEnergyValue;

    [Header("Radius Value")]
    //floater rádiusz kijelzése
    public GameObject radiusValueObject;
    [SerializeField] private getRadius radiusGetter;



    private void Awake()
    {
        energiaBar = GetComponentInChildren<energyBar>();
        radiusGetter = radiusValueObject.GetComponent<getRadius>();
        changeEnergyValue = energyValueObject.GetComponent<energyValueChanger>();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>(); //rb inicializálás

        id = Random.Range(10000, 99999); // ázonosító "sorsolása"
        if (fatherId == 0)
        {
            //ÉRTÉKADÁS
            forwardForce = Random.Range(4f, 5f);
            energyLimit = Random.Range(90f, 100f);
            energyLoss = Random.Range(15f, 20f);
            energy = Random.Range(energyLimit - 25f, energyLimit);
            radius = Random.Range(15f, 25f);
            maturityLimit = Random.Range(20, 25);
            fertility = Random.Range(2, 4);
            birthEnergyLimit = Random.Range(75f, energyLimit);
        }
        maturity = 0; //alapból 0 az érettség
        energiaBar.EnergyBarUpdate(energyLimit, energy);
        changeEnergyValue.SetEnergyValueOnFloater(energyLimit, energy);
        radiusGetter.RadiusStatSetter(radius);
        StartCoroutine(JumpMovement()); //Cooroutine indítása
    }

    void Update()
    {
        // folyamatosan növeljük az érettséget
        maturity += Time.deltaTime;
        // ha az érettség eléri a mehatározott szintet
        if (maturity >= maturityLimit)
        {
            if(energy >= birthEnergyLimit) // csak abban az esetben születik utód, ha van elég energiája a szülõnek
            {
                float heirEnergy = energy / fertility; // "heirEnergy" értéke lesz majd az utódok energiája mikor megsszületnek
                for (int i = 0; i < fertility; i++) // "fertility" változó értékeszer meghívja a "Reproduction()" függvényt
                {
                    Reproduction(heirEnergy);
                }
                maturity = 0f; //nullázódik a maturity
                energy = energy / fertility; // a szülõ energiáját elosszuk annyival, ahány utóda születik
            }
        }
    }

    private IEnumerator JumpMovement()
    {
        while (true)
        {
            Detect(); // Az érzékelési folyamat megkezdése

            if (turning)
            {
                // random fordulás kezelése
                float randomHeading = Random.Range(-90f, 90f);
                transform.rotation = Quaternion.Euler(0f, randomHeading, 0f);
            }
            else
            {
                if (selectedPlant != null)
                {
                    transform.LookAt(selectedPlant.transform.position);
                }
            }


            Vector3 eloreMozgas = transform.forward * forwardForce;
            rb.velocity = eloreMozgas;

            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            energy -= energyLoss;
            energiaBar.EnergyBarUpdate(energyLimit, energy);
            changeEnergyValue.SetEnergyValueOnFloater(energyLimit, energy);

            if (energy <= 0f)
            {
                Destroy(gameObject);

            }
            if (energy > energyLimit)
            {
                energy = energyLimit;
            }

            float waiting = Random.Range(1.5f, 3f);

            yield return new WaitForSeconds(waiting);
        }
    }

    void Detect()
    {
        if (selectedPlant == null)
        {
            int inoreDetectionLayerMask = ~LayerMask.GetMask("Ignore Detect");
            Collider[] colliders = Physics.OverlapSphere(transform.position, radius, inoreDetectionLayerMask);

            if (colliders.Length > 0)
            {
                turning = false;
                selectedPlant = colliders[0].gameObject;
                //Debug.Log("Növény kiválasztva: " + selectedPlant);
                MoveTowardsTarget();
            }

        }
        else
        {
            MoveTowardsTarget();
        }
    }

    void MoveTowardsTarget()
    {
        if (selectedPlant != null)
        {
            if (Vector3.Distance(transform.position, selectedPlant.transform.position) > 3.5f)
            {
                transform.LookAt(selectedPlant.transform.position);
            }
            else
            {
                Destroy(selectedPlant);
                energy += Random.Range(50f, 75f);
                energiaBar.EnergyBarUpdate(energyLimit, energy);
                changeEnergyValue.SetEnergyValueOnFloater(energyLimit, energy);
                selectedPlant = null;
                turning = true;
            }
        }
        else
        {
            turning = true;
            selectedPlant = null;
        }



    }

    //ÚJ EGYED SZÜLETÉSE
    void Reproduction(float heirEnergy)
    {
        print(heirEnergy);
        GameObject newRabbit = Instantiate(Rabbit, transform.position, transform.rotation); //klónozzuk a Rabbit objektumot

        // Definiáld a pályaterület határait
        float minX = -75f;
        float maxX = 75f;
        float minZ = -75f;
        float maxZ = 75f;

        Vector3 offset = new Vector3(Random.Range(-1f, 1f), 0.0f, Random.Range(-1f, 1f)); // Távolság a szülõ nyúltól
        Vector3 newPosition = transform.position + offset;

        // Korlátozd a kis nyúl pozícióját a pálya határai között
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.z = Mathf.Clamp(newPosition.z, minZ, maxZ);

        newRabbit.transform.position = newPosition;

        // Az új egyed megörökli a szülõ értékeit kisebb módosulásokkal
        rabbitBehaviour newRabbitScript = newRabbit.GetComponent<rabbitBehaviour>();
        newRabbitScript.fatherId = id;
        newRabbitScript.energy = heirEnergy;
        newRabbitScript.energyLoss = Random.Range(energyLoss-2f, energyLoss+2f);
        newRabbitScript.energyLimit = Random.Range(energyLimit - 5f, energyLimit + 5f);
        newRabbitScript.maturityLimit = Random.Range(maturityLimit - 5f, maturityLimit + 5f);
        newRabbitScript.fertility = Random.Range(fertility - 1, fertility + 1);
        newRabbitScript.birthEnergyLimit = Random.Range(birthEnergyLimit - 5f, birthEnergyLimit + 5f);
        newRabbitScript.radius = Random.Range(radius - 1f, radius + 1f);
        newRabbitScript.forwardForce = Random.Range(forwardForce - 1f, forwardForce + 1f);
    }


    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.DrawWireArc(transform.position, Vector3.up, Vector3.forward, 360, radius);
    }
    #endif

}
