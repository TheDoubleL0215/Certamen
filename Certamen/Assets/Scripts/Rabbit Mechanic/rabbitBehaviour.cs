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
    //V�LTOZ�K//
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
    public GameObject Rabbit; // ezt az objektumot fogjuk kl�nozni szapodrod�sn�l 
    public float jumpForce; // Az ugr�s magass�ga.
    public float forwardForce; // Az ugr�s hossza.
    Rigidbody rb; // RigidBody komponens.


    private bool turning = true; // Ez szab�lyozza a random fordul�sok ki- �s bekapcsol�s�t. 
    [SerializeField] private GameObject selectedPlant; // A kiv�lasztott n�v�ny GameObject-je.


    [Header("Energy Bar")]
    //floater energy kijelz�se
    [SerializeField] energyBar energiaBar;


    [Header("Energy Value")]
    //floater energyValue kijelz�se
    public GameObject energyValueObject;
    [SerializeField] private energyValueChanger changeEnergyValue;

    [Header("Radius Value")]
    //floater r�diusz kijelz�se
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
        rb = GetComponent<Rigidbody>(); //rb inicializ�l�s
        id = Random.Range(10000, 99999); // �zonos�t� "sorsol�sa"
        maturity = Random.Range(0f, maturityLimit); // lespawnolt nyulak �retts�ge v�letlen
        age = 0f;
        if (fatherId != 0)
        {
            maturity = 0f; // ha m�r egy sz�letett �s nem spawnolt ny�l, akkor alapb�l 0 az �retts�ge
        }
        energiaBar.EnergyBarUpdate(energyLimit, energy);
        changeEnergyValue.SetEnergyValueOnFloater(energyLimit, energy);
        radiusGetter.RadiusStatSetter(radius);
        StartCoroutine(JumpMovement()); //Cooroutine ind�t�sa
    }

    void Update()
    {
        // folyamatosan n�velj�k az �retts�get �s a kort
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
            int inoreDetectionLayerMask = ~LayerMask.GetMask("Ignore Detect");
            Collider[] colliders = Physics.OverlapSphere(transform.position, radius, inoreDetectionLayerMask);

            if (colliders.Length > 0)
            {
                turning = false;
                selectedPlant = colliders[0].gameObject;
                //Debug.Log("N�v�ny kiv�lasztva: " + selectedPlant);
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

    //�J EGYED SZ�LET�SE
    void Reproduction(float heirEnergy)
    {
        GameObject newRabbit = Instantiate(Rabbit, transform.position, transform.rotation); //kl�nozzuk a Rabbit objektumot

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

        newRabbit.transform.position = newPosition;

        // Az �j egyed meg�r�kli a sz�l� �rt�keit kisebb m�dosul�sokkal
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