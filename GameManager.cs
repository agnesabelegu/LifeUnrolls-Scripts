using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject[] PlayerObjectives;
    public static bool GameOver;
    public bool ContainsHelper;

    [HideInInspector]
    public GameObject CurrentPlayerObjective;

    private GameObject playerObject;
    private GameObject planetObject;
    private GameObject helperObject;
    private int currentObjective;
    private float playerDistanceFromPlanet;

    void Start()
    {
        currentObjective = 0;
        GameOver = false;
        playerObject = GameObject.FindGameObjectWithTag("Player");
        planetObject = GameObject.FindGameObjectWithTag("Planet");
        StartCoroutine(GameLoop());

        CurrentPlayerObjective = PlayerObjectives[currentObjective];

        if (ContainsHelper)
        {
            helperObject = GameObject.FindGameObjectWithTag("Helper");
            helperObject.GetComponent<AIHelper>().CurrentHelperObjective = CurrentPlayerObjective;
        }
    }

    void Update()
    {
        if (GameOver)
        {
            float cameraThreshold = Camera.main.GetComponent<BloomOptimized>().threshold;
            Camera.main.GetComponent<BloomOptimized>().threshold = Mathf.Lerp(cameraThreshold, 0, Time.deltaTime);

            if (Camera.main.GetComponent<BloomOptimized>().threshold <= 0.1)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    public void ObjectiveReached()
    {
        if (currentObjective != PlayerObjectives.Length - 1)
        {
            //deactivate the previous objective
            CurrentPlayerObjective.SetActive(false);

            //iterate on the next objective
            currentObjective++;
            CurrentPlayerObjective = PlayerObjectives[currentObjective];
            CurrentPlayerObjective.SetActive(true);
            CurrentPlayerObjective.GetComponent<Objective>().IsCurrentObjective = true;
            if (ContainsHelper)
            {
                helperObject.GetComponent<AIHelper>().CurrentHelperObjective = CurrentPlayerObjective;
            }
        }
        else
        {
            StartCoroutine(SwitchLevel());
        }
    }

    private IEnumerator CheckDistanceFromPlanet()
    {
        playerDistanceFromPlanet = Vector3.Distance(playerObject.transform.position, planetObject.transform.position);

        if (playerDistanceFromPlanet > planetObject.GetComponent<FauxGravityAttractor>().PlanetOrbit)
        {
            playerObject.GetComponent<FauxGravityBody>().enabled = false;

            yield return new WaitForSeconds(2f);

            GameOver = true;
        }
    }

    private IEnumerator GameLoop()
    {
        yield return CheckDistanceFromPlanet();

        if (!GameOver)
        {
            StartCoroutine(GameLoop());
            yield return null;
        }
    }

    public IEnumerator SwitchLevel()
    {
        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
