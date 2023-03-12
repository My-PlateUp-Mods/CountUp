using HarmonyLib;
using Kitchen;
using KitchenCountUp.Views;
using TMPro;
using UnityEngine;

namespace KitchenCountUp.Patches
{
    [HarmonyPatch(typeof(ItemView))]
    internal class ItemView_Patch
    {
        [HarmonyPatch(nameof(ItemView.Initialise))]
        [HarmonyPostfix]
        internal static void Initialise_Postfix(ItemView __instance, ref TextMeshPro ___ProcessIcons)
        {
            __instance.gameObject.AddComponent<ItemIconView>().ProcessIcons = ___ProcessIcons;
            ___ProcessIcons.color = Color.white;
            ___ProcessIcons.gameObject.transform.localPosition = Vector3.up * 0.75f;
            ___ProcessIcons = null;
        }
    }
}
