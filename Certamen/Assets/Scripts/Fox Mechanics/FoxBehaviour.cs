using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class FoxBehaviour : MonoBehaviour
{

    [Header("ID")]
    public int id; // egy�ni azonos�t�
    public int fatherId; // �r�k�lt azonos�t�

    [Header("Energy")]
    public float energy; // energiaszint
    public float energyLimit; //energia maximum
    public float energyLoss; // ugr�sonk�nti energia vesztes�g

    [Header("Reproduction")]
    public int fertility; // ez hat�rozza meg, h�ny k�lyke lehet a ny�lnak
    public float birthEnergyLimit; // ez a szint a minimum egy ut�dhoz
    public float maturity; // �retts�g, szaporod�sban van szerepe
    public float maturityLimit; // ezt az �rt�ket el�rve, v�gbe megy a szaporod�s

    [Header("Other")]
    public float radius; // Az �rz�kel�s�nek a r�diusza.
    public float age; // ny�l �letkora
    public float lifeTime; // ha el�ri ezt, megd�glik
    public GameObject Fox; // ezt az objektumot fogjuk kl�nozni szapodrod�sn�l 
    public float jumpForce; // Az ugr�s magass�ga.
    public float forwardForce; // Az ugr�s hossza.
    Rigidbody rb; // RigidBody komponens.

    private bool turning = true;
    [SerializeField] private GameObject selectedRabbit;
    // Start is called before the first frame update

    void Start()
    {
        rb = GetComponent<Rigidbody>(); //rb inicializ�l�s
        id = Random.Range(10000, 99999); // �zonos�t� "sorsol�sa"
        maturity = Random.Range(5f, maturityLimit); // lespawnolt nyulak �retts�ge v�letlen
        age = 0f;
        if (fatherId != 0)
        {
            maturity = 0f; // ha m�r egy sz�letett �s nem spawnolt ny�l, akkor alapb�l 0 az �retts�ge
        }
   
        StartCoroutine(JumpMovement());
    }

    // Update is called once per frame
    void Update()
    {
        maturity += Time.deltaTime;
        age += Time.deltaTime;
        // ha az �retts�g el�ri a mehat�rozott szintet
        if (maturity >= maturityLimit)
        {
            if (energy >= birthEnergyLimit) // csak abban az esetben sz�letik ut�d, ha van el�g energi�ja a sz�l�nek
            {
                float heirEnergy = energy / fertility + 1; // "heirEnergy" �rt�ke lesz majd az ut�dok energi�ja mikor megssz�letnek
                for (int i = 0; i < fertility; i++) // "fertility" v�ltoz� �rt�keszer megh�vja a "Reproduction()" f�ggv�nyt
                {
                    Reproduction(heirEnergy);
                }
                maturity = 0f; //null�z�dik a maturity
                energy = energy / fertility + 1; // a sz�l� energi�j�t elosszuk annyival, ah�ny ut�da sz�letik
            }
        }
        // el�regedett nyulak elpusztulnak
        if (age >= lifeTime)
        {
            Destroy(gameObject);
        }
    }
    private IEnumerator JumpMovement()
    {
        while (true)
        {
            Detect(); // Az �rz�kel�si folyamat megkezd�se

            if (turning)
            {
                // random fordul�s kezel�se
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
                rabbitManagerScript rabbitScript = selectedRabbit.GetComponent<rabbitManagerScript>();
                if (rabbitScript != null)
                {
                    energy += rabbitScript.hungerLevel;
                }
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
        GameObject newFox = Instantiate(Fox, transform.position, transform.rotation); //kl�nozzuk a Rabbit objektumot

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
        FoxBehaviour newFoxScript = newFox.GetComponent<FoxBehaviour>();
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