using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIController : MonoBehaviour
{

    public RectTransform mainMenu, optionsMenu;

    public Button playButton;

    private RectTransform currentMenu;

    private GameController gameController;

    private Vector2 screenSize;

    private void Awake()
    {
        gameController = GameController.GetInstance();
        currentMenu = mainMenu;
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

        if (currentMenu == mainMenu)
        {
            optionsMenu.DOAnchorPos(new Vector2(screenSize.x + 800, 0), 0.25f);
            mainMenu.DOAnchorPos(new Vector2(0, 0), 0.25f);
        }
        else if (currentMenu == optionsMenu)
        {
            mainMenu.DOAnchorPos(new Vector2(-screenSize.x -800, 0), 0.25f);
            optionsMenu.DOAnchorPos(new Vector2(0, 0), 0.25f);
        }
    }

    public void PlayButtonOnClick()
    {
        playButton.GetComponent<RectTransform>().DOScale(Vector3.zero, 0.5f);
        playButton.GetComponent<RectTransform>().DORotate(new Vector3(0, 0, 180f), 0.5f);
        gameController.StartGame();
    }

    private IEnumerator Wait(float duration) {
        yield return new WaitForSeconds(duration);
        gameController.StartGame();
    }

    public void QuitButtonOnClick()
    {
        gameController.QuitGame();
    }

    public void MainMenu()
    {
        currentMenu = mainMenu;
        UpdateLayout();
    }

    public void OptionsMenu()
    {
        currentMenu = optionsMenu;
        UpdateLayout();
    }
}
