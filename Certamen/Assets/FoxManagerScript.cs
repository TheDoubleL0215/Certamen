using UnityEngine;
using UnityEngine.AI;

public class FoxManagerScript : MonoBehaviour
{
    Rigidbody rb;
    public float jumpForce = 5f;
    public float forwardForce = 5f;
    public float range;
    public float radius = 0f;

    public NavMeshAgent agent;
    public Transform centrePoint;
    [SerializeField] private GameObject selectedRabbit;

    public float hungerLevel = 100f;
    public float hungerLoss = 5f;
    public enum State { Idle, Hunger }
    [SerializeField] private State state;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        radius = Random.Range(10, 20);
        state = State.Idle;
    }

    void Update()
    {
        PerformMovement();
        UpdateHungerState();
    }

    void PerformMovement()
    {
        hungerLevel -= hungerLoss / 1000;

        if (hungerLevel <= 0)
        {
            Destroy(gameObject);
        }
    }

    void UpdateHungerState()
    {
        if (hungerLevel <= 90)
        {
            state = State.Hunger;
            FoodMovement();
        }
        else
        {
            state = State.Idle;
            IdleMovement();
        }
    }

    void IdleMovement()
    {
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            Vector3 point;
            if (RandomPoint(centrePoint.position, radius, out point))
            {
                Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f);
                agent.SetDestination(point);
            }
        }
    }

    void FoodMovement()
    {
        if (selectedRabbit == null)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("Rabbit"))
                {
                    //Debug.Log("Rabbit Detected!");
                    selectedRabbit = collider.gameObject;
                    agent.SetDestination(selectedRabbit.transform.position);
                }
            }
        }
        else if (Vector3.Distance(transform.position, selectedRabbit.transform.position) < 10f)
        {
            //print("megéve");
            Destroy(selectedRabbit);
            hungerLevel += 50f;
            selectedRabbit = null;
        }
    }

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        Vector3 randomPoint = center + Random.insideUnitSphere * range;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
