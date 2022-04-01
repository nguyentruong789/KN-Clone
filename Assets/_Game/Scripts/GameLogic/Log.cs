using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dispatcher;
using CodeStringers.UIBase;
using DG.Tweening;

public class Log : MonoBehaviour
{
    private GameManager gameManager => GameManager.Instance;

    private int Count;

    [SerializeField] private List<RotateSequence> Sequences;
    [SerializeField] ParticleSystem particle;
    [SerializeField] SpriteRenderer flashLog;
    [SerializeField] AudioClip hit;

    private void GenerateSequenceBasedOnLevel(int level)
    {
        Sequences = new List<RotateSequence>();
        for (int i = 0; i < level; i++)
        {
            var s = new RotateSequence();
            s.speed = Random.Range(-5f, 5f);
            if (s.speed < 0) s.speed -= 1f;
            if (s.speed >= 0) s.speed += 1f;
            s.duration = Random.Range(1f, 5f);

            Sequences.Add(s);
        }
    }

    private void Awake()
    {
        MessageDispatcher.AddListener(MessageType.OnChangeGamePhase, OnChangeGamePhase);
        MessageDispatcher.AddListener(MessageType.OnResetGame, OnResetGame);
    }

    private void OnDestroy()
    {
        MessageDispatcher.RemoveListener(MessageType.OnChangeGamePhase, OnChangeGamePhase);
        MessageDispatcher.AddListener(MessageType.OnResetGame, OnResetGame);
    }

    private void OnResetGame(IMessage message)
    {
        GenerateSequenceBasedOnLevel(gameManager.Level);
        StartRotate();
        Count = 0;
        flashLog.color = new Color(1, 1, 1, 0);
    }

    private void OnChangeGamePhase(IMessage rMessage)
    {
        var gamePhase = (GamePhase)rMessage.Data;

        switch (gamePhase)
        {
            case GamePhase.MENU:
                StopAllCoroutines();
                break;
            case GamePhase.GAMEPLAY:
                GenerateSequenceBasedOnLevel(gameManager.Level);
                StartRotate();
                Count = 0;
                break;
        }
    }

    private void StartRotate()
    {
        StartCoroutine(Rotate_Corou());
    }

    private IEnumerator Rotate_Corou()
    {
        while (true)
        {
            for (int i = 0; i < Sequences.Count; i++)
            {
                yield return RotateSequence_Corou(Sequences[i]);

                if (i == Sequences.Count - 1)
                    i = 0;
            }
        }
    }

    private IEnumerator RotateSequence_Corou(RotateSequence sequence)
    {
        float timer = 0;
        while (timer < sequence.duration)
        {
            transform.Rotate(Vector3.forward * (sequence.speed) * Time.deltaTime * 20f);
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    private void CheckForWinCondition()
    {
        if (Count >= gameManager.NumShot)
        {
            StopAllCoroutines();
            UIManager.ShowPopup(UIPopupName.PopupNextLevel);
        }
    }

    Tweener tween;
    private void ShakeEffect()
    {
        particle.Play();

        float value = 0;
        tween = DOTween.To(x => value = x, 0, 1f, 0.05f);
        tween.OnUpdate(() =>
        {
            transform.position = new Vector3(transform.position.x,
            Mathf.Lerp(2.5f, 2.55f, value), transform.position.z);
            flashLog.color = new Color(1, 1, 1, Mathf.Lerp(0, 0.5f, value));
        })
        .OnComplete(() =>
        {
            tween = DOTween.To(x => value = x, 1, 0f, 0.05f);
            tween.OnUpdate(() =>
            {
                transform.position = new Vector3(transform.position.x,
                Mathf.Lerp(2.5f, 2.55f, value), transform.position.z);
                flashLog.color = new Color(1, 1, 1, Mathf.Lerp(0, 0.5f, value));
            });
        });
    }

    #region Unity callbacks

    private void OnTriggerEnter2D(Collider2D collider)
    {
        var knife = collider.GetComponent<Knife>();
        if (knife == null) return;
        if (knife.currentState != knife.LaunchState) return;

        knife.transform.parent = this.transform;
        knife.TransitionToState(knife.UsedState);
        Count++;

        CheckForWinCondition();
        ShakeEffect();
        AudioSource.PlayClipAtPoint(hit, Camera.main.transform.position);
    }

    #endregion
}
