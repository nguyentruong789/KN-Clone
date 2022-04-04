using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeReflect : KnifeBase
{
    private Knife otherKnife;
    public KnifeReflect(Knife mainKnife) : base(mainKnife) { }
    public override void EnterState()
    {
        base.EnterState();

        CalculateReflect(otherKnife.transform);
        ReflectRotate();

        ReturnToPool();
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public void SetOtherKnife(Knife _otherKnife)
    {
        otherKnife = _otherKnife;
    }

    private void CalculateReflect(Transform otherKnife)
    {
        if (otherKnife == null) return;

        Vector2 otherDir = otherKnife.transform.up;
        Vector2 newDir = Quaternion.Euler(0f, 0f, 135f * (otherDir.x < Mathf.Epsilon ? 1 : -1)) * otherDir;

        mainKnife.rb2d.velocity = newDir * 10f;
    }

    private void ReflectRotate()
    {
        mainKnife.StartCoroutine(ReflectRotate_Corou());
    }

    private IEnumerator ReflectRotate_Corou()
    {
        while (true)
        {
            mainKnife.transform.Rotate(Vector3.forward * 2000f * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

    private void ReturnToPool()
    {
        mainKnife.StartCoroutine(ReturnToPool_Corou());
    }

    private IEnumerator ReturnToPool_Corou()
    {
        float timer = 0f;
        while (timer < 2f)
        {
            timer += Time.deltaTime;
            if (timer >= 2f)
            {
                PoolManager.Instance.RetrieveObject(mainKnife.gameObject);
            }
            yield return new WaitForEndOfFrame();

        }
    }
}
