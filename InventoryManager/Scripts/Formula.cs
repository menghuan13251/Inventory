using UnityEngine; // 引入Unity引擎命名空间
using System.Collections; // 引入System.Collections命名空间（虽然在此代码中未直接使用）
using System.Collections.Generic; // 引入System.Collections.Generic命名空间，用于使用List等泛型集合

// 定义一个表示锻造公式的类
public class Formula
{
    // 定义两个物品的ID和数量属性，这些属性是只读的，只能通过构造函数设置
    public int Item1ID { get; private set; } // 第一个物品的ID
    public int Item1Amount { get; private set; } // 第一个物品的数量
    public int Item2ID { get; private set; } // 第二个物品的ID
    public int Item2Amount { get; private set; } // 第二个物品的数量

    // 定义锻造结果的物品ID属性，该属性也是只读的
    public int ResID { get; private set; } // 锻造结果的物品ID

    // 定义一个私有列表，用于存储锻造所需的所有物品ID
    private List<int> needIdList = new List<int>(); // 所需物品的ID列表

    // 定义一个公共属性，用于获取所需物品的ID列表（注意：这里没有提供设置方法，因此列表是只读的）
    public List<int> NeedIdList
    {
        get
        {
            return needIdList; // 返回所需物品的ID列表
        }
    }

    // 构造函数，用于初始化Formula类的实例
    public Formula(int item1ID, int item1Amount, int item2ID, int item2Amount, int resID)
    {
        // 设置两个物品的ID和数量
        this.Item1ID = item1ID;
        this.Item1Amount = item1Amount;
        this.Item2ID = item2ID;
        this.Item2Amount = item2Amount;
        // 设置锻造结果的物品ID
        this.ResID = resID;

        // 根据提供的物品数量和ID，填充所需物品的ID列表
        for (int i = 0; i < Item1Amount; i++)
        {
            needIdList.Add(Item1ID); // 将第一个物品的ID添加到列表中，数量为Item1Amount
        }
        for (int i = 0; i < Item2Amount; i++)
        {
            needIdList.Add(Item2ID); // 将第二个物品的ID添加到列表中，数量为Item2Amount
        }
    }

    // 定义一个方法，用于检查提供的物品ID列表是否满足锻造公式的要求
    public bool Match(List<int> idList) // 提供的物品ID列表
    {
        // 创建一个临时列表，用于遍历和修改，避免直接修改传入的列表
        List<int> tempIDList = new List<int>(idList);

        // 遍历所需物品的ID列表
        foreach (int id in needIdList)
        {
            // 尝试从临时列表中移除当前所需的物品ID
            bool isSuccess = tempIDList.Remove(id);
            // 如果移除失败，说明提供的物品ID列表中缺少某些所需物品
            if (isSuccess == false)
            {
                return false; // 返回false，表示不匹配
            }
        }
        // 如果所有所需物品ID都被成功移除，说明提供的物品ID列表满足锻造公式的要求
        return true; // 返回true，表示匹配
    }
}