using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameObject gameControllerGO;

    public GameSceneUIController gameSceneUIController;
    public GameSceneController gameSceneController;

    // SETTINGS
    private int dogCount, foodCount;

    public Animator crossfade;
    private bool won;

    private Player player;

    private int mazeColumns, mazeRows;

    private int currentLevel = 1;

    private void Awake()
    {
        if (gameControllerGO != null)
        {
            Destroy(this.gameObject);
            return;
        }

        gameControllerGO = this.gameObject;
        DontDestroyOnLoad(gameControllerGO);

    }

    public static GameController GetInstance()
    {
        return gameControllerGO.GetComponent<GameController>();
    }

    public int GetFoodCount()
    {
        return foodCount;
    }

    public int GetDogCount()
    {
        return dogCount;
    }

    public void SetPlayer(Player player)
    {
        this.player = player;
    }

    public Player GetPlayer()
    {
        return player;
    }

    public bool GetWon()
    {
        return won;
    }

    public int GetMazeRows()
    {
        return mazeRows;
    }

    public int GetMazeColumns()
    {
        return mazeColumns;
    }

    public void StartGame()
    {
        won = false;
        if (currentLevel < 6)
        {
            mazeColumns = 6 + currentLevel * 2;
            mazeRows = 6 + currentLevel * 2;
        } else {
            mazeColumns = 16;
            mazeRows = 16;
        }
        if (currentLevel < 5)
            foodCount = currentLevel;
        else
            foodCount = 4;

        dogCount = currentLevel;

        StartCoroutine(LoadLevel(1));
    }

    private IEnumerator LoadLevel(int i)
    {
        crossfade.SetTrigger("Start");

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(i);

        crossfade.SetTrigger("End");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public int GetLevel()
    {
        return currentLevel;
    }

    public void LoadScene(int i)
    {
        StartCoroutine(LoadLevel(i));
    }

    public void Win()
    {
        Debug.Log("Won!");
        won = true;
        LoadScene(2);
    }

    public void Lose()
    {
        won = false;
        currentLevel = 1;
        LoadScene(2);
    }

    public void NextLevel()
    {
        currentLevel++;
    }

    public void ResetLevel()
    {
        currentLevel = 1;
    }




}
