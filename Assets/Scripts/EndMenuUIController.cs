using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class EndMenuUIController : MonoBehaviour
{

    public RectTransform victory, defeat;
    public TextMeshProUGUI nextLevel;

    public Button playButton;

    private RectTransform currentMenu;

    private GameController gameController;

    private Vector2 screenSize;

    private void Awake()
    {
        gameController = GameController.GetInstance();
        if (gameController.GetWon())
            currentMenu = victory;
        else
            currentMenu = defeat;
        nextLevel.text = "Next level: " + (gameController.GetLevel() + 1);
        UpdateLayout();
    }

    void Update()
    {
        if (Screen.width != screenSize.x || Screen.height != screenSize.y)
        {
            UpdateLayout();
        }
    }


    private void UpdateLayout()
    {
        screenSize.x = Screen.width;
        screenSize.y = Screen.height;

        if (currentMenu == victory)
        {
            defeat.DOAnchorPos(new Vector2(screenSize.x + 800, 0), 0.25f);
            victory.DOAnchorPos(new Vector2(0, 0), 0.25f);
        }
        else if (currentMenu == defeat)
        {
            victory.DOAnchorPos(new Vector2(-screenSize.x - 800, 0), 0.25f);
            defeat.DOAnchorPos(new Vector2(0, 0), 0.25f);
        }
    }

    public void QuitButtonOnClick()
    {
        gameController.QuitGame();
    }

    public void NextLevel()
    {
        playButton.GetComponent<RectTransform>().DOScale(Vector3.zero, 0.5f);
        playButton.GetComponent<RectTransform>().DORotate(new Vector3(0, 0, 180f), 0.5f);
        gameController.NextLevel();
        gameController.StartGame();
    }
    
    public void MainMenu()
    {
        gameController.LoadScene(0);
    }

    public void StartOver()
    {
        gameController.ResetLevel();
        gameController.LoadScene(1);
    }
}
