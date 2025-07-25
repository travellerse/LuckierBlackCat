namespace LuckierBlackCat.Utils
{
    /// <summary>
    /// 黑猫相关的工具类
    /// 提供黑猫舔舐功能的通用方法和逻辑
    /// </summary>
    public static class BlackCatUtils
    {
        #region 常量定义

        /// <summary>
        /// 黑猫舔舐能力的元素ID
        /// </summary>
        private const int LICK_ABILITY_ELEMENT_ID = 1412;

        /// <summary>
        /// 附魔状态检查的属性ID
        /// </summary>
        private const int ENCHANT_STATUS_PROPERTY_ID = 107;

        #endregion

        #region 物品检查方法

        /// <summary>
        /// 检查物品是否符合舔舐条件
        /// </summary>
        /// <param name="item">要检查的物品</param>
        /// <returns>是否符合舔舐条件</returns>
        public static bool IsItemEligibleForLicking(Thing item)
        {
            // 检查是否为装备或远程武器
            if (!item.IsEquipmentOrRanged)
            {
                return false;
            }

            // 跳过被诅咒的物品或普通品质及以下的物品
            if (item.IsCursed || item.rarity <= Rarity.Normal)
            {
                return false;
            }

            // 检查物品是否已经被附魔过
            if (item.GetInt(ENCHANT_STATUS_PROPERTY_ID, null) > 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 检查角色是否拥有舔舐能力
        /// </summary>
        /// <param name="character">要检查的角色</param>
        /// <returns>是否拥有舔舐能力</returns>
        public static bool HasLickAbility(Chara character)
        {
            return character.HasElement(LICK_ABILITY_ELEMENT_ID, 1);
        }

        #endregion

        #region 舔舐行为方法

        /// <summary>
        /// 尝试对物品进行舔舐附魔
        /// </summary>
        /// <param name="item">要舔舐的物品</param>
        /// <param name="showMessage">是否显示消息和音效</param>
        /// <returns>是否成功找到黑猫并进行舔舐</returns>
        public static bool TryLickItem(Thing item, bool showMessage = true)
        {
            // 遍历地图上的所有角色，寻找有舔舐能力的黑猫
            foreach (Chara character in EClass._map.charas)
            {
                if (HasLickAbility(character))
                {
                    PerformLickAction(character, item, showMessage);
                    return true; // 只需要一只黑猫进行舔舐
                }
            }

            return false; // 没有找到有舔舐能力的黑猫
        }

        /// <summary>
        /// 执行舔舐动作
        /// </summary>
        /// <param name="blackCat">执行舔舐的黑猫</param>
        /// <param name="item">要舔舐的物品</param>
        /// <param name="showMessage">是否显示消息和音效</param>
        private static void PerformLickAction(Chara blackCat, Thing item, bool showMessage)
        {
            if (showMessage)
            {
                // 让黑猫说出舔舐的话并播放音效
                blackCat.Say("lick", blackCat, item, null, null);
                item.PlaySound("offering", 1f, true);
            }

            // 对物品进行舔舐附魔
            item.TryLickEnchant(blackCat, false);

            // 记录舔舐行为
            Logger.LogInfo("Black cat " + blackCat.Name + " licked " + item.Name);
        }

        #endregion

        #region 批量处理方法

        /// <summary>
        /// 对角色身上所有符合条件的装备进行舔舐附魔
        /// </summary>
        /// <param name="character">目标角色</param>
        /// <param name="showMessage">是否显示消息和音效</param>
        /// <returns>成功舔舐的物品数量</returns>
        public static int LickAllEligibleItems(Chara character, bool showMessage = true)
        {
            int lickCount = 0;

            // 寻找有舔舐能力的黑猫
            Chara blackCat = FindBlackCatWithLickAbility();
            if (blackCat == null)
            {
                return 0;
            }

            // 遍历角色身上的所有物品
            foreach (Thing item in character.things)
            {
                if (IsItemEligibleForLicking(item))
                {
                    PerformLickAction(blackCat, item, showMessage);
                    lickCount++;
                }
            }

            return lickCount;
        }

        /// <summary>
        /// 查找地图上第一个有舔舐能力的黑猫
        /// </summary>
        /// <returns>找到的黑猫，如果没有则返回 null</returns>
        private static Chara FindBlackCatWithLickAbility()
        {
            foreach (Chara character in EClass._map.charas)
            {
                if (HasLickAbility(character))
                {
                    return character;
                }
            }
            return null;
        }

        #endregion
    }
}
