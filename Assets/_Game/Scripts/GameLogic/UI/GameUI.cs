using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeStringers.Utils;
using Dispatcher;
using UniRx;
using TMPro;

public class GameUI : MonoBehaviour
{
    [SerializeField] RectTransform pnlShot;
    [SerializeField] TextMeshProUGUI txtCurrentLevel;

    private PlayerInput playerInput => PlayerInput.Instance;
    private GameManager gameManager => GameManager.Instance;

    private void Start()
    {
        MessageDispatcher.AddListener(MessageType.OnResetGame, OnResetGame);
        MessageDispatcher.AddListener(MessageType.OnUpdateUI, OnUpdateUI);
    }

    private void OnDestroy()
    {
        MessageDispatcher.RemoveListener(MessageType.OnResetGame, OnResetGame);
        MessageDispatcher.RemoveListener(MessageType.OnUpdateUI, OnUpdateUI);
    }

    private void OnResetGame(IMessage message)
    {
        UpdateUI();
    }

    private void OnUpdateUI(IMessage m)
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        int idx = 0;
        for (int i = 0; i < gameManager.NumShot; i++)
        {
            var comp = i < pnlShot.childCount ?
            pnlShot.GetChild(i).GetComponent<UIKnifeComponent>() :
            CSGameObjectUtils.LoadGameObject(pnlShot, pnlShot.GetChild(0).gameObject).GetComponent<UIKnifeComponent>();

            comp.gameObject.SetActive(true);
            comp.Show(i >= gameManager.numShotted);

            idx++;
        }

        for (int i = idx; i < pnlShot.childCount; i++)
            pnlShot.GetChild(i).gameObject.SetActive(false);

        txtCurrentLevel.text = gameManager.Level.ToString();
    }
}
