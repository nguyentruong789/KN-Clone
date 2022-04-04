using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dispatcher;
using CodeStringers.UIBase;
using DG.Tweening;
using UniRx;

public class Log : MonoBehaviour
{
    private GameManager gameManager => GameManager.Instance;

    private int Count;

    [SerializeField] private List<RotateSequence> Sequences;
    [SerializeField] ParticleSystem particle;
    [SerializeField] SpriteRenderer flashLog;
    [SerializeField] AudioClip hit;
    [SerializeField] AudioClip crash;
    [SerializeField] Explodable explodable;
    [SerializeField] ExplosionForce ef;
    [SerializeField] SpriteRenderer sprRenderer;
    [SerializeField] CircleCollider2D circleCollider;

    private void GenerateSequenceBasedOnLevel(int level)
    {
        Sequences = new List<RotateSequence>();
        for (int i = 0; i < level; i++)
        {
            var s = new RotateSequence();
            s.speed = Random.Range(-5.5f, 5.5f);
            if (s.speed < 0) s.speed -= 2f;
            if (s.speed >= 0) s.speed += 2f;
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
        this.transform.localScale = Vector3.zero;
        Zoom(0.3f);

        sprRenderer.enabled = true;
        circleCollider.enabled = true;
        explodable.RetrieveFragments();
    }

    private void OnChangeGamePhase(IMessage rMessage)
    {
        var gamePhase = (GamePhase)rMessage.Data;

        switch (gamePhase)
        {
            case GamePhase.MENU:
                StopAllCoroutines();
                this.transform.localScale = Vector3.zero;
                break;
            case GamePhase.GAMEPLAY:
                OnResetGame(null);
                break;
        }
    }

    private void StartRotate()
    {
        StartCoroutine(Rotate_Corou());
    }

    private void Zoom(float endValue)
    {
        this.transform.DOScale(endValue, 0.4f);
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

            sprRenderer.enabled = false;
            circleCollider.enabled = false;
            explodable.Explode();
            ef.doExplosion(transform.position);
            AudioSource.PlayClipAtPoint(crash, Camera.main.transform.position);

            foreach (var knife in this.transform.GetComponentsInChildren<Knife>())
            {
                knife.transform.parent = null;
                ((KnifeReflect)knife.reflectState).SetOtherKnife(knife);
                knife.TransitionToState(knife.reflectState);
            }

            //Next game sequence
            Observable.Timer(System.TimeSpan.FromSeconds(1f)).Subscribe(_ =>
            {
                gameManager.UpdateGameLevel(gameManager.Level + 1);
                gameManager.ResetGame();
            }).AddTo(this);

            //UIManager.ShowPopup(UIPopupName.PopupNextLevel);
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
            Mathf.Lerp(2.5f, 2.7f, value), transform.position.z);
            flashLog.color = new Color(1, 1, 1, Mathf.Lerp(0, 0.5f, value));
        })
        .OnComplete(() =>
        {
            tween = DOTween.To(x => value = x, 1, 0f, 0.05f);
            tween.OnUpdate(() =>
            {
                transform.position = new Vector3(transform.position.x,
                Mathf.Lerp(2.5f, 2.7f, value), transform.position.z);
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
