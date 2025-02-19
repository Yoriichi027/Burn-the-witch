using System.Collections;
using UnityEngine;

public class EnemyGroundBeetle : MonoBehaviour
{
    public enum BeetleState { Roaming, Chasing, Dashing }
    private BeetleState currentState = BeetleState.Roaming;

    [Header("Movement Settings")]
    public float roamSpeed = 2f;
    public float chaseSpeed = 3.5f;
    public float dashSpeed = 10f;
    public float dashDuration = 0.5f;
    public float dashCooldown = 2f;

    [Header("Detection Settings")]
    public Transform player;
    public float detectionRange = 5f;
    public float attackRange = 2f;

    [Header("Other Settings")]
    public Rigidbody2D rb;
    public BoxCollider2D visionCollider;
    private bool canDash = true;
    private Vector2 roamDirection = Vector2.right;

    void Start()
    {
        // Randomize starting roam direction
        if (Random.value > 0.5f) roamDirection = Vector2.left;
    }

    void Update()
    {
        switch (currentState)
        {
            case BeetleState.Roaming:
                Roam();
                break;
            case BeetleState.Chasing:
                Chase();
                break;
        }

        DetectPlayer();
    }

    void Roam()
    {
        rb.linearVelocity = new Vector2(roamSpeed * roamDirection.x, rb.linearVelocity.y);
    }

    void Chase()
    {
        float direction = Mathf.Sign(player.position.x - transform.position.x);
        rb.linearVelocity = new Vector2(chaseSpeed * direction, rb.linearVelocity.y);
    }

    void DetectPlayer()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance < attackRange && canDash)
        {
            StartCoroutine(DashAttack());
        }
        else if (distance < detectionRange)
        {
            currentState = BeetleState.Chasing;
        }
        else
        {
            currentState = BeetleState.Roaming;
        }
    }

    IEnumerator DashAttack()
    {
        canDash = false;
        currentState = BeetleState.Dashing;

        float direction = Mathf.Sign(player.position.x - transform.position.x);
        rb.linearVelocity = new Vector2(dashSpeed * direction, 0);

        yield return new WaitForSeconds(dashDuration);

        rb.linearVelocity = Vector2.zero;
        currentState = BeetleState.Chasing;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (currentState == BeetleState.Dashing && collision.gameObject.CompareTag("Player"))
        {
            // Damage player
            Debug.Log("Beetle hit the player!");
            // Implement your damage logic here
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            roamDirection *= -1; // Reverse direction if hitting a wall
        }
    }
}