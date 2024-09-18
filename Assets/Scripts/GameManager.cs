using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private bool _isGameOver = false;
    [SerializeField]
    private bool _isCoopMode = false;

    [SerializeField]
    private GameObject _pauseMenuPanel;

    [SerializeField]
    private float _enemySpeed = 2.6f;
    [SerializeField]
    private float _playerSpeed = 6f;
    [SerializeField]
    private float _powerupSpeed = 3.0f;

    private SpawnManager _spawnManager;
    private AudioSource _backgroundAudioSource;

    private float _speedUpdateDelay = 0.25f;
    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _backgroundAudioSource = GameObject.Find("Audio_Manager").transform.Find("Background").GetComponent<AudioSource>();

        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is NULL.");
        }
        if (_backgroundAudioSource == null)
        {
            Debug.LogError("Background AudioSource is NULL.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_isGameOver)
        {
            //_isGameOver = false;
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (_isCoopMode)
                {
                    SceneManager.LoadScene(2);
                }
                else
                {
                    SceneManager.LoadScene(1);
                }
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                SceneManager.LoadScene(0);
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            _pauseMenuPanel.SetActive(true);
            Time.timeScale = 0;
            _backgroundAudioSource.Pause();
        }
    }

    public void GameOver()
    {
        _isGameOver = true;
        _spawnManager.PlayersDied();
    }

    public bool IsCoopMode()
    {
        return _isCoopMode;
    }

    public void DisablePausePanel()
    {
        _pauseMenuPanel.SetActive(false);
    }
    
    public void StartUpdateSpeed()
    {
        StartCoroutine(UpdateSpeed());
    }

    IEnumerator UpdateSpeed()
    {
        while (_isGameOver == false)
        {
            yield return new WaitForSeconds(4.75f + _speedUpdateDelay);
            _speedUpdateDelay += 0.25f;
            _enemySpeed += 0.12f;
            _playerSpeed += 0.12f;
            _powerupSpeed += 0.051f;
        }
    }

    public float GetEnemySpeed()
    {
        return _enemySpeed;
    }

    public float GetPlayerSpeed()
    {
        return _playerSpeed;
    }

    public float GetPowerupSpeed()
    {
        return _powerupSpeed;
    }
}
