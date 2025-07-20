// --- START OF FILE Vendor.cs ---

using UnityEngine;
using System.Collections;
// REMOVED: Unused using statement
// using static UnityEditor.Progress; 

public class Vendor : Inventory
{
    // ... (no changes to singleton or other fields) ...
    #region 单例模式
    private static Vendor _instance;
    public static Vendor Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.Find("VendorPanel").GetComponent<Vendor>();
            }
            return _instance;
        }
    }
    #endregion

    public int[] itemIdArray;

    private Player player;

    public override void Start()
    {
        base.Start();
        InitShop();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        Hide();
    }

    private void InitShop()
    {
        foreach (int itemId in itemIdArray)
        {
            StoreItem(itemId);
        }
    }
    /// <summary>
    /// 主角购买
    /// </summary>
    /// <param name="item"></param>
    public void BuyItem(Item item)
    {
        bool isSuccess = player.ConsumeCoin(item.BuyPrice);
        if (isSuccess)
        {
            Knapsack.Instance.StoreItem(item);
            Debug.Log($"购买{item.Name}价格{item.BuyPrice}");
        }
    }

    // MODIFIED: SellItem now takes an ItemUI parameter from the drop event.
    public void SellItem(ItemUI itemUI)
    {
        int sellAmount = 1;
        // Holding control sells one, otherwise sell the whole stack.
        if (Input.GetKey(KeyCode.LeftControl))
        {
            sellAmount = 1;
        }
        else
        {
            sellAmount = itemUI.Amount;
        }

        int coinAmount = itemUI.Item.SellPrice * sellAmount;
        player.EarnCoin(coinAmount);

        // Reduce amount or destroy the item from its original slot
        itemUI.ReduceAmount(sellAmount);
        if (itemUI.Amount <= 0)
        {
            Destroy(itemUI.gameObject);
        }
        Debug.Log($"卖出{itemUI.Item.Name}价格{coinAmount}");
    }
}