using UnityEngine;

public class Spike : MonoBehaviour
{
    public int damage = 20;
    public float knockbackStrength = 5f;

   private void OnTriggerEnter2D(Collider2D other)
{
   
    player_Attributes player = other.GetComponentInParent<player_Attributes>();
   
    if (player != null)
    {
       Vector2 knockbackDirection = (other.transform.position - transform.position).normalized;
    knockbackDirection.y = Mathf.Abs(knockbackDirection.y) + 0.5f; // Makes sure it always knocks UP slightly
    knockbackDirection.Normalize(); // Normalize again to keep movement balanced

    player.DamagePlayer(20, knockbackDirection);
    }
}

}
