using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class LevelTimer : MonoBehaviour
{
    public float TimerDuration;
    public Text TimerText;
    
    // Use this for initialization
    void Start()
    {
        StartCoroutine("Timer");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator Timer()
    {
        float timeRemaining = TimerDuration;

        while (timeRemaining > 0)
        {
            yield return new WaitForSeconds(1.0f);
            timeRemaining--;
            TimerText.text = "Time: " + timeRemaining;
        }

        if (timeRemaining == 0)
        {
            GameManager.GameOver = true; 
        }
    }
}
