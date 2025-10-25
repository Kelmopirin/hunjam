using UnityEngine;
using System.Collections;

public class RatAI : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float rotationSpeed = 5f;
    public float minWaitTime = 1f;
    public float maxWaitTime = 3f;
    public float moveDistance = 5f;

    [Header("Detection Settings")]
    public LayerMask obstacleLayer = 1;
    public float detectionDistance = 2f;

    private Vector3 targetPosition;
    private bool isMoving = false;
    private Animator animator;
    private Rigidbody rb;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        
        // Kezdeti célpont beállítása
        SetNewRandomTarget();
        StartCoroutine(MovementRoutine());
    }

    void Update()
    {
        if (isMoving)
        {
            MoveToTarget();
        }
    }

    IEnumerator MovementRoutine()
    {
        while (true)
        {
            // Várakozás
            isMoving = false;
            if (animator != null)
                animator.SetBool("IsMoving", false);
            
            yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
            
            // Új célpont keresése
            SetNewRandomTarget();
            isMoving = true;
            if (animator != null)
                animator.SetBool("IsMoving", true);
            
            // Mozgás egy ideig
            yield return new WaitForSeconds(Random.Range(2f, 5f));
        }
    }

    void SetNewRandomTarget()
    {
        Vector3 randomDirection = Random.insideUnitSphere * moveDistance;
        randomDirection.y = 0; // 2D síkban maradjon

        targetPosition = transform.position + randomDirection;

        // Akadály elkerülése
        if (Physics.Raycast(transform.position, (targetPosition - transform.position).normalized, 
            detectionDistance, obstacleLayer))
        {
            // Ha akadály van, másik irányba fordul
            randomDirection = Quaternion.Euler(0, 90, 0) * randomDirection;
            targetPosition = transform.position + randomDirection;
        }
    }

    void MoveToTarget()
    {
        // Irány kiszámítása
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            // Forgás a célpont felé
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Mozgás előre
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

        // Ha közel van a célponthoz, új célpontot állít be
        if (Vector3.Distance(transform.position, targetPosition) < 0.5f)
        {
            SetNewRandomTarget();
        }
    }

    // Vizuális segítség a fejlesztéshez
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(targetPosition, 0.3f);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, targetPosition);
    }

    // Ütközés kezelése - patkány megfordul akadálynál
    void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & obstacleLayer) != 0)
        {
            // Azonnal új irány
            SetNewRandomTarget();
        }
    }
}