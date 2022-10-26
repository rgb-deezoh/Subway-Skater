using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    private const float LANE_DISTANCE = 2.5f;
    private const float TURN_SPEED = 0.05f;

    //
    private bool isInGame = false;

    private CharacterController charController;

    //animation
    private Animator anim;

    //movement
    private float jumpForce = 4.0f;
    private float gravity = 12.0f;
    private float verticalVelocity;

    //speed modifier
    private float originalSpeed = 7.0f;
    private float speed;
    private float speedIncreaseLastTick;
    private float speedIncreseTime = 2.5f;
    private float speedIncreaseAmount = 0.1f;


    //lane formulae
    private int desiredLane = 1; //0 = left, 1 = middle, 2 = right.

    //start method
    private void Start()
    {
        charController = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        speed = originalSpeed;
    }

    private void Update()
    {
        //checks if the game has started other keeps the pinguin in idle position
        if (!isInGame)
            return;

        if (Time.time - speedIncreaseLastTick > speedIncreseTime)
        {
            speedIncreaseLastTick = Time.time;
            speed += speedIncreaseAmount;
            GameManager.Instance.UpdateModifier(speed - originalSpeed);
        }
        //Gather inputs on which lane we should be
        if (MobileInput.Instance.SwipeLeft)
            MoveLane(false);
        if (MobileInput.Instance.SwipeRight)
            MoveLane(true);

        // gather input where we should be
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveLane(false);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveLane(true);
        }

        // calculate where we should be
        Vector3 targetPosition = transform.position.z * Vector3.forward;
        if (desiredLane == 0) {
            targetPosition += Vector3.left * LANE_DISTANCE;
        }else if (desiredLane == 2)
        {
            targetPosition += Vector3.right * LANE_DISTANCE;
        }

        // calculate move delta
        Vector3 moveVector = Vector3.zero;
        moveVector.x = (targetPosition - transform.position).normalized.x * speed;

        //calculate y

        bool isGrounded = IsGrounded();
        anim.SetBool("Grounded", isGrounded);

        if (isGrounded)//Is grounded
        {
            verticalVelocity = -0.1f;

            if (MobileInput.Instance.SwipeUp)
            {
                //jump
                anim.SetTrigger("Jump");
                verticalVelocity = jumpForce;
            }
            else if (MobileInput.Instance.SwipeDown)
            {
                //slide
                StartSliding();
                Invoke("StopSliding", 1.0f);
            }
        }
        else
        {
            //reduces the velocity with gravity every frame
            verticalVelocity -= (gravity * Time.deltaTime);
            //fast fall mechanic
            if (MobileInput.Instance.SwipeDown)
            {
                verticalVelocity = -jumpForce;
            }

        }
        moveVector.y = verticalVelocity;
        moveVector.z = speed;

        //move the penguin

        charController.Move(moveVector * Time.deltaTime);

        //rotate pinguin in the travel direction
        Vector3 dir = charController.velocity;
        if (dir != Vector3.zero) {
            dir.y = 0;
            transform.forward = Vector3.Lerp(transform.forward, dir, TURN_SPEED);
        }

    }

    private void MoveLane(bool goingRight)
    {
        /* This is alot of code, trying to narrow it down
        //left
        if (!goingRight)
        {
            desiredLane--;
            if (desiredLane == -1)
            {
                desiredLane = 0;
            }
            else
            {
                desiredLane++;
                if (desiredLane == 3)
                {
                    desiredLane = 2;
                }
            }
        }
        */

        //This lines of code does the exact thing as the one above
        desiredLane += (goingRight) ? 1 : -1;
        desiredLane = Mathf.Clamp(desiredLane, 0, 2);
    }

    private bool IsGrounded()
    {
        Ray groundRay = new Ray(new Vector3(charController.bounds.center.x, (charController.bounds.center.y - charController.bounds.extents.y) + 0.2f, charController.bounds.center.z), Vector3.down);
        //Debug.DrawRay(groundRay.origin, groundRay.direction, Color.cyan, 1.0f);

        return (Physics.Raycast(groundRay, 0.2f + 0.1f));
    }

    public void StartRunning()
    {
        isInGame = true;
        anim.SetTrigger("StartRunning");
    }

    private void StartSliding()
    {
        anim.SetBool("Sliding", true);
        charController.height /= 2;
        charController.center = new Vector3(charController.center.y, charController.center.y/2, charController.center.z);
    }
    private void StopSliding()
    {
        anim.SetBool("Sliding", false);
        charController.height *= 2;
        charController.center = new Vector3(charController.center.y, charController.center.y * 2, charController.center.z);
    }
    //when the player dies
    private void Crash()
    {
        anim.SetTrigger("Death");
        GameManager.Instance.OnDeath();
        isInGame = false;
        GameManager.Instance.isDead = true;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        switch (hit.gameObject.tag)
        {
            case "Obstacle":
                Crash();
                break;
        }
    }
}
