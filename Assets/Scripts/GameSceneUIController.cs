using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameSceneUIController : MonoBehaviour
{

    public RectTransform menuButton, restartButton, items, joystick;

    public Button scissorsButton, trapButton, vacuumButton;

    public Camera mainCamera;

    public float padding;


    private int mazeColumns;
    private int mazeRows;

    private GameController gameController;

    private Vector2 screenSize;
    private float aspectRatio;

    private Player player;

    private void Awake()
    {
        gameController = GameController.GetInstance();
        gameController.gameSceneUIController = this;
        mazeColumns = gameController.GetMazeColumns();
        mazeRows = gameController.GetMazeRows();

        SetHasScissors(false);
        SetHasTrap(false);
        SetHasVacuum(false);
        UpdateLayout();
    }

    void Update()
    {
        if (Screen.width != screenSize.x || Screen.height != screenSize.y)
        {
            UpdateLayout();
        }
    }

    public void SetPlayer(Player player)
    {
        this.player = player;
    }


    private void UpdateLayout()
    {
        screenSize = new Vector2(Screen.width, Screen.height);
        aspectRatio = screenSize.x / screenSize.y;


        if (Screen.width <= Screen.height)
        {
            mainCamera.DOOrthoSize((mazeColumns + padding) * screenSize.y / screenSize.x * 0.5f, 0.5f);
            mainCamera.transform.DOMoveY(-.2f * mazeRows, 0.25f);
            mainCamera.transform.DOMoveX(0, 0.25f);

            menuButton.anchorMax = Vector2.up;
            menuButton.anchorMin = Vector2.up;
            menuButton.pivot = Vector2.up;
            menuButton.DOSizeDelta(new Vector2(.45f * screenSize.x, .1f * screenSize.y), 0.5f);

            restartButton.DOAnchorPosY(0, 0.5f);
            restartButton.DOSizeDelta(new Vector2(.45f * screenSize.x, .1f * screenSize.y), 0.5f);

            items.DOSizeDelta(new Vector2(.25f * screenSize.x, .3f * screenSize.y), 0.5f);

            joystick.DOSizeDelta(new Vector2(.25f * screenSize.y, .25f * screenSize.y), 0.5f);

        }
        else
        {
            mainCamera.DOOrthoSize((mazeRows + padding) / 2, 0.5f);
            mainCamera.transform.DOMoveY(0, 0.25f);
            mainCamera.transform.DOMoveX(1 + .05f * mazeColumns, 0.25f);

            menuButton.anchorMax = Vector2.one;
            menuButton.anchorMin = Vector2.one;
            menuButton.pivot = Vector2.one;
            menuButton.DOSizeDelta(new Vector2(.2f * screenSize.x, .2f * screenSize.y), 0.5f);

            restartButton.DOAnchorPosY(-.2f * screenSize.y, 0.5f);
            restartButton.DOSizeDelta(new Vector2(.25f * screenSize.x, .2f * screenSize.y), 0.5f);

            items.DOSizeDelta(new Vector2(.15f * screenSize.x, .6f * screenSize.y), 0.5f);

            joystick.DOSizeDelta(new Vector2(.25f * screenSize.x, .25f * screenSize.x), 0.5f);

        }
        Canvas.ForceUpdateCanvases();
    }


    public void RestartButtonOnClick()
    {
        gameController.LoadScene(1);
    }

    public void MainMenu()
    {
        gameController.LoadScene(0);
    }

    public void Die()
    {
        gameController.LoadScene(0);
    }

    public void SetHasScissors(bool value)
    {
        if (value)
            scissorsButton.GetComponent<Image>().color = Color.green;
        else
            scissorsButton.GetComponent<Image>().color = Color.yellow;
    }

    public void SetHasTrap(bool value)
    {
        if (value)
            trapButton.GetComponent<Image>().color = Color.green;
        else
            trapButton.GetComponent<Image>().color = Color.yellow;
    }

    public void SetHasVacuum(bool value)
    {
        if (value)
            vacuumButton.GetComponent<Image>().color = Color.green;
        else
            vacuumButton.GetComponent<Image>().color = Color.yellow;
    }

    // Player Controls

    public void ScissorsButtonOnClick()
    {
        gameController.GetPlayer().UseScissors();
    }

    public void TrapButtonOnClick()
    {
        gameController.GetPlayer().UseTrap();
    }

    public void VacuumButtonOnClick()
    {
        gameController.GetPlayer().UseVacuum();
    }
}
