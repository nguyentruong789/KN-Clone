using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using CodeStringers.UIBase;

public class Knife : MonoBehaviour
{
    private PlayerInput playerInput => PlayerInput.Instance;
    private GameManager gameManager => GameManager.Instance;
    public Rigidbody2D rb2d;
    public BoxCollider2D boxCollider;
    public SpriteRenderer sprRenderer;
    public TrailRenderer trail;
    public AudioClip whoops;
    public AudioClip clash;

    #region States

    public KnifeBase currentState { get; private set; }
    public KnifeBase prevState { get; private set; }
    public KnifeBase ReadyState { get; private set; }
    public KnifeBase LaunchState { get; private set; }
    public KnifeBase UsedState { get; private set; }
    public KnifeBase reflectState { get; private set; }

    #endregion

    private void Awake()
    {
        ReadyState = new KnifeReady(this);
        LaunchState = new KnifeLauch(this);
        UsedState = new KnifeUsed(this);
        reflectState = new KnifeReflect(this);
    }

    private void OnEnable()
    {
        TransitionToState(ReadyState);

        playerInput.OnClickedSpace += Launch;
    }

    private void OnDisable()
    {
        playerInput.OnClickedSpace -= Launch;
    }

    public void Launch()
    {
        if (UIManager.AnyPopupShown()) return;
        if (gameManager.numShotted >= gameManager.NumShot) return;
        playerInput.OnClickedSpace -= Launch;

        TransitionToState(LaunchState);
    }

    public void TransitionToState(KnifeBase newState)
    {
        if (newState == null) return;
        prevState?.OnExit?.Invoke();

        currentState = newState;
        currentState.EnterState();
    }

    #region Unity callbacks

    private void OnTriggerEnter2D(Collider2D collider)
    {
        var otherKnife = collider.GetComponent<Knife>();
        if (otherKnife == null)
        {
            return;
        }

        //Todo: Handle lose
        if (otherKnife.currentState is KnifeUsed &&
            currentState is KnifeLauch)
        {
            StopAllCoroutines();
            ((KnifeReflect)reflectState).SetOtherKnife(otherKnife);
            TransitionToState(reflectState);
            AudioSource.PlayClipAtPoint(clash, Camera.main.transform.position);

            UIManager.ShowPopup(UIPopupName.PopupEndGame);
        }
    }

    #endregion
}
