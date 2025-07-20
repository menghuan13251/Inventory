// --- START OF FILE InventoryManager.cs ---

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    // ... (no changes to singleton or itemList) ...
    #region 单例模式
    private static InventoryManager _instance;

    public static InventoryManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.Find("InventoryManager").GetComponent<InventoryManager>();
            }
            return _instance;
        }
    }
    #endregion

    private List<Item> itemList;

    #region ToolTip
    private ToolTip toolTip;
    private bool isToolTipShow = false;
    private Vector2 toolTipPosionOffset = new Vector2(10, -10);
    #endregion

    private Canvas canvas;

    // --- REMOVED: The entire PickedItem system is no longer needed for drag-and-drop ---
    /*
    private bool isPickedItem = false;

    public bool IsPickedItem
    {
        get
        {
            return isPickedItem;
        }
    }

    private ItemUI pickedItem;//鼠标选中的物体

    public ItemUI PickedItem
    {
        get
        {
            return pickedItem;
        }
    }
    */
    // --- END OF REMOVED SECTION ---


    void Awake()
    {
        ParseItemJson();
    }

    void Start()
    {
        toolTip = GameObject.FindObjectOfType<ToolTip>();
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();

        // REMOVED: pickedItem is no longer used
        // pickedItem = GameObject.Find("PickedItem").GetComponent<ItemUI>();
        // pickedItem.Hide();
    }

    void Update()
    {
        // REMOVED: Logic for moving pickedItem with mouse
        /*
        if (isPickedItem)
        {
            //...
        }
        */

        // MODIFIED: Simplified Update loop
        //if (isToolTipShow)
        //{
        //    //控制提示面板跟随鼠标
        //    Vector2 position;
        //    RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, null, out position);
        //    toolTip.SetLocalPotion(position + toolTipPosionOffset);
        //}
        if (Input.GetMouseButtonDown(0) && UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject() == false)
        {
            // HideContextMenu();
            InventoryManager.Instance.HideToolTip();
        }
        // REMOVED: Logic for dropping item is now handled by OnEndDrag
        /*
        if (isPickedItem && Input.GetMouseButtonDown(0) && UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(-1)==false)
        {
            isPickedItem = false;
            PickedItem.Hide();
        }
        */
    }

    // ... (ParseItemJson and GetItemById remain the same) ...
    void ParseItemJson()
    {
        itemList = new List<Item>();
        //文本为在Unity里面是 TextAsset类型
        TextAsset itemText = Resources.Load<TextAsset>("Items");
        string itemsJson = itemText.text;//物品信息的Json格式
        JSONObject j = new JSONObject(itemsJson);
        foreach (JSONObject temp in j.list)
        {
            string typeStr = temp["type"].str;
            Item.ItemType type = (Item.ItemType)System.Enum.Parse(typeof(Item.ItemType), typeStr);

            //下面的事解析这个对象里面的公有属性
            int id = (int)(temp["id"].n);
            string name = temp["name"].str;
            Item.ItemQuality quality = (Item.ItemQuality)System.Enum.Parse(typeof(Item.ItemQuality), temp["quality"].str);
            string description = temp["description"].str;
            int capacity = (int)(temp["capacity"].n);
            int buyPrice = (int)(temp["buyPrice"].n);
            int sellPrice = (int)(temp["sellPrice"].n);
            string sprite = temp["sprite"].str;

            Item item = null;
            switch (type)
            {
                case Item.ItemType.Consumable:
                    int hp = (int)(temp["hp"].n);
                    int mp = (int)(temp["mp"].n);
                    item = new Consumable(id, name, type, quality, description, capacity, buyPrice, sellPrice, sprite, hp, mp);
                    break;
                case Item.ItemType.Equipment:
                    //
                    int strength = (int)temp["strength"].n;
                    int intellect = (int)temp["intellect"].n;
                    int agility = (int)temp["agility"].n;
                    int stamina = (int)temp["stamina"].n;
                    Equipment.EquipmentType equipType = (Equipment.EquipmentType)System.Enum.Parse(typeof(Equipment.EquipmentType), temp["equipType"].str);
                    item = new Equipment(id, name, type, quality, description, capacity, buyPrice, sellPrice, sprite, strength, intellect, agility, stamina, equipType);
                    break;
                case Item.ItemType.Weapon:
                    //
                    int damage = (int)temp["damage"].n;
                    Weapon.WeaponType wpType = (Weapon.WeaponType)System.Enum.Parse(typeof(Weapon.WeaponType), temp["weaponType"].str);
                    item = new Weapon(id, name, type, quality, description, capacity, buyPrice, sellPrice, sprite, damage, wpType);
                    break;
                case Item.ItemType.Material:
                    //
                    item = new Material(id, name, type, quality, description, capacity, buyPrice, sellPrice, sprite);
                    break;
            }
            itemList.Add(item);
            //Debug.Log(item);
        }
    }
    public Item GetItemById(int id)
    {
        foreach (Item item in itemList)
        {
            if (item.ID == id)
            {
                return item;
            }
        }
        return null;
    }


    public void ShowToolTip(string content)
    {// 【新增】在设置位置前，先调整 Pivot
        toolTip.AdjustPivot(Input.mousePosition);
        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, null, out position);
        toolTip.SetLocalPotion(position + toolTipPosionOffset);
        // MODIFIED: Removed isPickedItem check
        isToolTipShow = true;
        toolTip.Show(content);
    }

    public void HideToolTip()
    {
        isToolTipShow = false;
        toolTip.Hide();
    }

    // REMOVED: PickupItem and RemoveItem are no longer needed as drag-and-drop handles this directly.
    /*
    public void PickupItem(Item item,int amount) { ... }
    public void RemoveItem(int amount=1) { ... }
    */

    // ... (SaveInventory and LoadInventory remain the same) ...
    public void SaveInventory()
    {
        Knapsack.Instance.SaveInventory();
        Chest.Instance.SaveInventory();
        CharacterPanel.Instance.SaveInventory();
        Forge.Instance.SaveInventory();
        PlayerPrefs.SetInt("CoinAmount", GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().CoinAmount);
    }

    public void LoadInventory()
    {
        Knapsack.Instance.LoadInventory();
        Chest.Instance.LoadInventory();
        CharacterPanel.Instance.LoadInventory();
        Forge.Instance.LoadInventory();
        if (PlayerPrefs.HasKey("CoinAmount"))
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().CoinAmount = PlayerPrefs.GetInt("CoinAmount");
        }
    }
}