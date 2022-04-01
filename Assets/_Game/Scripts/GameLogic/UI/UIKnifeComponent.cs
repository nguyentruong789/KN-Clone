using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIKnifeComponent : MonoBehaviour
{
    public Image img;

    public void Show(bool value)
    {
        var color = value ? Color.white : new Color(1, 1, 1, 0.5f);
        img.color = color;
    }
}
