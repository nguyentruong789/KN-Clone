using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerInput : MonoSingleton<PlayerInput>
{
    public event Action OnClickedSpace;
    public event Action OnClickedR;

    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space))
        //     OnClickedSpace?.Invoke();

        // if (Input.GetKeyDown(KeyCode.R))
        //     OnClickedR?.Invoke();
    }

    public void InvokeOnClickedSpace()
    {
        OnClickedSpace?.Invoke();
    }
}
