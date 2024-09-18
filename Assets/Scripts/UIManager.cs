using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Text _scoreText;

    [SerializeField]
    private Text _gameOverText;

    [SerializeField]
    private Text _pauseText;

    [SerializeField]
    private Text _restartText;

    [SerializeField]
    private Text _mainMenuText;

    [SerializeField]
    private Sprite[] _liveSprites;
    [SerializeField]
    private Image _livesImage;
    [SerializeField]
    private Image _livesImage2;     // FOR SECOND PLAYER ON THE LEFT

    [SerializeField]
    private float _fadespeed = 0.01f;
    private float _pauseFadeSpeed = 0.01f;

    private GameManager _gameManager;

    private bool _everySecondLoop;

    private int _livesNum = 3;
    [SerializeField]
    private int _score = 0;

    [SerializeField]
    private AudioSource _backgroundAudioSource;


    // Start is called before the first frame update
    void Start()
    {
        _score = 0;
        _scoreText.text = "SCORE: " + 0;
        _gameOverText.gameObject.SetActive(false);
        _restartText.gameObject.SetActive(false);
        _mainMenuText.gameObject.SetActive(false);
        _pauseText.gameObject.SetActive(false);
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _everySecondLoop = false;
        _livesNum = 3;

        if (_gameManager == null)
        {
            Debug.LogError("The Game Manager is NULL");
        }
        if (_gameManager.IsCoopMode())
        {
            _livesNum = 6;
        }
        _backgroundAudioSource = GameObject.Find("Audio_Manager").transform.Find("Background").GetComponent<AudioSource>();
        if (_backgroundAudioSource == null)
        {
            Debug.LogError("Background AudioSource is NULL.");
        }
        PauseTextAnim();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateLives(int currentLives, bool _isRightPlayer)
    {
        _livesNum -= 1;
        if (_isRightPlayer)
        {
            _livesImage.sprite = _liveSprites[currentLives];
        }
        else
        {
            _livesImage2.sprite = _liveSprites[currentLives];
        }

        if (_livesNum < 1)
        {
            GameOverSequence();
            _gameManager.GameOver();
        }
    }
    void GameOverSequence()
    {
        _gameOverText.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true);
        _mainMenuText.gameObject.SetActive(true);
        StartCoroutine(GameOverTextFlicker());
    }

    IEnumerator GameOverTextFlicker()
    {
        while (true)
        {
            if (_everySecondLoop)
            {
                yield return new WaitForSeconds(0.65f);
                _everySecondLoop = false;
            }
            else
            {
                _everySecondLoop = true;
            }
            yield return new WaitForSeconds(0.05f);
            Color gameOverColor = _gameOverText.color;
            Color restartColor = _restartText.color;
            Color mainMenuColor = _mainMenuText.color;

            while (gameOverColor.a > 0)
            {
                gameOverColor.a = _gameOverText.color.a - _fadespeed;
                restartColor.a = _restartText.color.a - _fadespeed;
                mainMenuColor.a = _mainMenuText.color.a - _fadespeed;
                _gameOverText.color = gameOverColor;
                _restartText.color = restartColor;
                _mainMenuText.color = mainMenuColor;
                yield return new WaitForSeconds(0.02f);
            }
            gameOverColor.a = 1;
            restartColor.a = 1;
            mainMenuColor.a = 1;
            _gameOverText.color = gameOverColor;
            _restartText.color = restartColor;
            _mainMenuText.color = mainMenuColor;
        }
    }

    public void IncreaseScore(int points)
    {
        _score += points;
        _scoreText.text = "SCORE: " + _score.ToString();
    }

    public void ResumeGame()
    {
        _gameManager.DisablePausePanel();
        _backgroundAudioSource.Play();
        Time.timeScale = 1;
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    void PauseTextAnim()
    {
        _pauseText.gameObject.SetActive(true);
        StartCoroutine(PauseTextFlicker());
    }

    IEnumerator PauseTextFlicker()
    {
        while (Time.time < 10.0f)
        {
            Color pauseColor = _pauseText.color;

            while (pauseColor.a < 1)
            {
                pauseColor.a = _pauseText.color.a + _pauseFadeSpeed;
                _pauseText.color = pauseColor;
                yield return new WaitForSeconds(0.02f);
            }

            while (pauseColor.a > 0)
            {
                pauseColor.a = _pauseText.color.a - _pauseFadeSpeed;
                _pauseText.color = pauseColor;
                yield return new WaitForSeconds(0.02f);
            }
        }
    }
}
