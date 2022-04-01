using UnityEngine;
using Dispatcher;
public class KnifeLauch : KnifeBase
{
    public float upVelocity = 25f;
    public KnifeLauch(Knife mainKnife) : base(mainKnife) { }
    public override void EnterState()
    {
        base.EnterState();
        GameManager.Instance.numShotted++;
        mainKnife.rb2d.velocity = new Vector2(0f, upVelocity);
        mainKnife.trail.gameObject.SetActive(true);

        AudioSource.PlayClipAtPoint(mainKnife.whoops, Camera.main.transform.position);
        MessageSender.SendMessage(MessageType.OnUpdateUI);
    }

    public override void ExitState()
    {
        throw new System.NotImplementedException();
    }
}
