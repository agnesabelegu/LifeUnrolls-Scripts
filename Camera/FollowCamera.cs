using UnityEngine;
using System.Collections;

public class FollowCamera : MonoBehaviour
{

    public GameObject PlayerObject;
    public GameObject PlanetObject;

    public float DampTime = 1f; // Approximate time for the camera to refocus.

    private Vector3 cameraOffset;


    void Start()
    {
        //cameraOffset = PlayerObject.transform.position - transform.position;
    }

    void LateUpdate()
    {
        //Vector3 directionCameraToPlayer = PlayerObject.transform.position - transform.position; // directionCtoA = positionA - positionC
        //Vector3 directionCameraToPlanet = PlanetObject.transform.position - transform.position; // directionCtoB = positionB - positionC
        //Vector3 midpointPlanetToPlayer = new Vector3(
        //    (directionCameraToPlayer.x + directionCameraToPlanet.x) / 2.0f, 
        //    (directionCameraToPlayer.y + directionCameraToPlanet.y) / 2.0f, 
        //    (directionCameraToPlayer.z + directionCameraToPlanet.z) / 2.0f); // midpoint between A B 

        

        float currentAngle = transform.eulerAngles.y;

        float desiredAngle = PlayerObject.transform.eulerAngles.y;

        float angle = Mathf.LerpAngle(currentAngle, desiredAngle, Time.deltaTime * DampTime);

        Quaternion rotation = Quaternion.Euler(0, angle, 0);
        transform.position = PlayerObject.transform.position - (rotation * cameraOffset);

        //transform.LookAt(midpointPlanetToPlayer);
    }
}
