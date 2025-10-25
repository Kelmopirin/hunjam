using UnityEngine;
using UnityEngine.AI;

public class RatAI : MonoBehaviour
{
    [Header("AI Settings")]
    public Transform player;
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;
    
    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    public float waitTimeAtPoint = 2f;
    
    private NavMeshAgent agent;
    private Animator animator;
    private int currentPatrolIndex = 0;
    private float waitTimer = 0f;
    private RatState currentState = RatState.Patrol;

    private enum RatState
    {
        Patrol,
        Chase,
        Attack,
        Idle
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        
        agent.speed = patrolSpeed;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        // Állapot gépezés
        switch (currentState)
        {
            case RatState.Patrol:
                PatrolBehavior(distanceToPlayer);
                break;
            case RatState.Chase:
                ChaseBehavior(distanceToPlayer);
                break;
            case RatState.Attack:
                AttackBehavior(distanceToPlayer);
                break;
            case RatState.Idle:
                IdleBehavior(distanceToPlayer);
                break;
        }
        
        UpdateAnimations();
    }

    void PatrolBehavior(float distanceToPlayer)
    {
        // Ha a játékos közel van, üldözésbe kezd
        if (distanceToPlayer <= detectionRange)
        {
            currentState = RatState.Chase;
            agent.speed = chaseSpeed;
            return;
        }
        
        // Patrol logika
        if (patrolPoints.Length > 0)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (waitTimer <= 0f)
                {
                    // Következő patrol pont
                    currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                    agent.SetDestination(patrolPoints[currentPatrolIndex].position);
                    waitTimer = waitTimeAtPoint;
                }
                else
                {
                    waitTimer -= Time.deltaTime;
                }
            }
        }
    }

    void ChaseBehavior(float distanceToPlayer)
    {
        // Követi a játékost
        agent.SetDestination(player.position);
        
        // Ha túl messze van, vissza patrolra
        if (distanceToPlayer > detectionRange * 1.5f)
        {
            currentState = RatState.Patrol;
            agent.speed = patrolSpeed;
        }
        // Ha közel van, támadás
        else if (distanceToPlayer <= attackRange)
        {
            currentState = RatState.Attack;
        }
    }

    void AttackBehavior(float distanceToPlayer)
    {
        // Megáll és "támad"
        agent.SetDestination(transform.position);
        
        // Nézzen a játékos felé
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        directionToPlayer.y = 0;
        if (directionToPlayer != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
        
        // Ha túl messze kerül, vissza üldözésbe
        if (distanceToPlayer > attackRange)
        {
            currentState = RatState.Chase;
        }
    }

    void IdleBehavior(float distanceToPlayer)
    {
        // Ha a játékos közel van, üldözésbe kezd
        if (distanceToPlayer <= detectionRange)
        {
            currentState = RatState.Chase;
            agent.speed = chaseSpeed;
        }
    }

    void UpdateAnimations()
    {
        if (animator != null)
        {
            // Sebesség alapján animáció
            animator.SetFloat("Speed", agent.velocity.magnitude);
            
            // Állapot alapján animációk
            animator.SetBool("IsChasing", currentState == RatState.Chase);
            animator.SetBool("IsAttacking", currentState == RatState.Attack);
        }
    }

    // Vizuális segítség a editorban
    void OnDrawGizmosSelected()
    {
        // Detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        // Jelenlegi cél
        if (agent != null && agent.hasPath)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, agent.destination);
        }
    }
}