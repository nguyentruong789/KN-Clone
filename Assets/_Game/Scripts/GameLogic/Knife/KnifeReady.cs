using UnityEngine;
using DG.Tweening;
public class KnifeReady : KnifeBase
{
    public KnifeReady(Knife mainKnife) : base(mainKnife) { }

    public override void EnterState()
    {
        base.EnterState();
        mainKnife.StopAllCoroutines();

        mainKnife.transform.position = new Vector3(0f, -3.5f, 0f);
        mainKnife.rb2d.velocity = Vector2.zero;
        mainKnife.transform.rotation = Quaternion.identity;

        OnReload();
    }

    public override void ExitState()
    {
        base.ExitState();
        tweenMove?.Kill();
        tweenFade?.Kill();
        mainKnife.sprRenderer.color = Color.white;
    }

    public override void OnReload()
    {
        TweenMove();
        TweenFade();
    }

    Tweener tweenMove;
    Tweener tweenFade;
    private void TweenMove()
    {
        float value = 0;
        tweenMove = DOTween.To(x => value = x, 0f, 1f, 0.15f);
        tweenMove.OnUpdate(() =>
        {
            mainKnife.transform.localPosition = new Vector3(0f, Mathf.Lerp(-4.5f, -3.5f, value), 0f);
        });
    }

    private void TweenFade()
    {
        float value = 0;
        tweenFade = DOTween.To(x => value = x, 0f, 1f, 0.15f);
        tweenFade.OnUpdate(() =>
        {
            mainKnife.sprRenderer.color = new Color(1f, 1f, 1f, value);
        });
    }
}
