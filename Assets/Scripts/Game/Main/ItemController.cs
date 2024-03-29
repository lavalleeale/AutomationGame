using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : TooltipBehaviour
{
    public ItemStack itemStack;
    bool blocked;
    BuildingBehaviour waitingForInput;
    string waitingForInputName;
    public Rigidbody2D rb;
    Vector3 targetMoveDir;
    LayerMask moveMask;
    LayerMask inputMask;

    public override string tooltipInfo => $"Amount: {Helpers.FormatNumber(itemStack.amount)}";

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = itemStack.item.sprite;
        moveMask = LayerMask.GetMask("items", "buildings");
        inputMask = LayerMask.GetMask("input");
        InitializeTooltip(itemStack.item.name, itemStack.item.sprite);
    }

    // Update is called once per frame
    //void Update()
    void FixedUpdate()
    {
        if (
            waitingForInput != null
            && waitingForInput.Input(itemStack, inputName: waitingForInputName)
        )
        {
            Destroy(gameObject);
        }

        if (blocked)
        {
            RaycastHit2D hit = Physics2D.Raycast(
                transform.position + targetMoveDir * 2,
                targetMoveDir,
                0.05f,
                moveMask
            );
            if (hit.collider == null)
            {
                blocked = false;
                rb.WakeUp();
            }
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out ConveyorController controller) && controller.Active)
        {
            var dir = controller.pushDir;
            RaycastHit2D input = Physics2D.Raycast(
                transform.position + dir * 2,
                dir,
                0.05f,
                inputMask
            );
            if (input.collider != null)
            {
                var building = input.transform.parent.GetComponent<BuildingBehaviour>();
                if (building.Input(itemStack, input.collider.name))
                {
                    Destroy(gameObject);
                    return;
                }
                else
                {
                    waitingForInput = building;
                    waitingForInputName = input.collider.name;
                    return;
                }
            }
            RaycastHit2D blocker = Physics2D.Raycast(
                transform.position + dir * 2,
                dir,
                0.05f,
                moveMask
            );
            if (blocker.collider == null)
            {
                transform.position += dir;
            }
            else
            {
                rb.Sleep();
                blocked = true;
                targetMoveDir = dir;
            }
        }
    }
}
