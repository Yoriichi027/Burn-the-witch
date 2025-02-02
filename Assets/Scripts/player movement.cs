using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class playermovement : MonoBehaviour
{


    [Header("Flip Rotation Stats")]
    [SerializeField] private GameObject followobjectgo;
    public float accelaration;
    public Rigidbody2D body;
    public float groundspeed;
    public float jumpspeed;
    [Range(0f,1f)]
    public float grounddecay;

    public BoxCollider2D groundcheck;
    public LayerMask groundmask;
   
    public bool grounded;
    public bool IsFacingRight;
    private SmoothObjectFollow followobkject;
   
    float xinput;
    void Start(){
        followobkject=followobjectgo.GetComponent<SmoothObjectFollow>();
    }
    void Update()
    {
      getinput();
      jump();
       
    }
       void FixedUpdate(){
        Checkground();
        ApplyFriction();
        Turncheck();
        MoveWithInput();
    }   
    void getinput(){
        xinput = Input.GetAxis("Horizontal");
    }
    void MoveWithInput(){
        if(Mathf.Abs(xinput)>0){
            float increment = xinput * accelaration;
            float newspeed =Mathf.Clamp(body.linearVelocity.x + increment,-groundspeed ,groundspeed );
            body.linearVelocity = new Vector2(newspeed ,body.linearVelocity.y);
        }
        
    }

    void jump(){
        if(Input.GetButtonDown("Jump") && grounded){
            body.linearVelocity = new Vector2(body.linearVelocity.x, jumpspeed);
        }
    }

 

    private void Checkground()
    {
      grounded = Physics2D.OverlapAreaAll(groundcheck.bounds.min, groundcheck.bounds.max, groundmask).Length > 0;  
    }
    void ApplyFriction(){
         if(grounded && xinput==0 && body.linearVelocity.y<=0){
        body.linearVelocity*= grounddecay;
        }
    } 
  private void Turncheck(){
        getinput();
        if(xinput >0 && !IsFacingRight){
            Turn();
        }
        else if (xinput <0 && IsFacingRight)
        {
            Turn();
        }
    }
   void Turn()
{
    if (IsFacingRight)
    {
        transform.rotation = Quaternion.Euler(0, 180f, 0);
        //turn the camera follow  object 
        followobkject.CallTurn();
    }
    else
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
        followobkject.CallTurn();
    }
    IsFacingRight = !IsFacingRight;
}

}
