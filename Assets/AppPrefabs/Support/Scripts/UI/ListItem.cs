using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Generic item behaviour that sets a text and offers click handler
/// </summary>
public class ListItem : MonoBehaviour
{
    public Text textItem;
    public EventHandler<object> ItemClicked;
    private object _data;

    public void Setup(string label, object data)
    {
        _data = data;
        if (textItem != null)
        {
            textItem.text = label;
        }
    }

    public void Click()
    {
        ItemClicked?.Invoke(this, _data);
    }
}
