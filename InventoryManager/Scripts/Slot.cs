// --- START OF FILE Slot.cs ---

using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

/// <summary>
/// 物品槽
/// </summary>
// MODIFIED: Added drag-and-drop interfaces
public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerClickHandler
{

    public GameObject itemPrefab;

    // ADDED: Static reference to the item being dragged.
    public static ItemUI draggedItem;
    // ADDED: To remember where the item came from.
    public static Transform originalParent;
    // ADDED: To handle canvas group for raycasting.
    private CanvasGroup canvasGroup;


    /// <summary>
    /// 把item放在自身下面
    /// 如果自身下面已经有item了，amount++
    /// 如果没有 根据itemPrefab去实例化一个item，放在下面
    /// </summary>
    /// <param name="item"></param>
    public void StoreItem(Item item)
    {
        if (transform.childCount == 0)
        {
            GameObject itemGameObject = Instantiate(itemPrefab) as GameObject;
            itemGameObject.transform.SetParent(this.transform);
            itemGameObject.transform.localScale = Vector3.one;
            itemGameObject.transform.localPosition = Vector3.zero;
            itemGameObject.GetComponent<ItemUI>().SetItem(item);
        }
        else
        {
            transform.GetChild(0).GetComponent<ItemUI>().AddAmount();
        }
    }


    /// <summary>
    /// 得到当前物品槽存储的物品类型
    /// </summary>
    /// <returns></returns>
    public Item.ItemType GetItemType()
    {
        return transform.GetChild(0).GetComponent<ItemUI>().Item.Type;
    }

    /// <summary>
    /// 得到物品的id
    /// </summary>
    /// <returns></returns>
    public int GetItemId()
    {
        return transform.GetChild(0).GetComponent<ItemUI>().Item.ID;
    }

    public bool IsFilled()
    {
        ItemUI itemUI = transform.GetChild(0).GetComponent<ItemUI>();
        return itemUI.Amount >= itemUI.Item.Capacity;//当前的数量大于等于容量
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (transform.childCount > 0)
        {
            //  InventoryManager.Instance.HideToolTip();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (transform.childCount > 0 && draggedItem == null) // MODIFIED: Only show tooltip if not dragging
        {
            //string toolTipText = transform.GetChild(0).GetComponent<ItemUI>().Item.GetToolTipText();
           // InventoryManager.Instance.ShowToolTip(toolTipText);
        }

    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        
    }
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        // 只响应左键单击
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (transform.childCount > 0)
            {
                // ItemUI itemUI = transform.GetChild(0).GetComponent<ItemUI>();
                // InventoryManager.Instance.ShowContextMenu(itemUI);
                string toolTipText = transform.GetChild(0).GetComponent<ItemUI>().Item.GetToolTipText();
                InventoryManager.Instance.ShowToolTip(toolTipText);
            }
        }
    }
    // --- ADDED: DRAG AND DROP IMPLEMENTATION ---

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        // Can only drag with left mouse button and if there's an item in the slot
        if (eventData.button == PointerEventData.InputButton.Left && transform.childCount > 0)
        {
            draggedItem = transform.GetChild(0).GetComponent<ItemUI>();
            originalParent = transform;

            // Move the item to the top-level canvas to render it above all other UI
            draggedItem.transform.SetParent(transform.root);
            draggedItem.transform.SetAsLastSibling(); // Render on top

            // Get the CanvasGroup and disable 'blocksRaycasts'
            // This allows the OnDrop event to be triggered on the slot underneath the item.
            canvasGroup = draggedItem.GetComponent<CanvasGroup>();
            canvasGroup.blocksRaycasts = false;

            InventoryManager.Instance.HideToolTip(); // Hide tooltip while dragging
          
        }
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (draggedItem != null)
        {
            // Make the item follow the mouse
            draggedItem.transform.position = eventData.position;
        }
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        if (draggedItem != null)
        {
            // Re-enable raycasting on the item
            canvasGroup.blocksRaycasts = true;

            // If the item was not dropped on a valid slot, return it to its original position.
            // We check this by seeing if its parent is still the root canvas.
            if (draggedItem.transform.parent == transform.root)
            {
                draggedItem.transform.SetParent(originalParent);
                draggedItem.transform.localPosition = Vector3.zero;
            }

            // 【新增】在拖拽结束后，刷新涉及到的背包UI
            Inventory sourceInventory = originalParent.GetComponentInParent<Inventory>();
            if (sourceInventory != null)
            {
                sourceInventory.RefreshUI();
            }

            // 如果物品被移动到了新的背包
            if (draggedItem.transform.parent != originalParent)
            {
                Inventory destinationInventory = draggedItem.transform.GetComponentInParent<Inventory>();
                if (destinationInventory != null && destinationInventory != sourceInventory)
                {
                    destinationInventory.RefreshUI();
                }
            }

            // 重置静态变量
            draggedItem = null;
            originalParent = null;
            canvasGroup = null;
        }
    }

    public virtual void OnDrop(PointerEventData eventData)
    {
        if (draggedItem == null) return;

        // If this slot is empty
        if (transform.childCount == 0)
        {
            draggedItem.transform.SetParent(transform);
            draggedItem.transform.localPosition = Vector3.zero;
        }
        // If this slot already has an item
        else
        {
            ItemUI currentItem = transform.GetChild(0).GetComponent<ItemUI>();

            // If the items are the same, try to stack them
            if (draggedItem.Item.ID == currentItem.Item.ID)
            {
                int amountToTransfer = currentItem.Item.Capacity - currentItem.Amount;
                if (amountToTransfer > 0)
                {
                    int transfer = Mathf.Min(amountToTransfer, draggedItem.Amount);
                    currentItem.AddAmount(transfer);
                    draggedItem.ReduceAmount(transfer);
                    if (draggedItem.Amount <= 0)
                    {
                        Destroy(draggedItem.gameObject);
                    }
                }
            }
            // If the items are different, swap them
            else
            {
                // Move the current item to the dragged item's original slot
                currentItem.transform.SetParent(originalParent);
                currentItem.transform.localPosition = Vector3.zero;

                // Move the dragged item to this slot
                draggedItem.transform.SetParent(transform);
                draggedItem.transform.localPosition = Vector3.zero;
            }
        }
    }
}