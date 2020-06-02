using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenuSceneController : MonoBehaviour
{
    private GameController gameController;
    private void Awake(){
        gameController = GameController.gameControllerGO.GetComponent<GameController>();
    }
    public void GoBackButtonOnClick()
    {
        gameController.LoadScene(0);
    }
}
