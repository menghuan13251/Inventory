// Hotbar.cs (修正版)

using UnityEngine;

public class Hotbar : MonoBehaviour
{
    private HotbarSlot[] hotbarSlots;

    void Start()
    {
        hotbarSlots = GetComponentsInChildren<HotbarSlot>();
        // 初始化快捷键显示
        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            hotbarSlots[i].SetShortcutKey((i + 1).ToString());
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) UseSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) UseSlot(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) UseSlot(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) UseSlot(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) UseSlot(4);
        // ... 以此类推
    }

    private void UseSlot(int index)
    {
        if (index < hotbarSlots.Length)
        {
            hotbarSlots[index].UseShortcut();
        }
    }

    // 【新增】一个公共方法，用于更新所有包含指定物品ID的快捷栏
    public void UpdateHotbarAmounts(int itemID)
    {
        if (hotbarSlots == null) return;

        foreach (HotbarSlot slot in hotbarSlots)
        {
            if (slot.transform.childCount > 0 && slot.GetItemId() == itemID)
            {
                slot.UpdateAmountFromKnapsack();
            }
        }
    }
}