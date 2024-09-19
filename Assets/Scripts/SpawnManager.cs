using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private float _enemySpawnRate = 5f;
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private GameObject[] powerups;

    private bool _stopSpawning = false;
    private GameManager _gameManager;

    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (_gameManager == null)
        {
            Debug.LogError("The Game Manager is NULL");
        }

    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
        _gameManager.StartUpdateSpeed();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // spawn game objects every 5 seconds
    // create a coroutine of type IEnumerator
    IEnumerator SpawnEnemyRoutine()
    {
        yield return new WaitForSeconds(2.8f);
        while (_stopSpawning == false)
        {
            float randX = Random.Range(-8f, 8f);
            Vector3 pos = new Vector3(randX, 8, 0);

            GameObject newEnemy = Instantiate(_enemyPrefab, pos, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(_enemySpawnRate + (Random.Range(-2.5f, 2.3f)));
        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(3f);
        while (_stopSpawning == false)
        {
            Vector3 pos = new Vector3(Random.Range(-8f, 8f), 8, 0);
            int randPowerup = Random.Range(0,3);
            Instantiate(powerups[randPowerup], pos, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(6f, 10f));  // spawn every 6-10 seconds randomly
        }
    }

    public void PlayersDied()
    {
        _stopSpawning = true;
    }
}
