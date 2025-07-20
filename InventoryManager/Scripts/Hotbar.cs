// Hotbar.cs (������)

using UnityEngine;

public class Hotbar : MonoBehaviour
{
    private HotbarSlot[] hotbarSlots;

    void Start()
    {
        hotbarSlots = GetComponentsInChildren<HotbarSlot>();
        // ��ʼ����ݼ���ʾ
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
        // ... �Դ�����
    }

    private void UseSlot(int index)
    {
        if (index < hotbarSlots.Length)
        {
            hotbarSlots[index].UseShortcut();
        }
    }

    // ��������һ���������������ڸ������а���ָ����ƷID�Ŀ����
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