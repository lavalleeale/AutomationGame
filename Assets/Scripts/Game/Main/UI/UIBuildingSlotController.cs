using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using TMPro;

public class UIBuildingSlotController : MonoBehaviour
{
    public TextMeshProUGUI display;
    public string displayText;

    void Start()
    {
        display.text = displayText;
    }
}
