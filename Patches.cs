using HarmonyLib;
using Kitchen;
using KitchenData;
using KitchenLib.Utils;
using TMPro;
using UnityEngine;

namespace KitchenCountUp
{
    [HarmonyPatch(typeof(ItemView), "UpdateData")]
    class ItemViewPatch
    {
        [HarmonyPostfix]
        public static void Postfix(ItemView.ViewData ___Data, TextMeshPro ___ProcessIcons, GameObject ___PrefabContainer)
        {
            Item item;
            var container = ___PrefabContainer.GetComponent<CounterContainer>();
            var splittable = ___PrefabContainer.GetComponent<SplittableItemView>();

            // Ignore non-splittables & oil-based items
            if (!GameData.Main.TryGet(___Data.ItemID, out item) || item.SplitCount == 0 || item.SplitCount > 100)
            {
                // Clean up
                if (container != null)
                    Object.Destroy(container);
                if (splittable != null)
                    Object.Destroy(splittable);

                return;
            }

            // Setup Monobehaviours
            if (splittable == null)
                ___PrefabContainer.AddComponent<SplitCounterView>();

            if (container == null)
            {
                container = ___PrefabContainer.AddComponent<CounterContainer>();
                container.Remaining = item.SplitCount;
                container.Additional = item.SplitDepletedItems.Contains(item.SplitSubItem) ? 1 : 0;
                ___ProcessIcons.color = Color.white;
            }

            // Update container
            container.TMP = ___ProcessIcons;
            container.IconString = Item.GetIconSet(item);
            container.UpdateText();
        }
    }

    [HarmonyPatch(typeof(ObjectsSplittableView), nameof(ObjectsSplittableView.UpdateData))]
    class ObjectsSplittableViewPatch
    {
        [HarmonyPostfix]
        public static void Postfix(ObjectsSplittableView __instance, SplittableItemView.ViewData __0)
        {
            var container = __instance.gameObject.GetComponent<CounterContainer>();
            if (container == null)
                return;

            // Update container
            container.Remaining = __0.Remaining;
            container.UpdateText();
        }
    }

    [HarmonyPatch(typeof(PositionSplittableView), nameof(PositionSplittableView.UpdateData))]
    class PositionSplittableViewPatch
    {
        [HarmonyPostfix]
        public static void Postfix(PositionSplittableView __instance, SplittableItemView.ViewData __0)
        {
            var container = __instance.gameObject.GetComponent<CounterContainer>();
            if (container == null)
                return;

            // Update container
            container.Remaining = __0.Remaining;
            container.UpdateText();
        }
    }

}
