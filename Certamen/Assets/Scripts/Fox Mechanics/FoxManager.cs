using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class FoxManager : MonoBehaviour
{

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
    public GameObject Fox; // ezt az objektumot fogjuk klónozni szapodrodásnál 
    public float jumpForce; // Az ugrás magassága.
    public float forwardForce; // Az ugrás hossza.
    Rigidbody rb; // RigidBody komponens.

    private bool turning = true;
    [SerializeField] private GameObject selectedRabbit;
    // Start is called before the first frame update

    void Start()
    {
        rb = GetComponent<Rigidbody>(); //rb inicializálás
        id = Random.Range(10000, 99999); // ázonosító "sorsolása"
        maturity = Random.Range(5f, maturityLimit); // lespawnolt nyulak érettsége véletlen
        age = 0f;
        if (fatherId != 0)
        {
            maturity = 0f; // ha már egy született és nem spawnolt nyúl, akkor alapból 0 az érettsége
        }
   
        StartCoroutine(JumpMovement());
    }

    // Update is called once per frame
    void Update()
    {
        maturity += Time.deltaTime;
        age += Time.deltaTime;
        // ha az érettség eléri a mehatározott szintet
        if (maturity >= maturityLimit)
        {
            if (energy >= birthEnergyLimit) // csak abban az esetben születik utód, ha van elég energiája a szülõnek
            {
                float heirEnergy = energy / fertility + 1; // "heirEnergy" értéke lesz majd az utódok energiája mikor megsszületnek
                for (int i = 0; i < fertility; i++) // "fertility" változó értékeszer meghívja a "Reproduction()" függvényt
                {
                    Reproduction(heirEnergy);
                }
                maturity = 0f; //nullázódik a maturity
                energy = energy / fertility + 1; // a szülõ energiáját elosszuk annyival, ahány utóda születik
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
                if (selectedRabbit != null)
                {
                    transform.LookAt(selectedRabbit.transform.position);
                }
            }


            Vector3 eloreMozgas = transform.forward * forwardForce;
            rb.velocity = eloreMozgas;

            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            energy -= energyLoss;
            
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
        if (selectedRabbit == null)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("Rabbit"))
                {
                    turning = false;
                    selectedRabbit = collider.gameObject;
                    //Debug.Log("Rabbit selected: " + selectedRabbit.name);
                    MoveTowardsTarget();
                    break; // Exit the loop after finding the first rabbit
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
        if (selectedRabbit != null)
        {
            if (Vector3.Distance(transform.position, selectedRabbit.transform.position) > 10f)
            {
                transform.LookAt(selectedRabbit.transform.position);
            }
            else
            {
                rabbitBehaviour rabbitScript = selectedRabbit.GetComponent<rabbitBehaviour>();
                energy += rabbitScript.energy;
                Destroy(selectedRabbit);
                selectedRabbit = null;
                turning = true;
            }
        }
        else
        {
            turning = true;
            selectedRabbit = null;
        }
    }

    void Reproduction(float heirEnergy)
    {
        GameObject newFox = Instantiate(Fox, transform.position, transform.rotation); //klónozzuk a Rabbit objektumot

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

        newFox.transform.position = newPosition;

        // Az új egyed megörökli a szülõ értékeit kisebb módosulásokkal
        FoxManager newFoxScript = newFox.GetComponent<FoxManager>();
        newFoxScript.fatherId = id;
        newFoxScript.energy = heirEnergy;
    }

    #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Handles.DrawWireArc(transform.position, Vector3.up, Vector3.forward, 360, radius);
        }
    #endif
}
