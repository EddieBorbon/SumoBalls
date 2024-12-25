using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverPanel;
    public GameObject player;
    public GameObject titlePanel;
    public SpawnManager spawnManager;

    void Start(){
        titlePanel.SetActive(true);
    }

    public void StartGame(){
        player = GameObject.Find("Player");
        spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        titlePanel.SetActive(false);
        spawnManager.StarSpawnManager();  
    }

    void Update()
    {
        if (player.transform.position.y < -10)
        {
            GameOver();
        }
    }
    void GameOver()
    {
        gameOverPanel.SetActive(true);
    }
    public void Restart()
    {
        if (player.transform.position.y < -10)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
