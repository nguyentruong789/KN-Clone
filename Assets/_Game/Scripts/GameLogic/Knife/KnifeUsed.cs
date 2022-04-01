using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeUsed : KnifeBase
{
    public KnifeUsed(Knife mainKnife) : base(mainKnife) { }
    public override void EnterState()
    {
        base.EnterState();
        mainKnife.rb2d.velocity = Vector2.zero;
    }

    public override void ExitState()
    {
        throw new System.NotImplementedException();
    }
}
