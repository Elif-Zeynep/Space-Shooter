using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField]
    private float _powerupID;   // 0: triple shot, 1: speed, 2: shield
    [SerializeField]
    private float _speed = 3.0f;
    private float _lowerbound = -6.0f;

    [SerializeField]
    private AudioSource _powerupAudioSource;
    // Start is called before the first frame update

    void Start()
    {
        _powerupAudioSource = GameObject.Find("Audio_Manager").transform.Find("Powerup").GetComponent<AudioSource>();
        if (_powerupAudioSource == null)
        {
            Debug.LogError("Powerup AudioSource is NULL.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMovement();
    }
    void UpdateMovement()
    {
        transform.Translate(new Vector3(0, (-1) * _speed, 0) * Time.deltaTime);
        if (transform.position.y < _lowerbound)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            _powerupAudioSource.Play();
            Player player = other.transform.GetComponent<Player>();
            if (player != null) // null check player component
            {
                if (_powerupID == 0)
                {
                    player.TripleShotActivate();
                }
                else if (_powerupID == 1)
                {
                    player.SpeedActivate();
                }
                else if (_powerupID == 2)
                {
                    player.ShieldActivate();
                }
            }
            Destroy(this.gameObject);
        }
    }

}