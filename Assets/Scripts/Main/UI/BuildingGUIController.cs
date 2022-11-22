using UnityEngine;
using System.Collections;
using TMPro;

public class BuildingGUIController : MonoBehaviour
{
    public Canvas canvas;
    public TextMeshProUGUI nameText;
    public GameObject input,
        output,
        slotPrefab,
        itemPrefab;
    public RectTransform processingForeground;
    BuildingBehaviour building;
    Slot[] inputSlots;
    Slot outputSlot;

    void Start()
    {
        transform.SetParent(GameObject.Find("Canvas").transform, false);
    }

    public void Initialize(
        string name,
        int inputCount,
        bool needsFuel,
        BuildingBehaviour building
    )
    {
        nameText.text = name;
        inputSlots = new Slot[inputCount];
        for (int i = 0; i < inputCount; i++)
        {
            var slot = Instantiate(slotPrefab);
            slot.transform.SetParent(input.transform);
            inputSlots[i] = slot.GetComponent<Slot>();
            inputSlots[i].Initialize(SlotType.input, i, this);
        }

        var outputSlot = Instantiate(slotPrefab);
        outputSlot.transform.SetParent(output.transform);
        this.outputSlot = outputSlot.GetComponent<Slot>();
        this.outputSlot.Initialize(SlotType.output, 0, this);
        // TODO fuel
        this.building = building;
    }

    public enum SlotType
    {
        input,
        output
    }

    public void SetSlot(SlotType slotType, int slotNum, ItemStack itemStack)
    {
        if (slotType == SlotType.input)
        {
            if (inputSlots[slotNum].Child)
            {
                Destroy(inputSlots[slotNum].Child);
            }
            if (itemStack != null)
            {
                var item = Instantiate(itemPrefab);
                item.GetComponent<UIItem>().itemStack = itemStack;
                inputSlots[slotNum].Child = item;
            }
        }
        else
        {
            if (outputSlot.Child)
            {
                Destroy(outputSlot.Child);
            }

            if (itemStack != null)
            {
                var item = Instantiate(itemPrefab);
                item.GetComponent<UIItem>().itemStack = itemStack;
                outputSlot.Child = item;
            }
        }
    }

    public void UpdateProgress(float amount)
    {
        processingForeground.offsetMax = new Vector2(
            -(1 - amount) * 190,
            processingForeground.offsetMax.y
        );
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Destroy(gameObject);
        }
    }
}
