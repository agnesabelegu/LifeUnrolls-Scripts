using UnityEngine;

public class AIHelper : MonoBehaviour
{
    public float MoveSpeed = 40f;
    public HelperStates CurrentState; //What is the helper currently doing?

    [HideInInspector]
    public GameObject CurrentHelperObjective; //Where is the helper supposed to go next?

    void Update()
    {
        //Simply move the helper to the current destination.
        if (CurrentState == HelperStates.MovingToLocation)
        {
            transform.position = Vector3.MoveTowards(transform.position, CurrentHelperObjective.transform.position, MoveSpeed * Time.deltaTime);
        }

    }

    void OnColliderEnter(Collision other)
    {
        //Once the player reaches the helper, start moving to the next location.
        if (other.gameObject.tag == "Player")
        {
            CurrentState = HelperStates.MovingToLocation;
        }
        //If the helper has reached their objective, stop and wait for player.
        if (other.gameObject.tag == "Objective")
        {
            if (other.gameObject.GetComponent<Objective>().IsCurrentObjective && other.gameObject.GetComponent<Objective>().isActiveAndEnabled)
            {
                CurrentState = HelperStates.WaitingForPlayer;
            }
        }
    }
}
