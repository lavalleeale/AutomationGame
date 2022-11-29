using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public abstract class TooltipBehaviour : MonoBehaviour
{
    public GameObject tooltipPrefab;
    private GameObject canvas,
        tooltip;
    Sprite tooltipSprite;
    string tooltipName;
    public abstract string tooltipInfo { get; }

    public void InitializeTooltip(string name, Sprite sprite)
    {
        this.tooltipName = name;
        this.tooltipSprite = sprite;
        canvas = GameObject.FindGameObjectWithTag("canvas");
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
