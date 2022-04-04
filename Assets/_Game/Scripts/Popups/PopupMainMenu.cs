using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupMainMenu : UIPopupComponent
{
    public void OnClickedPlay()
    {
        GameManager.Instance.ChangeGamePhase(GamePhase.GAMEPLAY);
        this.ClosePopup();
    }
}
