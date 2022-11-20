using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public abstract class TooltipBehaviour : MonoBehaviour
{
    public GameObject tooltipPrefab;
    private GameObject canvas, tooltip;
    Sprite tooltipSprite;
    string tooltipName, tooltipInfo;

    public void InitializeTooltip(string name, string info, Sprite sprite)
    {
        this.tooltipInfo = info;
        this.tooltipName = name;
        this.tooltipSprite = sprite;
        canvas = GameObject.Find("Canvas");
    }

    public void UpdateTooltipInfo(string newInfo)
    {
        this.tooltipInfo = newInfo;
    }

    void OnMouseEnter()
    {
        tooltip = Instantiate(tooltipPrefab);
        tooltip.transform.SetParent(canvas.transform, false);

        var name = tooltip.transform.Find("Name").GetComponent<TextMeshProUGUI>();
        name.text = tooltipName;

        var info = tooltip.transform.Find("Info").GetComponent<TextMeshProUGUI>();
        info.text = tooltipInfo;

        var image = tooltip.transform.Find("Image").GetComponent<Image>();
        image.sprite = tooltipSprite;
    }

    void OnMouseExit()
    {
        Destroy(tooltip);
    }

    private void OnDestroy()
    {
        Destroy(tooltip);
    }
}

