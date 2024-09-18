using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3f;

    private UIManager _uiManager;
    private Animator _animator;
    private GameManager _gameManager;

    PolygonCollider2D _collider;
    bool _isDead;
    float _speedDecreaseSpeed = 0.001f;

    private AudioSource _explosionAudioSource;
    private AudioSource _laserAudioSource;

    [SerializeField]
    private GameObject _laserPrefab;
    private float _fireRate = 2.0f;
    private float _nextFire = 3.0f;
    private float _bulletOffset = -1.3f;

    private bool _playersAlive = true;

    // Start is called before the first frame update
    void Start()
    {
        _isDead = false;
        _speedDecreaseSpeed = 0f;
        _playersAlive = true;
        _collider = GetComponent<PolygonCollider2D>();
        _animator = GetComponent<Animator>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _explosionAudioSource = GameObject.Find("Audio_Manager").transform.Find("Explosion").GetComponent<AudioSource>();
        _laserAudioSource = GameObject.Find("Audio_Manager").transform.Find("LaserFire").GetComponent<AudioSource>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        if (_uiManager == null)
        {
            Debug.LogError("The UI Manager is NULL.");
        }
        if (_animator == null)
        {
            Debug.LogError("The Enemy Animator is NULL.");
        }
        if (_explosionAudioSource == null)
        {
            Debug.LogError("Explosion AudioSource is NULL.");
        }
        if (_laserAudioSource == null)
        {
            Debug.LogError("Laser AudioSource is NULL.");
        }
        if (_gameManager == null)
        {
            Debug.LogError("The Game Manager is NULL.");
        }
        _speed = _gameManager.GetEnemySpeed();
        _fireRate = Random.Range(0f, 3f);
        _nextFire = Time.time + _fireRate;
    }

    // Update is called once per frame
    void Update()
    {
        //move down at 4m per sec
        UpdateMovement();
        // if bottom respawn at top w new random x position
        if ( (transform.position.y < -5.9f )  && (!_isDead))
        {
            if (_playersAlive)
            {
                float randX = Random.Range(-8.0f, 8.0f);
                transform.position = (new Vector3(randX, 7.8f, 0));
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
        if (Time.time >= _nextFire && _isDead == false)
        {
            ShootLaser();
        }
    }

    void UpdateMovement()
    {
        UpdateSpeed();
        transform.Translate(new Vector3(0, -1, 0) * Time.deltaTime * _speed);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // other gives the object, transform gives its root
        // Debug.Log("Collided with: " + other.transform.name);
        if ( other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();
            if (player != null) // null check player component
            {
                player.Damage();
            }
            EnemyDeath();
        }

        if (other.tag == "Laser")
        {
            Laser laser = other.transform.GetComponent<Laser>();
            if ( laser != null)
            {
                if (laser.IsEnemyBullet() == false)
                {
                    Destroy(other.gameObject);
                    if (_uiManager != null) // null check player component
                    {
                        _uiManager.IncreaseScore(5);
                    }
                    EnemyDeath();
                }
            }
        }
    }
    private void EnemyDeath()
    {
        _explosionAudioSource.Play();
        _isDead = true;
        _collider.enabled = false;

        _animator.SetTrigger("OnEnemyDeath");
        Destroy(this.gameObject, 2.37f);
    }

    void UpdateSpeed()
    {
        if (_isDead)
        {
            _speedDecreaseSpeed += 0.001f;  // slow down behavior
            _speed -= _speedDecreaseSpeed;
            if (_speed < 0f)
            {
                _speed = 0f;
            }
        }
    }

    void ShootLaser()
    {
        _fireRate = Random.Range(1.5f, 6.0f);
        _nextFire = Time.time + _fireRate;
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + _bulletOffset, transform.position.z);
        GameObject enemyLaser = Instantiate(_laserPrefab, pos, Quaternion.identity);
        enemyLaser.GetComponent<Laser>().MakeEnemyLaser();
        if (_laserAudioSource != null)
        {
            _laserAudioSource.Play();
        }
    }
}
