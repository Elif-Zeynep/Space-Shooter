using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField]
    private float _rotationSpeed = 19f;
    //[SerializeField]
    //private float _fallSpeed = 3f;

    [SerializeField]
    private GameObject _explosionPrefab;

    private SpawnManager _spawnManager;
    PolygonCollider2D _collider;

    private AudioSource _explosionAudioSource;

    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        transform.position = new Vector3(0, 4f, 0);
        _collider = GetComponent<PolygonCollider2D>();

        _explosionAudioSource = GameObject.Find("Audio_Manager").transform.Find("Explosion").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMovement();
    }

    void UpdateMovement()
    {
        //transform.Translate(new Vector3(0, -1, 0) * Time.deltaTime * _fallSpeed);
        transform.Rotate(new Vector3(0, 0, 1) * Time.deltaTime * _rotationSpeed);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser")
        {
            Destroy(other.gameObject);
            GameObject newExplosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            _explosionAudioSource.Play();
            Destroy(newExplosion, 2.7f);
            _collider.enabled = false;
            _spawnManager.StartSpawning();
            Destroy(this.gameObject, 0.2f);
        }
    }

    //check for laser collison
    //instantiate explosion & destroy after 3 seconds
}
