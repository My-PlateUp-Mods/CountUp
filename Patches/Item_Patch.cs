using HarmonyLib;
using Kitchen;
using KitchenData;
using KitchenLib.Utils;

namespace KitchenCountUp.Patches
{
    [HarmonyPatch(typeof(Item))]
    internal class Item_Patch
    {
        [HarmonyPatch(nameof(Item.GetIconSet))]
        [HarmonyPostfix]
        public static void GetIconSet_Postfix(ref string __result, Item item)
        {
            if (!Mod.UpdateIconsPreference.Get())
                return;

            if (item.IsSplittable && item.PreventExplicitSplit)
                __result = "<sprite name=\"upgrade\">";
        }
    }
}
