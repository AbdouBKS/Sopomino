using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HoverManager : StaticInstance<HoverManager>
{
    [SerializeField] private TMP_Text hoverText;

    private void Start()
    {
        hoverText.text = "";
    }

    public void SetHoverText(string text)
    {
        hoverText.text = text;
    }
}
