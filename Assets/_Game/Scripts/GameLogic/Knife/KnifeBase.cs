using UnityEngine;
using System;
public abstract class KnifeBase
{
    public Knife mainKnife;
    public KnifeBase(Knife _mainKnife)
    {
        mainKnife = _mainKnife;
    }

    public Action OnEnter;
    public Action OnExit;

    public virtual void EnterState()
    {
        OnEnter?.Invoke();
        mainKnife.trail.gameObject.SetActive(false);
    }
    public virtual void ExitState()
    {
        OnExit?.Invoke();
    }

    public virtual void OnUpdate() { }
    public virtual void OnLaunch() { }
    public virtual void OnReload() { }
}
