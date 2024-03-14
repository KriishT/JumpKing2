using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements.Experimental;

public class PlayerMoveemnt : MonoBehaviour
{
    
    Transform trnsfrm;
    Rigidbody2D rb;
    Animator animator;
    [SerializeField] Camera cam;
    [SerializeField] float playerSpeed;
    [SerializeField] float JumpSpeed;
    [SerializeField] float ClimbSpeed;
    CapsuleCollider2D bodyCollider;
    BoxCollider2D myFeet;
    [SerializeField] GameObject playerspawn;
    float gravityScaleAtStart;
    private bool hasExitedTrigger = false;
    private bool hasTriggered = false;


    Vector2 moveInput; //vector2 values used for movement input
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        trnsfrm = GetComponent<Transform>();
        myFeet = GetComponent<BoxCollider2D>();
        bodyCollider = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
        gravityScaleAtStart = rb.gravityScale;

    }

    // Update is called once per frame
    void Update()
    {
        Run();
        flipSprite();
        climbLadder();
        onpressR();
    }


    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>(); // the value of vector 2 is extracted and assigned to the moveInput

    }

    void Run()
    {
        Vector2 playerVelocity = new Vector2(moveInput.x * playerSpeed, rb.velocity.y); //playervelocity is assigned 
        rb.velocity = playerVelocity;

        bool playerHasHorizontalSpeed = Mathf.Abs(rb.velocity.x) > Mathf.Epsilon;
        animator.SetBool("isRunning", playerHasHorizontalSpeed); // hence if the bool player has horizontal speed bool is true, the is running state is also true and if it is false, then so is the isRunning bool

    }

    void flipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(rb.velocity.x) > Mathf.Epsilon; // the bool is set to true if it has horizontal speed i.e. speed>0
        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(rb.velocity.x), 1f); //the mathf.sign returns a -1 value for something thats negative and 1f for summin +ve
        }
        // this basically says that if the player has velocity then assign the scale the value of +1 or -1 based on the velocity
    }

    void climbLadder()
    {

        if (!myFeet.IsTouchingLayers(LayerMask.GetMask("Climbing")))
        {
            rb.gravityScale = gravityScaleAtStart;
            animator.SetBool("isClimbing", false);
            return;
        }

        Vector2 ClimVelocity = new Vector2(rb.velocity.x, moveInput.y * ClimbSpeed);
        rb.velocity = ClimVelocity;
        rb.gravityScale = 0;

        bool playerontheLadder = bodyCollider.IsTouchingLayers(LayerMask.GetMask("Climbing"));
        if (playerontheLadder)
        {
            animator.SetBool("isClimbing", true);
        }

        bool playerHasVectricalSpeed = Mathf.Abs(rb.velocity.y) > Mathf.Epsilon;
        if (!playerHasVectricalSpeed)
        {
            animator.SetBool("isClimbing", playerHasVectricalSpeed);
        }


    }
    void OnJump(InputValue value)
    {


        if (!myFeet.IsTouchingLayers(LayerMask.GetMask("Ground")))
        { 
            if (bodyCollider.IsTouchingLayers(LayerMask.GetMask("Wall")))
            {
                
                if (value.isPressed)
                {
                    
                    rb.velocity += new Vector2(0f, JumpSpeed);
                }
            }
            
                else
                    {
                        
                        return;
                    }
        }

        if (value.isPressed)
        {
            rb.velocity += new Vector2(0f, JumpSpeed);
        }
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("MoveCam") && !hasTriggered)
        {
            hasTriggered = true; // Set the flag to true to indicate the action has been executed

            if (rb.velocity.y > 0f)
            {
                cam.transform.position += new Vector3(0f, 9f, 0f);
            }
            else if (rb.velocity.y < 0f)
            {
                cam.transform.position -= new Vector3(0f, 9f, 0f);
            }
        }
        if (collision.gameObject.CompareTag("MoveCam1"))
        {
            cam.transform.position = new Vector3(-0.05f, 29.37f, -10f);
        }


        if (collision.gameObject.CompareTag("Water"))
        {

            playerkill();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("MoveCam"))
        {
            hasTriggered = false; // Reset the flag when exiting the trigger area
        }
    }






    void resetcam()
    {
        Vector3 newPosition = cam.transform.position;
        newPosition.y = trnsfrm.position.y + 4f;
        cam.transform.position = newPosition;
    }

    void onpressR()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            resetcam();
        }
    }
   
    void playerkill()
    {
         trnsfrm.position = playerspawn.transform.position;
    }
    

}




