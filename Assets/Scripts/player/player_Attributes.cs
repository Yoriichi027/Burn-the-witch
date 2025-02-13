using UnityEngine;

public class player_Attributes : MonoBehaviour
{

    public int maxhealth =100;
    public int currenthealth;
    public health_bar health_Bar;
    public Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       currenthealth = maxhealth;
       health_Bar.setmaxvalue(currenthealth);
    }

   
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.KeypadEnter) && currenthealth >0){
            Takedamage(20);
            health_Bar.sethealthbar(currenthealth);
        }

    }
            // responsible for taking damage of all kinds
        public void Takedamage(int damage ){
            currenthealth -=damage;
            
        }
        public void DamagePlayer(int damage, Vector2 knockbackDirection)
    {
        currenthealth -= damage;

        if (health_Bar != null && currenthealth>0 )
        {
            health_Bar.sethealthbar(currenthealth);
        }

        // Apply knockback
        if (rb != null)
        {
             float knockbackForce = 10f; // Adjust this value as needed
             rb.linearVelocity = Vector2.zero; // Reset velocity before applying force
             rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
        }
    }
    }
