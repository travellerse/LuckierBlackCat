using HarmonyLib;
using LuckierBlackCat.Utils;
using System;

namespace LuckierBlackCat.Patches
{
    /// <summary>
    /// Chara.Pick 方法的补丁类
    /// 当玩家角色拾取装备时，自动触发黑猫的舔舐附魔效果
    /// </summary>
    [HarmonyPatch(typeof(Chara), "Pick", new Type[] { typeof(Thing), typeof(bool), typeof(bool) })]
    public static class CharaPickPatch
    {
        /// <summary>
        /// 在 Pick 方法执行前调用，检查并触发舔舐附魔
        /// </summary>
        /// <param name="__instance">调用方法的角色实例</param>
        /// <param name="t">要拾取的物品</param>
        /// <param name="msg">是否显示消息</param>
        /// <param name="tryStack">是否尝试堆叠</param>
        /// <returns>是否继续执行原方法</returns>
        private static bool Prefix(Chara __instance, ref Thing t, ref bool msg, ref bool tryStack)
        {
            // 只处理玩家角色的拾取行为
            if (!__instance.IsPC) return true;

            // 记录拾取行为的详细信息
            Logger.LogInfo("Chara " + __instance.Name + "::Pick " + t.Name + " - Msg: " + msg + ", TryStack: " + tryStack);

            // 检查物品是否符合舔舐条件
            if (!BlackCatUtils.IsItemEligibleForLicking(t))
            {
                return true;
            }

            // 寻找并执行舔舐附魔
            BlackCatUtils.TryLickItem(t, msg);

            return true; // 继续执行原方法
        }
    }

    /// <summary>
    /// ActPray.TryPray 方法的补丁类
    /// 当角色进行祈祷时，自动让黑猫对其身上符合条件的装备进行舔舐附魔
    /// </summary>
    [HarmonyPatch(typeof(ActPray), "TryPray", new Type[] { typeof(Chara), typeof(bool) })]
    public static class ActPrayTryPrayPatch
    {
        /// <summary>
        /// 在 TryPray 方法执行后调用，为祈祷角色的装备进行舔舐附魔
        /// </summary>
        /// <param name="c">进行祈祷的角色</param>
        /// <param name="passive">是否为被动祈祷</param>
        private static void Postfix(Chara c, bool passive)
        {
            // 记录祈祷行为
            Logger.LogInfo("ActPray::TryPray " + c.Name + " - Passive: " + passive);

            // 遍历地图上的所有角色，寻找有舔舐能力的黑猫
            foreach (Chara chara in EClass._map.charas)
            {
                // 检查角色是否拥有舔舐能力（元素ID 1412）
                if (chara.HasElement(1412, 1))
                {
                    // 遍历祈祷角色身上的所有物品
                    foreach (Thing thing in c.things)
                    {
                        // 检查物品是否符合舔舐条件
                        if (BlackCatUtils.IsItemEligibleForLicking(thing))
                        {
                            // 让黑猫说出舔舐的话并播放音效
                            chara.Say("lick", chara, thing, null, null);
                            thing.PlaySound("offering", 1f, true);

                            // 对物品进行舔舐附魔
                            thing.TryLickEnchant(chara, false);

                            // 记录附魔详细信息用于调试
                            Logger.LogInfo("[Enchant] " + c.Name + " (Passive: " + passive + ") - " + thing.Name + " " +
                                         "(Equipment: " + thing.IsEquipmentOrRanged + ", " +
                                         "NotCursed: " + !thing.IsCursed + ", " +
                                         "Rarity: " + thing.rarity + ", " +
                                         "EnchantStatus: " + thing.GetInt(107, null) + ")");
                        }
                    }
                }
            }
        }
    }
}
