using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {

    protected Slot[] slotList;

    private float targetAlpha = 1;

    private float smoothing = 4;

    private CanvasGroup canvasGroup;
    // 【新增】UI 引用
    public Transform tabsContainer; // 将UI中的 TabsContainer 拖到这里
    public GameObject tabPrefab;    // 将页签按钮预制体拖到这里
    public InputField searchInput;  // 将UI中的 InputField 拖到这里
    private Item.ItemType currentFilterType = Item.ItemType.All; // 新增一个"All"类型到你的枚举中
                                                                 // Use this for initialization
    public virtual void Start () {
        slotList = GetComponentsInChildren<Slot>();
        canvasGroup = GetComponent<CanvasGroup>();
        Hide();
        if (searchInput != null)
        {
            searchInput.onValueChanged.AddListener(OnSearchValueChanged);
        }
        GenerateTabs();
    }
    // 【新增】一个公共的UI刷新方法
    public void RefreshUI()
    {
        GenerateTabs();
        ApplyFilters();
    }
    // 【新增】当搜索框内容改变时调用
    private void OnSearchValueChanged(string text)
    {
        ApplyFilters();
    }

    // 【新增】生成分类页签
    public  void GenerateTabs()
    {
        if (tabsContainer == null || tabPrefab == null) return;

        // 清理旧页签
        foreach (Transform tab in tabsContainer) { Destroy(tab.gameObject); }

        // 创建一个“全部”页签
        CreateTab("全部", Item.ItemType.All);

        // 获取当前背包所有物品的类型
        HashSet<Item.ItemType> itemTypes = new HashSet<Item.ItemType>();
        foreach (Slot slot in slotList)
        {
            if (slot.transform.childCount > 0)
            {
                itemTypes.Add(slot.GetItemType());
            }
        }

        // 为每种类型创建一个页签
        foreach (Item.ItemType type in itemTypes)
        {
            CreateTab(GetChineseItemType(type), type);
        }
    }

    // 【新增】创建一个页签按钮
    private void CreateTab(string name, Item.ItemType type)
    {
        GameObject tabGO = Instantiate(tabPrefab, tabsContainer);
        tabGO.GetComponentInChildren<Text>().text = name;
        tabGO.GetComponent<Button>().onClick.AddListener(() =>
        {
            currentFilterType = type;
            ApplyFilters();
        });
    }

    // 【新增】应用所有过滤器（分类和搜索）
    public  void ApplyFilters()
    {
        string searchText = (searchInput != null) ? searchInput.text.ToLower() : "";

        foreach (Slot slot in slotList)
        {
            if (slot.transform.childCount > 0)
            {
                ItemUI itemUI = slot.transform.GetChild(0).GetComponent<ItemUI>();
                bool typeMatch = (currentFilterType == Item.ItemType.All || itemUI.Item.Type == currentFilterType);
                bool nameMatch = string.IsNullOrEmpty(searchText) || itemUI.Item.Name.ToLower().Contains(searchText);

                // 同时满足类型和搜索条件才显示
                slot.gameObject.SetActive(typeMatch && nameMatch);
            }
            else
            {
                // 空格子默认不显示，除非“全部”分类且没有搜索词
                slot.gameObject.SetActive(currentFilterType == Item.ItemType.All && string.IsNullOrEmpty(searchText));
            }
        }
    }

    // 【新增】辅助方法，用于将枚举转为中文显示
    private string GetChineseItemType(Item.ItemType type)
    {
        switch (type)
        {
            case Item.ItemType.Consumable: return "消耗品";
            case Item.ItemType.Equipment: return "装备";
            case Item.ItemType.Weapon: return "武器";
            case Item.ItemType.Material: return "材料";
            default: return "未知";
        }
    }

    void Update()
    {
        if (canvasGroup.alpha != targetAlpha)
        {
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, smoothing * Time.deltaTime);
            if (Mathf.Abs(canvasGroup.alpha - targetAlpha) < .01f)
            {
                canvasGroup.alpha = targetAlpha;
            }
        }
    }

    public bool StoreItem(int id)
    {
        Item item = InventoryManager.Instance.GetItemById(id);
      
        return StoreItem(item);
    }
    public bool StoreItem(Item item)
    {
        bool success = false; // 用于追踪是否存储成功
        if (item == null)
        {
            Debug.LogWarning("要存储的物品的id不存在");
            return false;
        }
        if (item.Capacity == 1)
        {
            Slot slot = FindEmptySlot();
            if (slot == null)
            {
                Debug.LogWarning("没有空的物品槽");
                return false;
            }
            else
            {
                slot.StoreItem(item);
                success = true;
            }
        }
        else
        {
            Slot slot = FindSameIdSlot(item);
            if (slot != null)
            {
                slot.StoreItem(item);
                success = true;
            }
            else
            {
                Slot emptySlot = FindEmptySlot();
                if (emptySlot != null)
                {
                    emptySlot.StoreItem(item);
                    success = true;
                }
                else
                {
                    Debug.LogWarning("没有空的物品槽");
                    return false;
                }
            }
        }

        if (success)
        {
            // 【关键】在成功存储后调用刷新
            RefreshUI();
        }

        // 我们也需要更新快捷栏
        FindObjectOfType<Hotbar>()?.UpdateHotbarAmounts(item.ID);

        return success;
    }

    /// <summary>
    /// 这个方法用来找到一个空的物品槽
    /// </summary>
    /// <returns></returns>
    private Slot FindEmptySlot()
    {
        foreach (Slot slot in slotList)
        {
            if (slot.transform.childCount == 0)
            {
                return slot;
            }
        }
        return null;
    }

    private Slot FindSameIdSlot(Item item)
    {
        foreach (Slot slot in slotList)
        {
            if (slot.transform.childCount >= 1 && slot.GetItemId() == item.ID &&slot.IsFilled()==false )
            {
                return slot;
            }
        }
        return null;
    }

    public void Show()
    {
        canvasGroup.blocksRaycasts = true;
        targetAlpha = 1;
    }
    public void Hide()
    {
        canvasGroup.blocksRaycasts = false;
        targetAlpha = 0;
    }
    public void DisplaySwitch()
    {
        if (targetAlpha == 0)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    #region save and load
    public void SaveInventory()
    {
        StringBuilder sb = new StringBuilder();
        foreach (Slot slot in slotList)
        {
            if (slot.transform.childCount > 0)
            {
                ItemUI itemUI = slot.transform.GetChild(0).GetComponent<ItemUI>();
                sb.Append(itemUI.Item.ID + ","+itemUI.Amount+"-");
            }
            else
            {
                sb.Append("0-");
            }
        }
        PlayerPrefs.SetString(this.gameObject.name, sb.ToString());
    }
    public void LoadInventory()
    {
        if (PlayerPrefs.HasKey(this.gameObject.name) == false) return;
        string str = PlayerPrefs.GetString(this.gameObject.name);
        //print(str);
        string[] itemArray = str.Split('-');
        for (int i = 0; i < itemArray.Length-1; i++)
        {
            string itemStr = itemArray[i];
            if (itemStr != "0")
            {
                //print(itemStr);
                string[] temp = itemStr.Split(',');
                int id = int.Parse(temp[0]);
                Item item = InventoryManager.Instance.GetItemById(id);
                int amount = int.Parse(temp[1]);
                for (int j = 0; j < amount; j++)
                {
                    slotList[i].StoreItem(item);
                }
            }
        }
        RefreshUI();
    }
    #endregion
}
