using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class UIItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public ItemStack itemStack;
    public GameObject canvas;
    CanvasGroup cg;
    TextMeshProUGUI count;
    Vector3 startPos;
    public static GameObject itemBeingDragged;
    public bool CanDrag = true;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (CanDrag)
        {
            startPos = transform.position;
            itemBeingDragged = gameObject;
            cg.blocksRaycasts = false;
            transform.SetParent(canvas.transform);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (CanDrag)
        {
            transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (CanDrag)
        {
            cg.blocksRaycasts = true;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        canvas = GameObject.Find("Canvas");
        cg = GetComponent<CanvasGroup>();
        GetComponentInChildren<TextMeshProUGUI>().text = itemStack.amount.ToString();
        GetComponent<Image>().sprite = itemStack.item.sprite;
    }
}
