using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Dispatcher;
using CodeStringers.UIBase;
using UniRx;

public class GameManager : MonoSingleton<GameManager>
{
    public int Level = 1;
    public int NumShot
    {
        get
        {
            return Mathf.Clamp(Level + 5, 0, 13);
        }
    }
    [HideInInspector] public int numShotted;

    public GamePhase CurrentPhase { get; private set; }

    private PlayerInput playerInput => PlayerInput.Instance;
    private PoolManager pool => PoolManager.Instance;

    private void Start()
    {
        Observable.TimerFrame(1).Subscribe(_ =>
        {
            ChangeGamePhase(GamePhase.MENU);
        }).AddTo(this);
        //ChangeGamePhase(GamePhase.MENU);
        pool.Register(GameDefine.KNIFE);


    }

    public void ResetGame()
    {
        pool.RetrieveAll(GameDefine.KNIFE);
        numShotted = 0;
        SpawnNewKnife();
        MessageSender.SendMessage(MessageType.OnResetGame);
    }

    private void SpawnNewKnife()
    {
        if (numShotted >= NumShot) return;

        var knife = pool.GetObject(GameDefine.KNIFE);
    }

    public void UpdateGameLevel(int level)
    {
        Level = level;
    }

    public void ChangeGamePhase(GamePhase phase)
    {
        if (CurrentPhase == phase) return;

        CurrentPhase = phase;

        MessageSender.SendMessage(this, MessageType.OnChangeGamePhase, phase, 0f);

        OnChangeGamePhase();
    }

    private void OnChangeGamePhase()
    {
        switch (CurrentPhase)
        {
            case GamePhase.GAMEPLAY:
                ResetGame();
                SpawnNewKnife();
                playerInput.OnClickedSpace += SpawnNewKnife;
                break;
            case GamePhase.MENU:
                UIManager.ShowPopup(UIPopupName.PopupMainMenu);
                playerInput.OnClickedSpace -= SpawnNewKnife;
                break;
        }
    }
}
