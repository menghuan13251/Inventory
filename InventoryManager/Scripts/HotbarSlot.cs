// HotbarSlot.cs (修正版)

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI; // 需要引入UI

public class HotbarSlot : Slot
{
    // 【新增】快捷键文本，用于显示快捷键（例如 '1', '2'）
    public Text shortcutKeyText;

    // 这个方法可以让你在编辑器或者代码里设置快捷键显示
    public void SetShortcutKey(string key)
    {
        if (shortcutKeyText != null)
        {
            shortcutKeyText.text = key;
        }
    }

    // 重写 OnDrop，快捷栏只接受快捷方式，不移动真实物品
    public override void OnDrop(PointerEventData eventData)
    {
        // 确保有被拖拽的物品，并且它不是从另一个快捷栏拖来的
        if (Slot.draggedItem != null && Slot.originalParent.GetComponent<HotbarSlot>() == null)
        {
            Item draggedItemData = Slot.draggedItem.Item;
            // 快捷栏通常只放消耗品或技能
            if (draggedItemData is Consumable)
            {
                // 清理掉当前快捷栏可能存在的旧物品
                if (transform.childCount > 0)
                {
                    Destroy(transform.GetChild(0).gameObject);
                }

                // 在快捷栏中创建一个物品的“镜像”或“快捷方式”
                this.StoreItem(draggedItemData);
                // 注意：这里我们不再需要管原始物品，因为拖拽结束后它会自动回到原位。
                // 我们只是在快捷栏创建了一个引用。
            }
        }
        // 如果是从其他快捷栏拖来的，就交换位置
        else if (Slot.draggedItem != null && Slot.originalParent.GetComponent<HotbarSlot>() != null)
        {
            // 如果当前格是空的
            if (transform.childCount == 0)
            {
                Slot.draggedItem.transform.SetParent(this.transform);
                Slot.draggedItem.transform.localPosition = Vector3.zero;
            }
            else // 如果当前格有东西，就交换
            {
                ItemUI currentItem = this.transform.GetChild(0).GetComponent<ItemUI>();
                currentItem.transform.SetParent(Slot.originalParent);
                currentItem.transform.localPosition = Vector3.zero;

                Slot.draggedItem.transform.SetParent(this.transform);
                Slot.draggedItem.transform.localPosition = Vector3.zero;
            }
        }
    }

    // 【修改】快捷栏现在可以拖拽了，方便玩家整理快捷栏
    // public override void OnBeginDrag(PointerEventData eventData)
    // {
    //     // 留空以禁用拖拽
    // }

    // 使用快捷栏物品的逻辑
    public void UseShortcut()
    {
        if (transform.childCount > 0)
        {
            // 【修正】这里是错误的关键点
            int itemIDToUse = GetItemId(); // 1. 先获取物品ID
            Item itemData = InventoryManager.Instance.GetItemById(itemIDToUse); // 2. 通过ID从管理器获取完整的Item数据

            if (itemData == null) // 安全检查
            {
                Debug.LogError($"快捷栏物品ID {itemIDToUse} 无效，无法在InventoryManager中找到！");
                return;
            }

            // 在整个背包里找到这个ID的物品并消耗一个
            // 我们需要先在 Knapsack.cs 中实现 ConsumeItemByID 方法
            bool consumed = Knapsack.Instance.ConsumeItemByID(itemIDToUse);

            if (consumed)
            {
                Debug.Log($"通过快捷栏使用了 {itemData.Name}");

                // 更新快捷栏UI的数量
                ItemUI itemUI = transform.GetChild(0).GetComponent<ItemUI>();

                // **重要逻辑**：快捷栏的物品数量应该与背包中的实际数量同步
                // 我们需要一个方法来更新快捷栏
                UpdateAmountFromKnapsack();
            }
            else
            {
                Debug.Log($"背包里没有 {itemData.Name} 了！");
                // 如果背包里没有了，也应该清理快捷栏
                Destroy(transform.GetChild(0).gameObject);
            }
        }
    }

    // 【新增】一个方法，用于在快捷栏物品数量变化时，从背包同步最新的数量
    public void UpdateAmountFromKnapsack()
    {
        if (transform.childCount > 0)
        {
            ItemUI itemUI = transform.GetChild(0).GetComponent<ItemUI>();
            int amountInKnapsack = Knapsack.Instance.GetItemCountByID(itemUI.Item.ID);

            if (amountInKnapsack > 0)
            {
                itemUI.SetAmount(amountInKnapsack);
            }
            else
            {
                // 如果背包里一个都没有了，就销毁快捷方式
                Destroy(itemUI.gameObject);
            }
        }
    }
}