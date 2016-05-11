using UnityEngine;
using System.Collections;

[RequireComponent(typeof (Rigidbody))]
public class FauxGravityBody : MonoBehaviour
{
    //private
    private Rigidbody myRigidBody;
    private FauxGravityAttractor Attractor;

    // Use this for initialization
    void Start ()
    {
        Attractor = GameObject.FindGameObjectWithTag("Planet").GetComponent<FauxGravityAttractor>();

        myRigidBody = GetComponent<Rigidbody>();
        myRigidBody.constraints = RigidbodyConstraints.FreezeRotation;
        myRigidBody.useGravity = false;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        Attractor.Attract(transform);
	}
}
