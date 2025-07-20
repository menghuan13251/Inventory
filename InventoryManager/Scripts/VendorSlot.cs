// --- START OF FILE VendorSlot.cs ---

using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class VendorSlot : Slot
{

    // MODIFIED: Keep right-click to buy, but left-click/drag is handled by OnDrop.
    public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (transform.childCount > 0)
            {
                Item currentItem = transform.GetChild(0).GetComponent<ItemUI>().Item;
                // We assume this message is sent to the Vendor panel
                transform.parent.parent.SendMessage("BuyItem", currentItem);
            }
        }
    }

    // ADDED: Override OnDrop to handle selling items.
    public override void OnDrop(PointerEventData eventData)
    {
        if (draggedItem != null)
        {
            // Don't allow dropping items from the shop itself back into the shop
            if (originalParent.GetComponent<VendorSlot>() != null) return;

            // Send a message to the Vendor panel to sell the item
            // The item will be destroyed after selling.
            transform.parent.parent.SendMessage("SellItem", draggedItem);
        }
    }

    // REMOVED: These methods are no longer needed as drag-and-drop handles it
    // public override void OnBeginDrag(PointerEventData eventData) { }
    // public override void OnDrag(PointerEventData eventData) { }
    // public override void OnEndDrag(PointerEventData eventData) { }
    // We can also prevent dragging items from the vendor slot
    public override void OnBeginDrag(PointerEventData eventData)
    {
        // Prevent dragging items from the vendor slot.
        // If you want to allow picking them up, you would need more logic.
        // For a shop, it's usually better to prevent this.
    }
}