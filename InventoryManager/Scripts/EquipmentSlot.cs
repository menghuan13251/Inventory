// --- START OF FILE EquipmentSlot.cs ---

using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class EquipmentSlot : Slot
{
    public Equipment.EquipmentType equipType;
    public Weapon.WeaponType wpType;

    // MODIFIED: The original OnPointerDown is heavily simplified because drag-and-drop handles most of it.
    public override void OnPointerDown(PointerEventData eventData)
    {
        
    }

    // ADDED: Override OnDrop to handle equipment logic.
    public override void OnDrop(PointerEventData eventData)
    {
        if (draggedItem == null) return;

        // Check if the dragged item is suitable for this slot
        if (IsRightItem(draggedItem.Item))
        {
            // If there's already an item here, swap it with the dragged one
            if (transform.childCount > 0)
            {
                ItemUI currentItem = transform.GetChild(0).GetComponent<ItemUI>();

                // Check if the current item can be placed in the original slot of the dragged item
                Slot originalSlot = originalParent.GetComponent<Slot>();
                if (originalSlot is EquipmentSlot && !((EquipmentSlot)originalSlot).IsRightItem(currentItem.Item))
                {
                    // Cannot swap if the item in the equipment slot cannot go into a regular slot (or another equipment slot)
                    // This logic can be expanded. For now, we'll allow the swap.
                }

                currentItem.transform.SetParent(originalParent);
                currentItem.transform.localPosition = Vector3.zero;
            }

            // Place the dragged item in this equipment slot
            draggedItem.transform.SetParent(transform);
            draggedItem.transform.localPosition = Vector3.zero;

            // Update player stats
            transform.parent.SendMessage("UpdatePropertyText");
        }
    }

    public bool IsRightItem(Item item)
    {
        if ((item is Equipment && ((Equipment)item).EquipType == this.equipType) ||
            (item is Weapon && ((Weapon)item).WpType == this.wpType))
        {
            return true;
        }
        return false;
    }
}