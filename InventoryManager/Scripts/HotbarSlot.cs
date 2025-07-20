// HotbarSlot.cs (������)

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI; // ��Ҫ����UI

public class HotbarSlot : Slot
{
    // ����������ݼ��ı���������ʾ��ݼ������� '1', '2'��
    public Text shortcutKeyText;

    // ����������������ڱ༭�����ߴ��������ÿ�ݼ���ʾ
    public void SetShortcutKey(string key)
    {
        if (shortcutKeyText != null)
        {
            shortcutKeyText.text = key;
        }
    }

    // ��д OnDrop�������ֻ���ܿ�ݷ�ʽ�����ƶ���ʵ��Ʒ
    public override void OnDrop(PointerEventData eventData)
    {
        // ȷ���б���ק����Ʒ�����������Ǵ���һ�������������
        if (Slot.draggedItem != null && Slot.originalParent.GetComponent<HotbarSlot>() == null)
        {
            Item draggedItemData = Slot.draggedItem.Item;
            // �����ͨ��ֻ������Ʒ����
            if (draggedItemData is Consumable)
            {
                // �������ǰ��������ܴ��ڵľ���Ʒ
                if (transform.childCount > 0)
                {
                    Destroy(transform.GetChild(0).gameObject);
                }

                // �ڿ�����д���һ����Ʒ�ġ����񡱻򡰿�ݷ�ʽ��
                this.StoreItem(draggedItemData);
                // ע�⣺�������ǲ�����Ҫ��ԭʼ��Ʒ����Ϊ��ק�����������Զ��ص�ԭλ��
                // ����ֻ���ڿ����������һ�����á�
            }
        }
        // ����Ǵ���������������ģ��ͽ���λ��
        else if (Slot.draggedItem != null && Slot.originalParent.GetComponent<HotbarSlot>() != null)
        {
            // �����ǰ���ǿյ�
            if (transform.childCount == 0)
            {
                Slot.draggedItem.transform.SetParent(this.transform);
                Slot.draggedItem.transform.localPosition = Vector3.zero;
            }
            else // �����ǰ���ж������ͽ���
            {
                ItemUI currentItem = this.transform.GetChild(0).GetComponent<ItemUI>();
                currentItem.transform.SetParent(Slot.originalParent);
                currentItem.transform.localPosition = Vector3.zero;

                Slot.draggedItem.transform.SetParent(this.transform);
                Slot.draggedItem.transform.localPosition = Vector3.zero;
            }
        }
    }

    // ���޸ġ���������ڿ�����ק�ˣ����������������
    // public override void OnBeginDrag(PointerEventData eventData)
    // {
    //     // �����Խ�����ק
    // }

    // ʹ�ÿ������Ʒ���߼�
    public void UseShortcut()
    {
        if (transform.childCount > 0)
        {
            // �������������Ǵ���Ĺؼ���
            int itemIDToUse = GetItemId(); // 1. �Ȼ�ȡ��ƷID
            Item itemData = InventoryManager.Instance.GetItemById(itemIDToUse); // 2. ͨ��ID�ӹ�������ȡ������Item����

            if (itemData == null) // ��ȫ���
            {
                Debug.LogError($"�������ƷID {itemIDToUse} ��Ч���޷���InventoryManager���ҵ���");
                return;
            }

            // �������������ҵ����ID����Ʒ������һ��
            // ������Ҫ���� Knapsack.cs ��ʵ�� ConsumeItemByID ����
            bool consumed = Knapsack.Instance.ConsumeItemByID(itemIDToUse);

            if (consumed)
            {
                Debug.Log($"ͨ�������ʹ���� {itemData.Name}");

                // ���¿����UI������
                ItemUI itemUI = transform.GetChild(0).GetComponent<ItemUI>();

                // **��Ҫ�߼�**�����������Ʒ����Ӧ���뱳���е�ʵ������ͬ��
                // ������Ҫһ�����������¿����
                UpdateAmountFromKnapsack();
            }
            else
            {
                Debug.Log($"������û�� {itemData.Name} �ˣ�");
                // ���������û���ˣ�ҲӦ����������
                Destroy(transform.GetChild(0).gameObject);
            }
        }
    }

    // ��������һ�������������ڿ������Ʒ�����仯ʱ���ӱ���ͬ�����µ�����
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
                // ���������һ����û���ˣ������ٿ�ݷ�ʽ
                Destroy(itemUI.gameObject);
            }
        }
    }
}