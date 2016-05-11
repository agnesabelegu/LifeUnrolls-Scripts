using UnityEngine;

public class AIHelper : MonoBehaviour
{
    public float MoveSpeed = 40f;
    public HelperStates CurrentState;

    [HideInInspector]
    public GameObject CurrentHelperObjective;


    void Update()
    {
        if (CurrentState == HelperStates.MovingToLocation)
        {
            transform.position = Vector3.MoveTowards(transform.position, CurrentHelperObjective.transform.position, MoveSpeed * Time.deltaTime);
        }

    }

    void OnColliderEnter(Collision other)
    {
        if (other.gameObject.tag == "Player")
        {
            CurrentState = HelperStates.MovingToLocation;

        }
        if (other.gameObject.tag == "Objective")
        {
            if (other.gameObject.GetComponent<Objective>().IsCurrentObjective && other.gameObject.GetComponent<Objective>().isActiveAndEnabled)
            {
                CurrentState = HelperStates.WaitingForPlayer;
            }
        }
    }
}
