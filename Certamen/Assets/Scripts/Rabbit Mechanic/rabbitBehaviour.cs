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
    //VÁLTOZÓK//
    [Header("ID")]
    public int id; // egyéni azonosító
    public int fatherId; // örökölt azonosító
    [Header("Energy")]
    public float energy; // energiaszint
    public float energyLimit; //energia maximum
    public float energyLoss; // ugrásonkénti energia veszteség
    [Header("Reproduction")]
    public int fertility; // ez határozza meg, hány kölyke lehet a nyúlnak
    public float birthEnergyLimit; // ez a szint a minimum egy utódhoz
    public float maturity; // érettség, szaporodásban van szerepe
    public float maturityLimit; // ezt az értéket elérve, végbe megy a szaporodás
    [Header("Other")]
    public float radius; // Az érzékelésének a rádiusza.
    public float age; // nyúl életkora
    public float lifeTime; // ha eléri ezt, megdöglik
    public GameObject Rabbit; // ezt az objektumot fogjuk klónozni szapodrodásnál 
    public float jumpForce; // Az ugrás magassága.
    public float forwardForce; // Az ugrás hossza.
    Rigidbody rb; // RigidBody komponens.


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
        maturity = Random.Range(0f, maturityLimit); // lespawnolt nyulak érettsége véletlen
        age = 0f;
        if (fatherId != 0)
        {
            maturity = 0f; // ha már egy született és nem spawnolt nyúl, akkor alapból 0 az érettsége
        }
        energiaBar.EnergyBarUpdate(energyLimit, energy);
        changeEnergyValue.SetEnergyValueOnFloater(energyLimit, energy);
        radiusGetter.RadiusStatSetter(radius);
        StartCoroutine(JumpMovement()); //Cooroutine indítása
    }

    void Update()
    {
        // folyamatosan növeljük az érettséget és a kort
        maturity += Time.deltaTime;
        age += Time.deltaTime;
        // ha az érettség eléri a mehatározott szintet
        if (maturity >= maturityLimit)
        {
            if (energy >= birthEnergyLimit) // csak abban az esetben születik utód, ha van elég energiája a szülőnek
            {
                float heirEnergy = energy / fertility + 1; // "heirEnergy" értéke lesz majd az utódok energiája mikor megsszületnek
                for (int i = 0; i < fertility; i++) // "fertility" változó értékeszer meghívja a "Reproduction()" függvényt
                {
                    Reproduction(heirEnergy);
                }
                maturity = 0f; //nullázódik a maturity
                energy = energy / fertility + 1; // a szülő energiáját elosszuk annyival, ahány utóda születik
            }
        }
        // elöregedett nyulak elpusztulnak
        if (age >= lifeTime)
        {
            Destroy(gameObject);
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
            if (energy >= energyLimit)
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
            int ignoreDetectionLayerMask = ~LayerMask.GetMask("Ignore Detect");
            Collider[] colliders = Physics.OverlapSphere(transform.position, radius, ignoreDetectionLayerMask);

            if (colliders.Length > 0)
            {
                GameObject detectedPlant = colliders[0].gameObject;

                if (detectedPlant.CompareTag("Grass"))
                {
                    turning = false;
                    selectedPlant = detectedPlant;
                    MoveTowardsTarget();
                }
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
                energy += 50f;
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
        GameObject newRabbit = Instantiate(Rabbit, transform.position, transform.rotation); //klónozzuk a Rabbit objektumot

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
        rabbitBehaviour newRabbitScript = newRabbit.GetComponent<rabbitBehaviour>();
        newRabbitScript.fatherId = id;
        newRabbitScript.energy = heirEnergy;
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.DrawWireArc(transform.position, Vector3.up, Vector3.forward, 360, radius);
    }
#endif

}
