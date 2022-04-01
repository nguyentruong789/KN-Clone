using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupNextLevel : UIPopupComponent
{
    private GameManager gameManager => GameManager.Instance;

    public void OnClickedNext()
    {
        gameManager.UpdateGameLevel(gameManager.Level + 1);
        gameManager.ResetGame();
        this.ClosePopup();
    }
}
