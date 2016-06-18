using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Objective : MonoBehaviour
{
    public bool ObjectiveReached;
    public bool IsCurrentObjective;
    public AudioClip ObjectiveAudio;

    private GameManager GameManager;
    
    void Start ()
    {
        GameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        
        //Play the repetitive sound soon as Objective is initialized so player can find its location. 
        GetComponent<AudioSource>().clip = ObjectiveAudio;
        GetComponent<AudioSource>().Play();
    }

    void OnCollisionEnter(Collision other)
    {
        //Once objective is reached
        if (other.gameObject.tag.Equals("Player") && IsCurrentObjective && !ObjectiveReached)
        {
            GameManager.ObjectiveReached();
            ObjectiveReached = true;
        }
    }
}
