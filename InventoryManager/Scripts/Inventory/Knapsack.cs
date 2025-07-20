using UnityEngine;
using System.Collections;

public class Knapsack : Inventory
{

    #region 单例模式
    private static Knapsack _instance;
    public static Knapsack Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance =  GameObject.Find("KnapsackPanel").GetComponent<Knapsack>();
            }
            return _instance;
        }
    }
    #endregion
    public bool ConsumeItem(int id)
    {
        foreach (Slot slot in slotList)
        {
            if (slot.transform.childCount > 0 && slot.GetItemId() == id)
            {
                ItemUI itemUI = slot.transform.GetChild(0).GetComponent<ItemUI>();
                itemUI.ReduceAmount(1);
                if (itemUI.Amount <= 0)
                {
                    Destroy(itemUI.gameObject);
                }
                return true; // 消耗成功
            }
        }
        return false; // 背包里没找到
    }// 【新增】根据ID消耗一个物品
    public bool ConsumeItemByID(int id)
    {
        foreach (Slot slot in slotList)
        {
            if (slot.transform.childCount > 0 && slot.GetItemId() == id)
            {
                ItemUI itemUI = slot.transform.GetChild(0).GetComponent<ItemUI>();
                itemUI.ReduceAmount(1);

                // 通知快捷栏更新数量
                FindObjectOfType<Hotbar>()?.UpdateHotbarAmounts(id);

                if (itemUI.Amount <= 0)
                {
                    Destroy(itemUI.gameObject);
                }
                return true; // 消耗成功
            }
        }
        return false; // 背包里没找到
    }

    // 【新增】根据ID获取物品在背包中的总数量
    public int GetItemCountByID(int id)
    {
        int totalAmount = 0;
        foreach (Slot slot in slotList)
        {
            if (slot.transform.childCount > 0 && slot.GetItemId() == id)
            {
                totalAmount += slot.transform.GetChild(0).GetComponent<ItemUI>().Amount;
            }
        }
        return totalAmount;
    }

    // 【修改】当物品被存入或堆叠时，也需要更新快捷栏
    public bool StoreItem(Item item)
    {
        // ... 原有的 StoreItem 逻辑 ...

        // 在方法的最后，当存储成功时 (return true 之前)
        FindObjectOfType<Hotbar>()?.UpdateHotbarAmounts(item.ID);
        return true;
    }
}
