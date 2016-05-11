using UnityEngine;
using System.Collections;
/// <summary>
/// The purpose of this script is to make whatever has this script raycast downards, get what was hit,
/// and make the object with this script "stick" to the raycast hit object.
/// </summary>
public class SphericalGravityEffect : MonoBehaviour
{
    public float planetLerp = 1f;
    public float linkDistance = 30f;
    public float rotateCatchupSpeed = 1f;
    public float rotateSpeed = 0.1f;

    private Transform planetObject;
    private Quaternion startingQuaternion;
    private Vector3 startUpPosition;

    private bool IsColliding;

    // Use this for initialization
    void Start()
    {       

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 downwardVector = transform.TransformDirection(Vector3.down);
        RaycastHit hit;
        IsColliding = false;

        if (Physics.Raycast(transform.position, downwardVector, out hit, linkDistance))
        {
            ConnectToPlanet(hit);
        }

        if (!IsColliding)
        {
            transform.Rotate(rotateCatchupSpeed * Time.deltaTime, 0f, 0f);
        }

        IsColliding = false;
    }

    void ConnectToPlanet(RaycastHit Planet)
    {
        if (Planet.transform.CompareTag("Planet"))
        {
            IsColliding = true;

            transform.rotation = Quaternion.FromToRotation(transform.up, Planet.normal) * transform.rotation;
        }
    }
}
