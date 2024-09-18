using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private bool _isLeftPlayer = false;
    [SerializeField]
    private bool _isRightPlayer = false;

    private float _horizontalInput;

    private bool _rightInput;
    private bool _leftInput;
    private bool _upInput;
    private bool _downInput;

    [SerializeField]
    private float _speed = 6f;
    private float _upperbound = 2.3f;
    private float _bottombound = -3.8f;
    private float _leftbound = -11.4f;
    private float _rightbound = 11.4f;
    private float _bulletOffset = 1.05f;
    [SerializeField]
    private int _lives = 3;

    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleShotPrefab;

    [SerializeField]
    private float _fireRate = 0.15f;
    private float _nextFire = 0.0f;

    private bool _tripleShotActivated = false;
    private float _tripleShotCoolDownTime = 7.0f;

    private bool _speedActivated = false;
    [SerializeField]
    private float _speedIncrease = 3.0f;
    private float _speedIncreaseTemp = 0f;
    private float _speedCoolDownTime = 7.0f;

    private bool _shieldActivated = false;
    private float _shieldCoolDownTime = 12.0f;

    [SerializeField]
    private GameObject _shieldVisualizer;

    [SerializeField]
    private GameObject _rightEngine;
    [SerializeField]
    private GameObject _leftEngine;

    private SpawnManager _spawnManager;
    private UIManager _uiManager;
    private GameManager _gameManager;
    [SerializeField]
    private AudioClip _laserAudioClip;
    private AudioSource _AudioSource;
    private AudioSource _explosionAudioSource;

    // in case of getting the same powerup multiple times, to not turn it off
    private int _tripleShotNum;
    private int _speedNum;
    private int _shieldNum;

    [SerializeField]
    Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();   // can also find by tag
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _explosionAudioSource = GameObject.Find("Audio_Manager").transform.Find("Explosion").GetComponent<AudioSource>();
        _tripleShotNum = _speedNum = _shieldNum = 0;

        _rightEngine.SetActive(false);
        _leftEngine.SetActive(false);
        _AudioSource = GetComponent<AudioSource>();

        if (_explosionAudioSource == null)
        {
            Debug.LogError("Explosion AudioSource is NULL.");
        }

        if (_spawnManager == null)
        {
            Debug.LogError("The Spawn Manager is NULL.");
        }

        if (_uiManager == null)
        {
            Debug.LogError("The UI Manager is NULL.");
        }

        if (_gameManager == null)
        {
            Debug.LogError("The Game Manager is NULL.");
        }

        if (_AudioSource == null)
        {
            Debug.LogError("Player AudioSource is NULL.");
        }
        else
        {
            _AudioSource.clip = _laserAudioClip;
        }

        if ( _gameManager.IsCoopMode() == false)
        {
            transform.position = new Vector3(0, 0, 0);
        }
        _speed = _gameManager.GetPlayerSpeed();
    }

    // Update is called once per frame
    void Update()
    {
        _speed = _gameManager.GetPlayerSpeed();
        UpdateMovement();
        if (_isRightPlayer)
        {
            if ((Input.GetKeyDown(KeyCode.Space) ) && Time.time >= _nextFire)   // || Input.GetMouseButton(0)
            {
                ShootLaser();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Tab) && Time.time >= _nextFire)
            {
                ShootLaser();
            }
        }
        EngineDamage();
    }
    void UpdateMovement()
    {
        if (_isLeftPlayer)
        {
            _rightInput = Input.GetKey(KeyCode.D);
            _leftInput = Input.GetKey(KeyCode.A);
            _upInput = Input.GetKey(KeyCode.W);
            _downInput = Input.GetKey(KeyCode.S);
        }
        else
        {
            _rightInput = Input.GetKey(KeyCode.RightArrow);
            _leftInput = Input.GetKey(KeyCode.LeftArrow);
            _upInput = Input.GetKey(KeyCode.UpArrow);
            _downInput = Input.GetKey(KeyCode.DownArrow);
        }

        if (_speedActivated)
        {
            _speedIncreaseTemp = _speedIncrease;
        }
        else
        {
            _speedIncreaseTemp = 0f;
        }
        if (_rightInput)
        {
            transform.Translate(Vector3.right * Time.deltaTime * (_speed + _speedIncreaseTemp));
            if (_leftInput == false)
            {
                _horizontalInput = 1;
            }
        }
        if (_leftInput)
        {
            transform.Translate(Vector3.left * Time.deltaTime * (_speed + _speedIncreaseTemp));
            if (_rightInput == false)
            {
                _horizontalInput = -1;
            }
        }
        if ((_leftInput || _rightInput) == false)
        {
            _horizontalInput = 0;
        }
        _animator.SetFloat("Direction", _horizontalInput);
        if (_upInput)
        {
            transform.Translate(Vector3.up * Time.deltaTime * (_speed + _speedIncreaseTemp));
        }
        if (_downInput)
        {
            transform.Translate(Vector3.down * Time.deltaTime * (_speed + _speedIncreaseTemp));
        }

        if (transform.position.y >= _upperbound)
        {
            transform.position = new Vector3(transform.position.x, _upperbound, 0);
        }
        else if (transform.position.y <= _bottombound)
        {
            transform.position = new Vector3(transform.position.x, _bottombound, 0);
        }
        if (transform.position.x < _leftbound)
        {
            transform.position = new Vector3(_rightbound, transform.position.y, 0);
        }
        else if (transform.position.x > _rightbound)
        {
            transform.position = new Vector3(_leftbound, transform.position.y, 0);
        }
    }
    void ShootLaser()
    {
        _nextFire = Time.time + _fireRate;
        if (_tripleShotActivated)
        {
            Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Vector3 pos = new Vector3(transform.position.x, transform.position.y + _bulletOffset, transform.position.z);
            Instantiate(_laserPrefab, pos, Quaternion.identity);
        }
        _AudioSource.Play();
    }

    public void Damage()
    {
        if (_shieldActivated)
        {
            _shieldActivated = false;
            _shieldVisualizer.SetActive(false);
            _shieldNum -= 1;
            return;
        }

        _lives -= 1;
        _uiManager.UpdateLives(_lives, _isRightPlayer);

        if ( _lives < 1)
        {
            _explosionAudioSource.Play();
            Destroy(this.gameObject);
        } 
    }
    public void TripleShotActivate()
    {
        _tripleShotNum += 1;
        _tripleShotActivated = true;
        StartCoroutine(TripleShotCoolDown());
    }

    IEnumerator TripleShotCoolDown()
    {
        _tripleShotNum -= 1;
        yield return new WaitForSeconds(_tripleShotCoolDownTime);
        if ( _tripleShotNum < 1)
        {
            _tripleShotActivated = false;
        }
    }

    public void SpeedActivate()
    {
        _speedNum += 1;
        _speedActivated = true;
        StartCoroutine(SpeedCoolDown());
    }

    IEnumerator SpeedCoolDown()
    {
        _speedNum -= 1;
        yield return new WaitForSeconds(_speedCoolDownTime);
        if (_speedNum < 1)
        {
            _speedActivated = false;
        }
    }

    public void ShieldActivate()
    {
        _shieldNum += 1;
        _shieldActivated = true;
        _shieldVisualizer.SetActive(true);
        StartCoroutine(ShieldCoolDown());
    }

    IEnumerator ShieldCoolDown()
    {
        yield return new WaitForSeconds(_shieldCoolDownTime);
        _shieldNum -= 1;
        if (_shieldNum < 1)
        {
            _shieldActivated = false;
            _shieldVisualizer.SetActive(false);
        }
    }

    void EngineDamage()
    {
        if (_lives < 3)
        {
            _rightEngine.SetActive(true);
            if (_lives < 2)
            {
                _leftEngine.SetActive(true);
            }
            else
            {
                _leftEngine.SetActive(false);
            }
        }
        else
        {
            _rightEngine.SetActive(false);
        }
    }
}
