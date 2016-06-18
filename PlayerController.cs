using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;
using UnityEngine.SceneManagement;

//TODO
public enum MovementMode
{
    Normal,
    Acceleration
}

public class PlayerController : MonoBehaviour
{
    //public
    [Tooltip("Time to infection clearing up")]
    private float TimeToClarity = 10f;
    public float MoveSpeed = 30f;
    public float RotationSpeed = 5f;
    public float MinSpeed = 0f;
    public float MaxSpeed = 20f;
    public float ForceMagnitude;
    public float JumpForce = 250f;
    public LayerMask groundedMask;
    public GameObject InfectedOverlay;
    public int NudgesToFreedom = 10; //How many times does the player have to nudge to free themselves?

    //Hidden in inspector
    [HideInInspector]
    public bool LostControl = false;
    [HideInInspector]
    public bool Infected = false;

    //Private
    private int currentNudges; //How many times has the player nudged?
    private bool grounded; //Is the player on the ground
    private float distanceFromPlanet; //How far away is the player from the center of the planet?
    private GameObject planet;
    private Rigidbody myRigidBody;
    private float timer;

    //Controls
    private Vector3 moveDirection;

    void Start()
    {
        myRigidBody = GetComponent<Rigidbody>();
        planet = GameObject.FindGameObjectWithTag("Planet");
        LostControl = false;
    }

    void Update()
    {
        if (!GameManager.GameOver)
        {
            if (Input.GetButtonDown("Jump"))
            {
                if (grounded)
                {
                    myRigidBody.AddForce(transform.up * JumpForce);
                }
            }

            if (Input.touchCount == 1)
            {
                if (grounded)
                {
                    myRigidBody.AddForce(transform.up * JumpForce);
                }
            }
            
            //Check if grounded to disable double-jumping.
            grounded = false;
            
            Ray ray = new Ray(transform.position, -transform.up);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, transform.localScale.y + 0.1f, groundedMask))
            {
                grounded = true;
            }
        }

        //If the player is "Infected" by an ambushing obstacle, activate the overlay until the infection dissipates.
        if (Infected)
        {
            InfectedOverlay.SetActive(true);

            if (timer <= TimeToClarity)
            {
                timer += Time.deltaTime;
            }
            else
            {
                InfectedOverlay.SetActive(false);
                Infected = false;
            }
        }
    }

    //Check if the player has left orbit
    private IEnumerator CheckDistanceFromPlanet()
    {
        distanceFromPlanet = Vector3.Distance(transform.position, planet.transform.position);

        if (distanceFromPlanet > planet.GetComponent<FauxGravityAttractor>().PlanetOrbit)
        {
            gameObject.GetComponent<FauxGravityBody>().enabled = false;

            yield return new WaitForSeconds(2f);

            GameManager.GameOver = true;

        }
    }

    void FixedUpdate()
    {
        float horizontalAxis = Input.GetAxis("Horizontal") * RotationSpeed;
        transform.Rotate(0, horizontalAxis, 0);

        float accelerationX = Input.GetAxisRaw("Horizontal");
        float accelerationY = Input.GetAxisRaw("Vertical");

        Vector3 moveDirectionKeyboard = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
       
        //If player is still in control of ball
        if (!LostControl)
        {
            myRigidBody.MovePosition(myRigidBody.position + transform.TransformDirection(moveDirectionKeyboard * MoveSpeed * Time.deltaTime));
        }
        //If player has been caught and lost control, count their nudges
        else
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                //If player presses Space, increasee their nudge factor
                if (currentNudges != NudgesToFreedom)
                {
                    currentNudges++;
                }
                //If the player nudged their way out of captivity, set them free
                else
                {
                    LostControl = false;
                    currentNudges = 0;
                }
            }
        }
    }

    void Move(float accelerationX, float accelerationY)
    {
        if (accelerationX != 0 || accelerationY != 0)
        {
            // Set the movement vector based on the axis input.
            moveDirection.Set(accelerationX, 0f, accelerationY);
            MoveSpeed = Mathf.Min(MoveSpeed + ForceMagnitude * Time.deltaTime, MaxSpeed);
        }
        else
        {
            MoveSpeed = Mathf.Max(MoveSpeed - ForceMagnitude * Time.deltaTime, MinSpeed);
        }

        // Normalise the movement vector and make it proportional to the speed per second.
        moveDirection = moveDirection.normalized * MoveSpeed * Time.deltaTime;
        // Move the player to it's current position plus the movement.
        myRigidBody.MovePosition(myRigidBody.position + transform.TransformDirection(moveDirection));
    }

}
