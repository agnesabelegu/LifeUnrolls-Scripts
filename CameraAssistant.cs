using UnityEngine;
using System.Collections;

public class CameraAssistant : MonoBehaviour
{
    public GameObject PlayerObject;
    public GameObject PlanetObject;
    public Camera MainCameraObject;

    public float DistanceY = 10;
    public float DistanceZ = -5;
    public float RotationSpeed = 2;
    public float DampingSpeed = 2;

    // The adjusted position should be halfway between the player and the planet
    private Vector3 AdjustedPosition;
    private Quaternion ControlledRotation;


    // Use this for initialization
    void Start()
    {
    
    }

    // Update is called once per frame
    void Update()
    {
        // Match the rotation to the player's rotation
        transform.rotation = PlayerObject.transform.rotation;

        //MainCameraObject.transform.LookAt(PlayerObject.transform);

        // Move with the player object
        transform.position = PlayerObject.transform.position;

        // Rotates the object/player
        MainCameraObject.transform.Rotate(0, Input.GetAxis("Horizontal") * Time.deltaTime * RotationSpeed, 0);

        MainCameraObject.transform.rotation = transform.rotation;
    }
}
