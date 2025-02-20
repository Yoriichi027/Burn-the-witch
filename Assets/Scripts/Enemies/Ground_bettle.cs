using System.Collections;
using UnityEngine;

public class EnemyGroundBeetle : MonoBehaviour
{
    // Basic movement & states
    public float Movement_speed;
    public float CircleRadius;
    public Rigidbody2D Enemy_RB;
    public GameObject GroundCheck;
    public LayerMask GroundLayer;
    public bool FacingRight;
    public bool IsGrounded;
    
    private bool isAttacking;
    private bool isChasing;
    private float timeSinceLastSeen = 0f;
    public float lostPlayerTime = 3f; // Time before giving up chase

    // Player detection
    public float DetectionRange;
    public Transform Player;
    public LayerMask PlayerLayer;
    
    // Attack properties
    public float AttackRange;
    public float dashSpeed;

    void Start()
    {
        Enemy_RB = GetComponent<Rigidbody2D>();

        if (Enemy_RB == null)
            Debug.LogError("No Rigidbody2D found on " + gameObject.name);
        if (GroundCheck == null)
            Debug.LogError("GroundCheck GameObject is not assigned on " + gameObject.name);
    }

    void Update()
    {
        if (isAttacking)
        {
            return; // Don't interrupt attack state
        }
        
        if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            Roam();
            DetectPlayer();
        }
    }

    void DetectPlayer()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, Player.position);
        Vector2 directionToPlayer = (Player.position - transform.position).normalized;
        Vector2 enemyFacingDirection = FacingRight ? Vector2.right : Vector2.left;

        float dot = Vector2.Dot(directionToPlayer, enemyFacingDirection);

        if (distanceToPlayer <= DetectionRange && dot > 0.5f)
        {
            Debug.Log("Player spotted! Switching to Chase Mode!");
            isChasing = true;
            timeSinceLastSeen = 0f;
        }
    }

    void ChasePlayer()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, Player.position);
        
        if (distanceToPlayer > DetectionRange) 
        {
            // Lost sight of the player? Start the countdown to give up.
            timeSinceLastSeen += Time.deltaTime;

            IsGrounded = GroundCheck != null && Physics2D.OverlapCircle(GroundCheck.transform.position, CircleRadius, GroundLayer);
            if (IsGrounded){
               // Move towards the player
            float chase_Direction = (Player.position.x > transform.position.x) ? 1f : -1f;
            Enemy_RB.linearVelocity = new Vector2(chase_Direction * Movement_speed, Enemy_RB.linearVelocity.y);

            // Flip direction if needed
            if ((chase_Direction > 0 && !FacingRight) || (chase_Direction < 0 && FacingRight))
            {
                Flip();
            }     
            }

            if (timeSinceLastSeen >= lostPlayerTime)
            {
                isChasing = false;
                timeSinceLastSeen = 0f;
                ReturnToRoam();
            }
            return;
        }
        
        if (distanceToPlayer > AttackRange)
        {
            
            
            // Move towards the player
            float chaseDirection = (Player.position.x > transform.position.x) ? 1f : -1f;
            Enemy_RB.linearVelocity = new Vector2(chaseDirection * Movement_speed, Enemy_RB.linearVelocity.y);

            // Flip direction if needed
            if ((chaseDirection > 0 && !FacingRight) || (chaseDirection < 0 && FacingRight))
            {
                Flip();
            }
        }
        else
        {
            // Stop and attack when in range
            Enemy_RB.linearVelocity = Vector2.zero;
            isChasing = false;
            isAttacking = true;
            StartCoroutine(ChargeAndAttack());
        }
    }

    IEnumerator ChargeAndAttack()
    {
        Debug.Log("Charging attack!");

        // Store the player's position at the moment of charging
        Vector2 targetPosition = Player.position;

        yield return new WaitForSeconds(0.2f); // Charging for 0.2s

        // Determine dash direction based on stored position
        float dashDirection = (targetPosition.x > transform.position.x) ? 1f : -1f;
        Enemy_RB.linearVelocity = new Vector2(dashDirection * dashSpeed, Enemy_RB.linearVelocity.y);

        yield return new WaitForSeconds(0.2f); // Dash duration

        // Stop movement completely
        Enemy_RB.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(0.5f); // Pause for 0.5s after dash

        isAttacking = false; // Reset attack state
        isChasing = true; // Resume chasing if player is still detected
    }

    void Roam()
    {
        if (isChasing || isAttacking)
            return;

        Enemy_RB.linearVelocity = new Vector2((FacingRight ? 1 : -1) * Movement_speed, Enemy_RB.linearVelocity.y);
        IsGrounded = GroundCheck != null && Physics2D.OverlapCircle(GroundCheck.transform.position, CircleRadius, GroundLayer);

        if (!IsGrounded)
            Flip();
    }

    void ReturnToRoam()
    {
        Debug.Log("back to roam");
        isChasing = false;
        isAttacking = false;
        Enemy_RB.linearVelocity = Vector2.zero;
        Roam();
    }

    private void OnDrawGizmosSelected()
    {
        if (GroundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(GroundCheck.transform.position, CircleRadius);
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, DetectionRange);

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, AttackRange);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, DetectionRange + 4f); // Chase range visualization
        }
    }

    void Flip()
    {
        FacingRight = !FacingRight;
        transform.Rotate(0, 180, 0);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        player_Attributes player = other.GetComponentInParent<player_Attributes>();

        if (player != null)
        {
            Vector2 knockbackDirection = (other.transform.position - transform.position).normalized;
            knockbackDirection.y = Mathf.Abs(knockbackDirection.y) + 0.5f;
            knockbackDirection.Normalize();
            player.DamagePlayer(20, knockbackDirection);
        }
    }
}
