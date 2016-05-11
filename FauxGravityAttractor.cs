using UnityEngine;
using System.Collections;

public class FauxGravityAttractor : MonoBehaviour {

    public float GravityForce = -10f;
    public float BodyRotationSpeed = 50f;
    public float RotationsPerMinute = 2;
    public float PlanetOrbit = 60f;
    public bool RotatePlanet = false;

    //private
    private float xSpin;
    private float ySpin;
    private float zSpin;

    // Use this for initialization
    void Start () {
        xSpin = Random.Range(0, 360);
        ySpin = Random.Range(0, 360);
        zSpin = Random.Range(0, 360);
    }

    public void Attract(Transform body)
    {
        Vector3 gravityUp = (body.position - transform.position).normalized;
        Vector3 bodyUp = body.up;

        body.GetComponent<Rigidbody>().AddForce(gravityUp*GravityForce);
        Quaternion targetRotation = Quaternion.FromToRotation(bodyUp, gravityUp) * body.rotation;
        body.rotation = Quaternion.Slerp(body.rotation, targetRotation, BodyRotationSpeed * Time.deltaTime); //spherical interpolation
    }

    void FixedUpdate()
    {

        if (RotatePlanet)
        {
            transform.Rotate(xSpin * RotationsPerMinute * Time.deltaTime, ySpin * RotationsPerMinute * Time.deltaTime, zSpin * RotationsPerMinute * Time.deltaTime);

            if (RotationsPerMinute >= 0.02f || RotationsPerMinute >= 0.001f)
            {
                RotationsPerMinute -= 0.001f * Time.deltaTime;
            }
            else if (RotationsPerMinute <= 0.001f)
            {
                RotationsPerMinute = 0.02f;
            }
        }
        
    }
}
