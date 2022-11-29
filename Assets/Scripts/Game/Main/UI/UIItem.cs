using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class UIItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public ItemStack itemStack;
    public GameObject canvas,
        itemPrefab;
    public CanvasGroup cg;
    public TextMeshProUGUI amount;
    public Image image;
    Vector3 startPos;
    public static UIItem itemBeingDragged;
    public bool CanDrag = true;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (CanDrag)
        {
            var slot = transform.parent.GetComponent<Slot>();
            startPos = transform.position;
            itemBeingDragged = this;
            cg.blocksRaycasts = false;
            transform.SetParent(canvas.transform);
            slot.Remove();
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
        if (transform.parent.CompareTag("canvas"))
        {
            var item = Instantiate(itemPrefab);
            item.GetComponent<ItemController>().itemStack = itemStack;
            item.transform.position = Camera.main.transform.position + new Vector3(0, 0, 4);
            Destroy(gameObject);
        }
        if (CanDrag)
        {
            cg.blocksRaycasts = true;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        canvas = GameObject.FindGameObjectWithTag("canvas");
        amount.text = itemStack.amount.ToString();
        image.sprite = itemStack.item.sprite;
    }
}
