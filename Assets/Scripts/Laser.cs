using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _speed = 8.0f;
    private float _laserUpperLimit = 8.0f;
    private float _laserLowerLimit = -6.0f;
    private float _movement = 1; // player laser = 1, enemy laser = -1

    private GameManager _gameManager;

    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (_gameManager == null)
        {
            Debug.LogError("The Game Manager is NULL.");
        }
        _speed = _gameManager.GetLaserSpeed();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMovement();
    }

    void UpdateMovement()
    {
        transform.Translate(new Vector3(0, _movement, 0) * Time.deltaTime * _speed);
        if ((transform.position.y > _laserUpperLimit) || (transform.position.y < _laserLowerLimit))
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
            Destroy(this.gameObject);
        }

    }

    public void MakeEnemyLaser()
    {
        _movement = -1;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && _movement < 0)
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.Damage();
            }
        }
    }

    public bool IsEnemyBullet()
    {
        if (_movement < 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}