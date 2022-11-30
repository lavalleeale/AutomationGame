using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class UIItem : MonoBehaviour
{
    public ItemStack itemStack;
    public GameObject canvas,
        itemPrefab;
    public CanvasGroup cg;
    public TextMeshProUGUI amount;
    public Image image;
    public static UIItem itemBeingDragged;
    public bool CanDrag = true;
    public bool isBeingDragged = false;

    public void OnClick()
    {
        if (CanDrag)
        {
            var slot = transform.parent.GetComponent<Slot>();
            slot.Remove();
            if (UIItem.itemBeingDragged != null)
            {
                if (slot.Add())
                {
                    UIItem.itemBeingDragged.isBeingDragged = false;
                    UIItem.itemBeingDragged.cg.blocksRaycasts = true;
                    slot.Child = UIItem.itemBeingDragged.gameObject;
                }
                else
                {
                    slot.Add();
                    return;
                }
            }
            isBeingDragged = true;
            itemBeingDragged = this;
            cg.blocksRaycasts = false;
            transform.SetParent(canvas.transform);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        canvas = GameObject.FindGameObjectWithTag("canvas");
        amount.text = itemStack.amount.ToString();
        image.sprite = itemStack.item.sprite;
    }

    private void Update()
    {
        if (itemBeingDragged == this)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Invoke(nameof(Drop), 0.2f);
            }
            transform.position = Input.mousePosition;
        }
    }

    private void Drop()
    {
        if (itemBeingDragged == this)
        {
            itemBeingDragged = null;
            var item = Instantiate(itemPrefab);
            item.GetComponent<ItemController>().itemStack = itemStack;
            item.transform.position = Camera.main.transform.position + new Vector3(0, 0, 4);
            Destroy(gameObject);
        }
    }
}
