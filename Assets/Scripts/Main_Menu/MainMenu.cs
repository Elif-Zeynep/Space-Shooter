using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadSinglePlayerGame()
    {
        SceneManager.LoadScene(1);
        Debug.Log("Single Player Mode Loading...");
    }
    public void LoadCoOpGame()
    {
        SceneManager.LoadScene(2);
        Debug.Log("Co-Op Mode Loading...");
    }
}
