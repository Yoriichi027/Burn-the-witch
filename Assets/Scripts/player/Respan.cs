using System.Diagnostics;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    // 
    private static Transform Last_Checkpoint;
     private static Collider2D lastCheckpointCollider; 
    public  void Respan_(){
        transform.position = Last_Checkpoint.position;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (lastCheckpointCollider != null)
            {
                lastCheckpointCollider.enabled = true;
                //  UnityEngine.Debug.LogError("check point registered");
            }

        if(collision.transform.tag == "Checkpoint"){
            Last_Checkpoint = collision.transform;
            lastCheckpointCollider = collision.GetComponent<Collider2D>();
            collision.GetComponent<Collider2D>().enabled =false;
        }
    }
}
