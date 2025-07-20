
using UnityEngine; // 引入Unity引擎命名空间
using System.Collections; // 引入系统集合命名空间
using UnityEngine.UI; // 引入Unity UI命名空间

public class CharacterPanel : Inventory // 定义一个继承自Inventory的CharacterPanel类
{
    #region 单例模式
    private static CharacterPanel _instance; // 定义一个静态私有变量_instance来保存类的唯一实例
    public static CharacterPanel Instance // 定义一个公共静态属性Instance来获取类的唯一实例
    {
        get
        {
            if (_instance == null) // 如果_instance为空
            {
                _instance = GameObject.Find("CharacterPanel").GetComponent<CharacterPanel>(); // 则查找名为"CharacterPanel"的游戏对象并获取其CharacterPanel组件作为实例
            }
            return _instance; // 返回实例
        }
    }
    #endregion

    private Text propertyText; // 定义一个Text类型的私有变量propertyText用于显示角色属性

    private Player player; // 定义一个Player类型的私有变量player用于引用玩家对象

    // 重写Start方法
    public override void Start()
    {
        base.Start(); // 调用基类的Start方法
        propertyText = transform.Find("PropertyPanel/Text").GetComponent<Text>(); // 在UI层级中找到"PropertyPanel/Text"并获取其Text组件
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>(); // 查找标签为"Player"的游戏对象并获取其Player组件
        UpdatePropertyText(); // 更新属性文本
        Hide(); // 隐藏角色面板
    }

    // 定义一个方法用于装备物品
    public void PutOn(Item item)
    {
        Item exitItem = null; // 定义一个临时变量exitItem用于存储被替换下来的物品
        foreach (Slot slot in slotList) // 遍历装备槽
        {
            EquipmentSlot equipmentSlot = (EquipmentSlot)slot; // 将槽位转换为装备槽
            if (equipmentSlot.IsRightItem(item)) // 如果装备槽接受该物品
            {
                if (equipmentSlot.transform.childCount > 0) // 如果装备槽已经有物品
                {
                    ItemUI currentItemUI = equipmentSlot.transform.GetChild(0).GetComponent<ItemUI>(); // 获取当前物品UI组件
                    exitItem = currentItemUI.Item; // 获取当前物品
                    currentItemUI.SetItem(item, 1); // 将当前物品替换为新物品
                   // Debug.Log($"替换装备{item.Name}");
                }
                else
                {
                    equipmentSlot.StoreItem(item); // 将物品存储到装备槽中
                   // Debug.Log($"装备{item.Name}");
                }
                break; // 跳出循环
            }
        }
        if (exitItem != null) // 如果有被替换下来的物品
            Knapsack.Instance.StoreItem(exitItem); // 则将其存储到背包中

        UpdatePropertyText(); // 更新属性文本
    }

    // 定义一个方法用于卸下物品
    public void PutOff(Item item)
    {
        Knapsack.Instance.StoreItem(item); // 将物品存储到背包中
        //Debug.Log($"卸下{item.Name}");
        UpdatePropertyText(); // 更新属性文本
    }

    // 定义一个私有方法用于更新属性文本
    private void UpdatePropertyText()
    {
        int strength = 0, intellect = 0, agility = 0, stamina = 0, damage = 0; // 定义属性变量并初始化为0
        foreach (EquipmentSlot slot in slotList) // 遍历装备槽
        {
            if (slot.transform.childCount > 0) // 如果装备槽有物品
            {
                Item item = slot.transform.GetChild(0).GetComponent<ItemUI>().Item; // 获取物品
                if (item is Equipment) // 如果物品是装备
                {
                    Equipment e = (Equipment)item; // 将物品转换为装备
                    strength += e.Strength; // 累加力量属性
                    intellect += e.Intellect; // 累加智力属性
                    agility += e.Agility; // 累加敏捷属性
                    stamina += e.Stamina; // 累加体力属性
                }
                else if (item is Weapon) // 如果物品是武器
                {
                    damage += ((Weapon)item).Damage; // 累加攻击力属性
                }
            }
        }
        strength += player.BasicStrength; // 累加玩家的基础力量
        intellect += player.BasicIntellect; // 累加玩家的基础智力
        agility += player.BasicAgility; // 累加玩家的基础敏捷
        stamina += player.BasicStamina; // 累加玩家的基础体力
        damage += player.BasicDamage; // 累加玩家的基础攻击力
        string text = string.Format("力量：{0}\n智力：{1}\n敏捷：{2}\n体力：{3}\n攻击力：{4} ", strength, intellect, agility, stamina, damage); // 格式化属性文本
        propertyText.text = text; // 将属性文本赋值给propertyText的text属性以显示
    }
}