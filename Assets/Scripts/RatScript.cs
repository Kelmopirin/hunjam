using UnityEngine;
using UnityEngine.AI;

public class PatkanyAI : MonoBehaviour
{
    [Header("Beállítások")]
    public float minWaitTime = 1f;
    public float maxWaitTime = 3f;
    public float wanderRadius = 10f;
    
    private NavMeshAgent agent;
    private float timer;
    private float currentWaitTime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        SetNewDestination();
        SetRandomWaitTime();
    }

    void Update()
    {
        if (agent == null) return;

        timer += Time.deltaTime;

        if (timer >= currentWaitTime || 
            (agent.hasPath && agent.remainingDistance <= agent.stoppingDistance))
        {
            SetNewDestination();
            SetRandomWaitTime();
            timer = 0f;
        }
    }

    void SetNewDestination()
    {
        if (agent == null) return;

        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += transform.position;
        
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    void SetRandomWaitTime()
    {
        currentWaitTime = Random.Range(minWaitTime, maxWaitTime);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, wanderRadius);
    }
}