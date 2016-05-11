using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    public GameObject Obstacle;
    public float SpawnTime = 4f;
    public float EnemySpawnVerticalOffset = 50f;
    
    private Vector3 spawnPosition;
    private SphereCollider spawnTrigger;
    private bool stopSpawning = false;
    
    // Use this for initialization
    void Start()
    {
        spawnTrigger = GetComponent<SphereCollider>();
        StartCoroutine(SpawnEnemyLoop());
    }

    private IEnumerator SpawnEnemy()
    {

        yield return new WaitForSeconds(SpawnTime);
        spawnPosition = new Vector3(Random.insideUnitSphere.x * spawnTrigger.radius,
             EnemySpawnVerticalOffset, Random.insideUnitSphere.z * spawnTrigger.radius);

        GameObject tempEnemy = Instantiate(Obstacle, spawnPosition, Quaternion.identity) as GameObject;
        Destroy(tempEnemy, 10f);
        yield return null;
    }

    private IEnumerator SpawnEnemyLoop()
    {
        yield return SpawnEnemy();
        if (!GameManager.GameOver)
        {
            StartCoroutine(SpawnEnemyLoop());
            yield return null;
        }
    }
}
