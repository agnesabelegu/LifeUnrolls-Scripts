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
    public MovementMode CurrentMovementMode; //TODO
    public float MoveSpeed = 30f;
    public float RotationSpeed = 5f;
    public float MinSpeed = 0f;
    [Tooltip("Time to infection clearing up")]
    private float TimeToClarity = 10f;
    public float MaxSpeed = 20f;
    public float ForceMagnitude;
    public float JumpForce = 250f;
    public LayerMask groundedMask;
    public GameObject InfectedOverlay;
    public int NudgesToFreedom = 10; //How many times does the player have to nudge to free themselves?

    [HideInInspector]
    public bool LostControl = false;

    //TODO Flanking enemy logic
    [HideInInspector]
    public bool Infected = false;

    //Private

    public int currentNudges; //How many times has the player nudged?
    private bool grounded; //Is the player on the ground
    private float distanceFromPlanet; //How far away is the player from the center of the planet?
    private GameObject planet;
    private Rigidbody myRigidBody;
    private float timer;

    //Controls
    private Vector3 moveDirection;
    //private Vector3 moveDirectionTablet;
    //private bool touchDevice; //Is the player using a tablet or keyboard?


    //void Awake()
    //{
    //    //check if our current system info equals a desktop
    //    if (SystemInfo.deviceType == DeviceType.Desktop)
    //    {
    //        //we are on a desktop device, so don't use touch
    //        touchDevice = false;
    //    }
    //    //if it isn't a desktop, lets see if our device is a handheld device aka a mobile device
    //    else if (SystemInfo.deviceType == DeviceType.Handheld)
    //    {
    //        //we are on a mobile device, so lets use touch input
    //        touchDevice = true;
    //    }
    //}

    // Use this for initialization
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody>();
        planet = GameObject.FindGameObjectWithTag("Planet");
        LostControl = false;
    }

    // Update is called once per frame
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

            grounded = false;
            //Check if grounded
            Ray ray = new Ray(transform.position, -transform.up);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, transform.localScale.y + 0.1f, groundedMask))
            {
                grounded = true;
            }
        }

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
        //if (touchDevice)
        //{
        //   float accelerationX = Input.acceleration.x;
        //   float accelerationY = Input.acceleration.y;
        //   myRigidBody.MovePosition(myRigidBody.position + transform.TransformDirection(moveDirectionTablet * MoveSpeed * Time.deltaTime));
        //}
        //else
        //{
        //   float accelerationX = Input.GetAxisRaw("Horizontal");
        //   float accelerationY = Input.GetAxisRaw("Vertical");
        //}
        float horizontalAxis = Input.GetAxis("Horizontal") * RotationSpeed;
        transform.Rotate(0, horizontalAxis, 0);

        float accelerationX = Input.GetAxisRaw("Horizontal");
        float accelerationY = Input.GetAxisRaw("Vertical");

        Vector3 moveDirectionKeyboard = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        //Vector3 moveDirectionTablet = new Vector3(accelerationX, 0, accelerationY).normalized;

        //If player is still in control of ball
        if (!LostControl)
        {
            //Allow movement inputs
            //Move(accelerationX, accelerationY);

            //Old Movement logic
            myRigidBody.MovePosition(myRigidBody.position + transform.TransformDirection(moveDirectionKeyboard * MoveSpeed * Time.deltaTime));
            //myRigidBody.MoveRotation(Quaternion.Euler(transform.TransformDirection(moveDirectionKeyboard * RotationSpeed * Time.deltaTime)));
           // myRigidBody.MovePosition(myRigidBody.position + transform.TransformDirection(moveDirectionTablet * MoveSpeed * Time.deltaTime));
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