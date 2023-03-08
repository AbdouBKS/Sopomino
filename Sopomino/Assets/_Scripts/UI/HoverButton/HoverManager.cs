using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HoverManager : StaticInstance<HoverManager>
{
    [SerializeField] private TMP_Text hoverText;
    [SerializeField] private GameObject hoverBorder;

    public string chooseTitle = "";
    public string chooseDescription = "";

    private void Start()
    {
        hoverText.text = "";
        hoverBorder.SetActive(false);
    }

    public void HoverEnter(string text)
    {
        hoverText.text = text;
        hoverBorder.SetActive(true);
    }

    public void HoverExit()
    {
        hoverText.text = chooseDescription;

        if (chooseDescription.Length <= 0)
            hoverBorder.SetActive(false);
    }
}
