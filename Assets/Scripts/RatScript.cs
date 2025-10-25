using UnityEngine;

public class SimpleWallAvoidRat : MonoBehaviour
{
    public float speed = 3f;
    public float rotationSpeed = 2f;
    
    private Vector3 currentDirection;
    private float changeTimer = 2f;

    void Start()
    {
        SetRandomDirection();
    }

    void Update()
    {
        changeTimer -= Time.deltaTime;
        
        if (changeTimer <= 0f)
        {
            SetRandomDirection();
            changeTimer = Random.Range(1f, 3f);
        }
        
        // Mozgás
        transform.position += currentDirection * speed * Time.deltaTime;
        
        // Forgás
        if (currentDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(currentDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    void SetRandomDirection()
    {
        currentDirection = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Ütközéskor visszapattan és új irányt választ
        Vector3 reflection = Vector3.Reflect(currentDirection, collision.contacts[0].normal);
        currentDirection = reflection.normalized;
        changeTimer = Random.Range(0.5f, 1.5f);
    }
}