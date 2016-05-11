using UnityEngine;

public class AIObstacle : MonoBehaviour
{
    //DefaultObstacle
    public EnemyTypes EnemyType; //obstacle type
    public EnemyStates CurrentEnemyState; //enemy state check
    public float PushForce = 1f; //force to push enemy when releasing
    public float MoveSpeed = 1f; //default movement speed
    public GameObject EnemyTarget; //Default "Lose condition" to carry player to.

    //Charging obstacle
    public float ChargeTime = 1f;
    public float ChargeForce = 100f;

    //TODO Charging bar
    private Rigidbody myRigidBody;
    private Transform playerPlaceholder; // empty gameobject within which to contain player
    private float chargeTimer = 0; //amount of time the enemy charges for
    private GameObject playerObject; //reference to player object
    private Rigidbody playerRigidBody; //reference to player rigid body
    private Vector3 currentPlayerPos; // constantly updates player's location to follow
    private Vector3 latestPlayerPos; // latest memorized location of player to charge towards
    private bool isPlayerCaptured;
    private bool isPlayerInfected;

    // Use this for initialization
    void Start()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject == null)
        {
            Debug.Log("Player could not be found");
        }
        else
        {
            playerRigidBody = playerObject.GetComponent<Rigidbody>();
        }
        myRigidBody = GetComponent<Rigidbody>();

        if (EnemyTarget == null)
        {
            EnemyTarget = GameObject.FindGameObjectWithTag("EnemyTarget");
        }

        if (EnemyType == EnemyTypes.ChasingObstacle || EnemyType == EnemyTypes.ChargingObstacle)
        {
            CurrentEnemyState = EnemyStates.Following;
        }
        else if (EnemyType == EnemyTypes.AmbushingObstacle)
        {
            CurrentEnemyState = EnemyStates.HoldingPosition;
        }

        playerPlaceholder = gameObject.transform.GetChild(0);
    }

    void Update()
    {
        currentPlayerPos = playerObject.transform.position;

        //If the enemy type is a chasing obstacle
        //TODO - similar to charging obstacle
        if (EnemyType == EnemyTypes.ChasingObstacle)
        {
            ActivateChasingObstacleLogic();
        }
        else if (EnemyType == EnemyTypes.ChargingObstacle)
        {
            ActivateChargingObstacleLogic();
        }
        else if (EnemyType == EnemyTypes.AmbushingObstacle)
        {
            ActivateAmbushingObstacleLogic();
        }

        if (CurrentEnemyState == EnemyStates.HoldingPosition)
        {
            myRigidBody.velocity = Vector3.zero;
        }

        isPlayerCaptured = playerObject.GetComponent<PlayerController>().LostControl;
        isPlayerInfected = playerObject.GetComponent<PlayerController>().Infected;
    }

    void ActivateChargingObstacleLogic()
    {
        if (CurrentEnemyState == EnemyStates.Following)
        {
            FollowPlayer();
        }
        if (CurrentEnemyState == EnemyStates.Charging)
        {
            ChargeAtPlayer();
        }
        if (CurrentEnemyState == EnemyStates.Capturing)
        {
            CarryPlayerToPrison();
        }
    }

    void ActivateChasingObstacleLogic()
    {
        if (CurrentEnemyState == EnemyStates.Following)
        {
            FollowPlayer();
        }
        else if (CurrentEnemyState == EnemyStates.Capturing)
        {
            CarryPlayerToPrison();
        }
    }

    void ActivateAmbushingObstacleLogic()
    {
        if (CurrentEnemyState == EnemyStates.Charging)
        {
            ChargeAtPlayer();
        }
        else
        {
            CurrentEnemyState = EnemyStates.HoldingPosition;
        }
    }

    //Following the player transform
    void FollowPlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, currentPlayerPos, MoveSpeed * Time.deltaTime);
    }

    //Carrying the player to FailGame location
    void CarryPlayerToPrison()
    {
        chargeTimer = 0;
        //Catch the player if reached

        if (playerObject.GetComponent<PlayerController>().LostControl == true)
        {
            transform.position = Vector3.MoveTowards(transform.position, EnemyTarget.transform.position, MoveSpeed * Time.deltaTime);

            playerObject.transform.parent = transform;
            playerObject.transform.position = playerPlaceholder.position;
            playerObject.transform.rotation = playerPlaceholder.rotation;
        }
        //Release the player if they nudge out of grasp
        else
        {
            CurrentEnemyState = EnemyStates.Releasing;

            playerObject.transform.localPosition = -Vector3.forward;
            playerRigidBody.AddForce(-playerPlaceholder.transform.forward * PushForce, ForceMode.Force);
            playerObject.transform.parent = null;

            CurrentEnemyState = EnemyStates.Following;
        }

    }

    //Charging towards the player location after 1 second of charge-up time
    void ChargeAtPlayer()
    {
        if (chargeTimer <= ChargeTime)
        {
            transform.LookAt(latestPlayerPos);
            myRigidBody.AddRelativeForce(0, 0, ChargeForce);

            myRigidBody.drag = 1.5f;

            chargeTimer += Time.deltaTime;
        }
        else
        {
            myRigidBody.drag = 0;
            chargeTimer = 0;
            if (EnemyType == EnemyTypes.ChargingObstacle)
            {
                CurrentEnemyState = EnemyStates.Following;
            }
            else if (EnemyType == EnemyTypes.AmbushingObstacle)
            {
                CurrentEnemyState = EnemyStates.HoldingPosition;
            }

        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            if ((EnemyType == EnemyTypes.ChargingObstacle || EnemyType == EnemyTypes.ChasingObstacle) && !isPlayerCaptured)
            {
                CurrentEnemyState = EnemyStates.Capturing;
                playerObject.GetComponent<PlayerController>().LostControl = true;

            }
            else if (EnemyType == EnemyTypes.AmbushingObstacle )
            {
                playerObject.GetComponent<PlayerController>().Infected = true;
                CurrentEnemyState = EnemyStates.HoldingPosition;
            }
        }

        if (other.gameObject.tag == "EnemyTarget" && CurrentEnemyState == EnemyStates.Capturing)
        {
            GameManager.GameOver = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && ((EnemyType == EnemyTypes.ChargingObstacle || (EnemyType == EnemyTypes.AmbushingObstacle && !isPlayerInfected) && !isPlayerCaptured)))
        {
            CurrentEnemyState = EnemyStates.Charging;
            latestPlayerPos = currentPlayerPos;
        }
    }
}
