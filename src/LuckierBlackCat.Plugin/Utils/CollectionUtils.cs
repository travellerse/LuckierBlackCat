using System.Collections.Generic;
using UnityEngine;

namespace LuckierBlackCat.Utils
{
    /// <summary>
    /// 集合操作工具类
    /// 提供各种集合操作的通用方法
    /// </summary>
    public static class CollectionUtils
    {
        /// <summary>
        /// Fisher-Yates 洗牌算法实现
        /// 对附魔源元素列表进行随机洗牌，确保每次获取附魔时都有不同的顺序
        /// </summary>
        /// <param name="list">需要洗牌的附魔源元素列表</param>
        /// <returns>洗牌后的新列表</returns>
        public static List<SourceElement.Row> Shuffle(System.Collections.Generic.IList<SourceElement.Row> list)
        {
            // 创建输入列表的副本
            List<SourceElement.Row> shuffledList = new List<SourceElement.Row>(list);

            // Fisher-Yates 洗牌算法
            for (int i = 0; i < shuffledList.Count; i++)
            {
                // 从当前位置到列表末尾随机选择一个索引
                int randomIndex = Random.Range(i, shuffledList.Count);

                // 交换当前位置和随机位置的元素
                SourceElement.Row temp = shuffledList[i];
                shuffledList[i] = shuffledList[randomIndex];
                shuffledList[randomIndex] = temp;
            }

            return shuffledList;
        }

        /// <summary>
        /// 通用的列表洗牌方法
        /// </summary>
        /// <typeparam name="T">列表元素类型</typeparam>
        /// <param name="list">要洗牌的列表</param>
        /// <returns>洗牌后的新列表</returns>
        public static List<T> Shuffle<T>(System.Collections.Generic.IList<T> list)
        {
            List<T> shuffledList = new List<T>(list);

            for (int i = 0; i < shuffledList.Count; i++)
            {
                int randomIndex = Random.Range(i, shuffledList.Count);
                T temp = shuffledList[i];
                shuffledList[i] = shuffledList[randomIndex];
                shuffledList[randomIndex] = temp;
            }

            return shuffledList;
        }
    }
}
